using Application.DTOs.Requests.PaymentRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.PaymentResponse;
using Application.Interfaces;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Options;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IPaymentGateway> _paymentGateways;

        public PaymentService(IUnitOfWork unitOfWork, ILogger<PaymentService> logger, IMapper mapper, IEnumerable<IPaymentGateway> paymentGateways)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _paymentGateways = paymentGateways;
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentCreateRequest request)
        {
            // 1. Khởi tạo Transaction để đảm bảo tính toàn vẹn dữ liệu
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 2. Tìm Provider (VNPay/PayPal...) phù hợp
                var gateway = _paymentGateways.FirstOrDefault(g => g.GetProviderName() == request.PaymentMethod.ToString());
                if (gateway == null)
                    throw new AppException(ErrorCode.PAYMENT_PROVIDER_NOT_SUPPORTED);

                // 3. Tạo Ref duy nhất cho giao dịch này (Cực kỳ quan trọng để đối soát sau này)
                var uniqueRef = $"PAY_{Guid.NewGuid():N}".ToUpper();
                request.TransactionRef = uniqueRef;

                // 4. Lấy URL từ Gateway (VNPay/PayPal sẽ dùng uniqueRef này để báo kết quả về)
                var paymentUrl = await gateway.CreatePaymentUrlAsync(request);

                // 5. Tạo Entity Payment ở trạng thái Pending
                var payment = new Domain.Entities.Payment
                {
                    BillId = request.BillId,
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod,
                    PaymentRef = uniqueRef,
                    Status = Domain.ValueObjects.StatusPayment.Pending,
                    PaymentDate = DateTime.UtcNow
                };

                // 6. Lưu vào DB thông qua Unit of Work
                await _unitOfWork.Payments.AddAsync(payment);

                // Sử dụng CommitAsync của UoW (hàm này đã có UpdateAuditFields em viết)
                await _unitOfWork.CommitAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Payment request created for Bill {BillId} with Ref {Ref}", request.BillId, uniqueRef);

                // 7. Trả về Response kèm URL để Frontend chuyển hướng người dùng
                return PaymentResponse.FromEntity(payment, paymentUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment request");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<PaymentResponse>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<PaymentResponse>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentResponse> GetPaymentByRefAsync(string paymentRef)
        {
            var payment = await _unitOfWork.Payments.GetByPaymentRefAsync(paymentRef);
            if (payment == null)
                throw new AppException(ErrorCode.PAYMENT_NOT_FOUND);

            return PaymentResponse.FromEntity(payment);
        }

        public async Task<PagedResult<PaymentResponse>> GetPaymentsByCustomerAsync(long customerId, int page = 1, int pageSize = 10)
        {
            var payments = await _unitOfWork.Payments.GetSuccessfulPaymentsAsync(customerId);

            var total = payments.Count;
            var items = payments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(payment => PaymentResponse.FromEntity(payment)) 
                .ToList();

            return new PagedResult<PaymentResponse>(items, total, pageSize);
        }

        public async Task<decimal> GetTotalPaidAmountAsync(long customerId)
        {
            return await _unitOfWork.Payments.GetTotalPaidAmountAsync(customerId);
        }

        public Task<ApiResponse<bool>> UpdateAsync(int id, PaymentUpdateRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentResponse> CreatePaymentAsync(PaymentCreateRequest request)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Validate request
                if (request.Amount <= 0)
                    throw new AppException(ErrorCode.INVALID_AMOUNT, "The amount not valid");

                // 2. Tạo Payment entity
                var payment = new Payment
                {
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod,
                    PaymentRef = $"PAY_{Guid.NewGuid():N}".ToUpper(),
                    BillId = request.BillId,
                    Status = StatusPayment.Pending
                };

                // Liên kết với Transaction nếu có
                //if (request.TransactionId.HasValue)
                //{
                //    var trans = await _unitOfWork.Transactions.GetByIdAsync(request.TransactionId.Value);
                //    if (trans != null)
                //    {
                //        payment.Transactions.Add(trans);
                //        payment.TransactionId = trans.Id;
                //    }
                //}

                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                // 3. Tạo URL thanh toán VNPay
                var paymentRequest = new PaymentCreateRequest
                {
                    Amount = request.Amount,
                    TransactionRef = payment.PaymentRef,
                    OrderInfo = request.OrderInfo ?? $"Payment the order {payment.PaymentRef}",
                    IpAddress = request.IpAddress ?? "127.0.0.1"
                };

                var gateway = _paymentGateways.FirstOrDefault(g => g.GetProviderName() == request.PaymentMethod.ToString());
                if (gateway == null)
                    throw new AppException(ErrorCode.PAYMENT_PROVIDER_NOT_SUPPORTED);

                var paymentUrl = await gateway.CreatePaymentUrlAsync(paymentRequest);

                await transaction.CommitAsync();

                _logger.LogInformation("Created payment {PaymentRef} with VNPay URL", payment.PaymentRef);

                return PaymentResponse.FromEntity(payment, paymentUrl);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to create payment");
                throw;
            }
        }

        public async Task<PaymentCallbackResult> HandleVNPayCallbackAsync(Dictionary<string, string> callbackData)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Find the appropriate payment gateway based on the callback data or provider name
                var gateway = _paymentGateways.FirstOrDefault(g => g.GetProviderName() == "VNPay");
                if (gateway == null)
                {
                    _logger.LogWarning("No payment gateway found for VNPay");
                    return PaymentCallbackResult.Failed("Payment gateway not found");
                }

                var result = await gateway.HandleCallbackAsync(callbackData);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("VNPay callback failed: {Message}", result.Message);
                    return PaymentCallbackResult.Failed(result.Message);
                }

                // Tìm Payment theo PaymentRef
                var payment = await _unitOfWork.Payments.GetByPaymentRefAsync(result.TransactionRef);
                if (payment == null)
                    return PaymentCallbackResult.Failed("This transaction can not found");

                // Cập nhật trạng thái Payment
                payment.Status = StatusPayment.Completed;
                payment.PaymentDate = DateTime.UtcNow;

                // Cập nhật Transaction liên quan (nếu có)
                foreach (var trans in payment.Transactions)
                {
                    trans.Status = StatusTransaction.Success;
                    trans.CompletedAt = DateTime.UtcNow;
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Payment {PaymentRef} completed successfully via VNPay", payment.PaymentRef);

                return PaymentCallbackResult.Success(payment.PaymentRef, "VNPay", new Dictionary<string, string> { { "Message", "The payment is successfully" } });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error processing VNPay callback");
                return PaymentCallbackResult.Failed("Failed handle callback");
            }
        }
    }
}
