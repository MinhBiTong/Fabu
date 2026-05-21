using Application.Common.Caching;
using Application.DTOs.Requests.PaymentRequest;
using Application.DTOs.Responses.PaymentResponse;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Options;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class PaymentTransactionSagaService : IPaymentTransactionSagaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnumerable<IPaymentGateway> _paymentGateways;
        private readonly ICouponService _couponService;
        private readonly IResponseCacheService? _cacheService;
        private readonly ILogger<PaymentTransactionSagaService> _logger;

        public PaymentTransactionSagaService(
            IUnitOfWork unitOfWork,
            IEnumerable<IPaymentGateway> paymentGateways,
            ICouponService couponService,
            ILogger<PaymentTransactionSagaService> logger,
            IResponseCacheService? cacheService = null)
        {
            _unitOfWork = unitOfWork;
            _paymentGateways = paymentGateways;
            _couponService = couponService;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<PaymentResponse> StartAsync(PaymentCreateRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Amount <= 0)
                throw new AppException(ErrorCode.INVALID_AMOUNT, "The amount is not valid.");

            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var customer = await ResolveCustomerAsync(request);
                EnsureActiveCustomerUser(customer);

                var account = customer.Account ?? await _unitOfWork.Accounts.GetByCustomerIdAsync(customer.Id);
                if (account is null)
                    throw new InvalidOperationException("Customer account not found.");

                EnsureActiveAccount(account);

                var bill = await ResolveBillAsync(request, customer);
                var transactionType = NormalizeTransactionType(request.TransactionType, customer, bill);
                var customerType = ResolveCustomerType(customer, bill);
                var isPostpaid = IsPostpaid(customerType);

                var originalAmount = request.Amount;
                var payableAmount = originalAmount;
                var discountApplied = 0m;
                CouponUsage? couponUsage = null;

                if (!string.IsNullOrWhiteSpace(request.CouponCode))
                {
                    var couponResult = await _couponService.ApplyCouponAsync(
                        request.CouponCode,
                        customer.Id,
                        originalAmount,
                        transactionType);

                    if (couponResult.IsSuccess)
                    {
                        payableAmount = couponResult.FinalAmount;
                        couponUsage = couponResult.CouponUsage;
                        discountApplied = originalAmount - payableAmount;
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Coupon {CouponCode} rejected for customer {CustomerId}: {Reason}",
                            request.CouponCode,
                            customer.Id,
                            couponResult.Message);
                    }
                }

                ValidateBillAmount(bill, payableAmount);
                var balanceBefore = account.Balance;
                var completesImmediately = request.UseAccountBalance || request.PaymentMethod == PaymentMethod.Cash;
                var paymentRef = request.PaymentRef ?? $"PAY_{Guid.NewGuid():N}".ToUpperInvariant();
                var transactionRef = BuildTransactionRef(transactionType);

                request.PaymentRef = paymentRef;
                request.Amount = payableAmount;
                request.OrderInfo ??= $"Fabu {transactionType} {transactionRef}";

                if (completesImmediately)
                {
                    EnsureEnoughBalance(account, payableAmount, isPostpaid, transactionType);
                    ApplyAccountAndBillSettlement(account, bill, transactionType, payableAmount, now, debitAccount: ShouldDebitAccount(transactionType));
                }

                var payment = new Payment
                {
                    Amount = payableAmount,
                    PaymentMethod = request.PaymentMethod,
                    PaymentRef = paymentRef,
                    BillId = bill?.Id ?? request.BillId,
                    Status = completesImmediately ? StatusPayment.Completed : StatusPayment.Pending,
                    PaymentDate = now,
                    Transactions = new List<Transaction>()
                };

                var transactionEntity = new Transaction
                {
                    CustomerId = customer.Id,
                    Payment = payment,
                    Amount = payableAmount,
                    TransactionRef = transactionRef,
                    TransactionType = transactionType,
                    PaymentMethod = request.PaymentMethod,
                    Status = completesImmediately ? StatusTransaction.Success : StatusTransaction.Pending,
                    CompletedAt = completesImmediately ? now : null,
                    CouponUsages = new List<CouponUsage>()
                };

                payment.Transactions.Add(transactionEntity);
                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                if (couponUsage is not null)
                {
                    couponUsage.TransactionId = transactionEntity.Id;
                    couponUsage.Status = completesImmediately ? "Success" : "Pending";
                    await _unitOfWork.CouponUsages.AddAsync(couponUsage);
                }

                string? paymentUrl = null;
                if (!completesImmediately)
                {
                    var gateway = ResolveGateway(request.PaymentMethod);
                    paymentUrl = await gateway.CreatePaymentUrlAsync(request);
                }

                await AddAuditLogAsync(
                    customer.UserId,
                    completesImmediately ? "PaymentSaga.Completed" : "PaymentSaga.PendingGateway",
                    "Payment",
                    payment.Id,
                    $"Ref={paymentRef}; Transaction={transactionRef}; Customer={customer.Id}; Type={transactionType}; Original={originalAmount}; Payable={payableAmount}; Discount={discountApplied}; BalanceBefore={balanceBefore}; BalanceAfter={account.Balance}");

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                await InvalidatePaymentCachesAsync(paymentRef, transactionRef);

                return BuildPaymentResponse(
                    payment,
                    transactionEntity,
                    customer,
                    balanceBefore,
                    account.Balance,
                    discountApplied,
                    paymentUrl);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Payment transaction saga failed while starting payment");
                throw;
            }
        }

        public async Task<PaymentCallbackResult> CompleteAsync(
            string providerName,
            Dictionary<string, string> callbackData,
            CancellationToken cancellationToken = default)
        {
            var gateway = ResolveGateway(providerName);
            var gatewayResult = await gateway.HandleCallbackAsync(callbackData);
            if (!gatewayResult.IsSuccess)
            {
                await MarkPaymentFailedIfPossibleAsync(gatewayResult.PaymentRef, providerName, gatewayResult.Message);
                return gatewayResult;
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var payment = await _unitOfWork.Payments.GetByPaymentRefAsync(gatewayResult.PaymentRef);
                if (payment is null)
                    return PaymentCallbackResult.Failed("Payment not found.");

                if (payment.Status == StatusPayment.Completed)
                    return PaymentCallbackResult.Success(payment.PaymentRef, providerName, gatewayResult.RawData);

                var now = DateTime.UtcNow;
                payment.Status = StatusPayment.Completed;
                payment.PaymentDate = now;

                decimal? balanceBefore = null;
                decimal? balanceAfter = null;
                long? userId = null;
                string? lastTransactionRef = null;

                foreach (var transactionEntity in payment.Transactions)
                {
                    if (transactionEntity.CustomerId is null)
                        continue;

                    var customer = await _unitOfWork.Customers.GetWithAccountAsync(transactionEntity.CustomerId.Value)
                        ?? throw new AppException(ErrorCode.CUSTOMER_NOT_FOUND);
                    EnsureActiveCustomerUser(customer);

                    var account = customer.Account ?? await _unitOfWork.Accounts.GetByCustomerIdAsync(customer.Id)
                        ?? throw new InvalidOperationException("Customer account not found.");
                    EnsureActiveAccount(account);

                    balanceBefore ??= account.Balance;
                    var bill = payment.BillId.HasValue
                        ? await _unitOfWork.PostpaidBills.GetByIdAsync(payment.BillId.Value)
                        : null;

                    ApplyAccountAndBillSettlement(
                        account,
                        bill,
                        transactionEntity.TransactionType,
                        payment.Amount,
                        now,
                        debitAccount: false);

                    transactionEntity.Status = StatusTransaction.Success;
                    transactionEntity.CompletedAt = now;

                    var couponUsage = await _unitOfWork.CouponUsages.GetByTransactionIdAsync(transactionEntity.Id);
                    if (couponUsage is not null)
                    {
                        couponUsage.Status = "Success";
                    }

                    balanceAfter = account.Balance;
                    userId = customer.UserId;
                    lastTransactionRef = transactionEntity.TransactionRef;
                }

                await AddAuditLogAsync(
                    userId,
                    "PaymentSaga.CompletedGateway",
                    "Payment",
                    payment.Id,
                    $"Ref={payment.PaymentRef}; Provider={providerName}; Transaction={lastTransactionRef}; Amount={payment.Amount}; BalanceBefore={balanceBefore}; BalanceAfter={balanceAfter}");

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
                await InvalidatePaymentCachesAsync(payment.PaymentRef, lastTransactionRef);

                return PaymentCallbackResult.Success(payment.PaymentRef, providerName, gatewayResult.RawData);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Payment transaction saga failed while completing provider callback {Provider}", providerName);
                return PaymentCallbackResult.Failed("Failed to complete payment callback.");
            }
        }

        private async Task<Customer> ResolveCustomerAsync(PaymentCreateRequest request)
        {
            long? customerId = request.CustomerId;

            if (!customerId.HasValue && request.BillId.HasValue)
            {
                var bill = await _unitOfWork.PostpaidBills.GetByIdAsync(request.BillId.Value);
                customerId = bill?.CustomerId;
            }

            if (!customerId.HasValue && !string.IsNullOrWhiteSpace(request.MobileNumber))
            {
                var customerByMobile = await _unitOfWork.Customers.GetByMobileNumberAsync(request.MobileNumber);
                customerId = customerByMobile?.Id;
            }

            if (!customerId.HasValue)
                throw new AppException(ErrorCode.CUSTOMER_NOT_FOUND);

            var customer = await _unitOfWork.Customers.GetWithAccountAsync(customerId.Value);
            return customer ?? throw new AppException(ErrorCode.CUSTOMER_NOT_FOUND);
        }

        private async Task<PostpaidBill?> ResolveBillAsync(PaymentCreateRequest request, Customer customer)
        {
            if (!request.BillId.HasValue) return null;

            var bill = await _unitOfWork.PostpaidBills.GetByIdAsync(request.BillId.Value);
            if (bill is null)
                throw new InvalidOperationException("Postpaid bill not found.");

            if (bill.CustomerId != customer.Id)
                throw new InvalidOperationException("Postpaid bill does not belong to this customer.");

            return bill;
        }

        private IPaymentGateway ResolveGateway(PaymentMethod paymentMethod)
            => ResolveGateway(paymentMethod.ToString());

        private IPaymentGateway ResolveGateway(string providerName)
        {
            var gateway = _paymentGateways.FirstOrDefault(gateway =>
                string.Equals(gateway.GetProviderName(), providerName, StringComparison.OrdinalIgnoreCase));

            return gateway ?? throw new AppException(ErrorCode.PAYMENT_PROVIDER_NOT_SUPPORTED);
        }

        private static string NormalizeTransactionType(string? transactionType, Customer customer, PostpaidBill? bill)
        {
            if (bill is not null)
                return "BillPayment";

            if (!string.IsNullOrWhiteSpace(transactionType))
                return transactionType.Trim();

            return IsPostpaid(customer.CustomerType) ? "BillPayment" : "Recharge";
        }

        private static string ResolveCustomerType(Customer customer, PostpaidBill? bill)
            => bill is not null ? "Postpaid" : customer.CustomerType;

        private static bool IsPostpaid(string? customerType)
            => string.Equals(customerType, "Postpaid", StringComparison.OrdinalIgnoreCase);

        private static string BuildTransactionRef(string transactionType)
        {
            var prefix = transactionType switch
            {
                "BillPayment" => "BILL",
                "ServiceActivation" => "SVC",
                "Recharge" => "RECH",
                _ => "TRX"
            };

            return $"{prefix}_{Guid.NewGuid():N}".ToUpperInvariant();
        }

        private static bool ShouldDebitAccount(string transactionType)
            => !string.Equals(transactionType, "Recharge", StringComparison.OrdinalIgnoreCase);

        private static void EnsureActiveCustomerUser(Customer customer)
        {
            if (customer.UserId.HasValue && customer.User is not { IsActive: true })
            {
                throw new AppException(ErrorCode.UNAUTHORIZED, "Customer user is not active.");
            }
        }

        private static void EnsureActiveAccount(Account account)
        {
            if (account.Status != StatusAccount.Active)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED, "Customer account is not active.");
            }
        }

        private static void EnsureEnoughBalance(Account account, decimal amount, bool isPostpaid, string transactionType)
        {
            if (!ShouldDebitAccount(transactionType))
            {
                return;
            }

            var available = account.Balance + (isPostpaid ? account.CreditLimit : 0);
            if (available < amount)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED, "Account balance is not enough for this transaction.");
            }
        }

        private static void ValidateBillAmount(PostpaidBill? bill, decimal amount)
        {
            if (bill is null) return;

            if (bill.Status == StatusPostpaid.Paid)
            {
                throw new InvalidOperationException("Postpaid bill has already been paid.");
            }

            var remaining = bill.TotalAmount - bill.PaidAmount;
            if (amount > remaining)
            {
                throw new InvalidOperationException("Payment amount is greater than the remaining bill amount.");
            }
        }

        private static void ApplyAccountAndBillSettlement(
            Account account,
            PostpaidBill? bill,
            string transactionType,
            decimal amount,
            DateTime now,
            bool debitAccount)
        {
            if (string.Equals(transactionType, "Recharge", StringComparison.OrdinalIgnoreCase))
            {
                account.Balance += amount;
                account.LastRechargeDate = now;
            }
            else if (debitAccount)
            {
                account.Balance -= amount;
            }

            if (bill is not null)
            {
                bill.PaidAmount += amount;
                bill.Status = bill.PaidAmount >= bill.TotalAmount
                    ? StatusPostpaid.Paid
                    : StatusPostpaid.Partial;
            }
        }

        private async Task AddAuditLogAsync(
            long? userId,
            string action,
            string entityType,
            long? entityId,
            string description)
        {
            await _unitOfWork.AuditLogs.AddAsync(new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Description = description,
                IpAddress = "system",
                CreatedAt = DateTime.UtcNow
            });
        }

        private async Task MarkPaymentFailedIfPossibleAsync(string? paymentRef, string providerName, string reason)
        {
            if (string.IsNullOrWhiteSpace(paymentRef)) return;

            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var payment = await _unitOfWork.Payments.GetByPaymentRefAsync(paymentRef);
                if (payment is null) return;

                payment.Status = StatusPayment.Failed;
                foreach (var transactionEntity in payment.Transactions)
                {
                    transactionEntity.Status = StatusTransaction.Failed;
                }

                await AddAuditLogAsync(
                    null,
                    "PaymentSaga.FailedGateway",
                    "Payment",
                    payment.Id,
                    $"Ref={paymentRef}; Provider={providerName}; Reason={reason}");

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
                await InvalidatePaymentCachesAsync(paymentRef, payment.Transactions.FirstOrDefault()?.TransactionRef);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to mark payment {PaymentRef} as failed", paymentRef);
            }
        }

        private async Task InvalidatePaymentCachesAsync(string paymentRef, string? transactionRef)
        {
            if (_cacheService is null) return;

            await _cacheService.RemoveCacheResponseAsync(CacheKeyBuilder.Entity("payment", paymentRef));
            if (!string.IsNullOrWhiteSpace(transactionRef))
            {
                await _cacheService.RemoveCacheResponseAsync(CacheKeyBuilder.Entity("transaction", transactionRef));
            }

            await _cacheService.RemoveCacheResponseByGroupAsync(CacheGroups.Payments);
            await _cacheService.RemoveCacheResponseByGroupAsync(CacheGroups.Transactions);
        }

        private static PaymentResponse BuildPaymentResponse(
            Payment payment,
            Transaction transaction,
            Customer customer,
            decimal balanceBefore,
            decimal balanceAfter,
            decimal discountApplied,
            string? paymentUrl)
        {
            var response = PaymentResponse.FromEntity(payment, paymentUrl);
            response.CustomerId = customer.Id;
            response.CustomerType = customer.CustomerType;
            response.TransactionRef = transaction.TransactionRef;
            response.AccountBalanceBefore = balanceBefore;
            response.AccountBalanceAfter = balanceAfter;
            response.DiscountApplied = discountApplied;
            response.Message = payment.Status == StatusPayment.Completed
                ? "Payment transaction completed successfully."
                : "Payment transaction is pending gateway confirmation.";
            return response;
        }
    }
}
