using Application.DTOs.Requests.PostpaidRequest;
using Application.DTOs.Requests.RechargePlanRequest;
using Application.DTOs.Requests.ServiceRequest;
using Application.DTOs.Requests.TransactionRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.TransactionResponse;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionService> _logger;
        private readonly IPaymentService _paymentService;
        private readonly ICouponService _couponService;
        public TransactionService(IUnitOfWork unitOfWork, ILogger<TransactionService> logger, IPaymentService paymentService, ICouponService couponService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _paymentService = paymentService;
            _couponService = couponService;
        }

        public Task<ApiResponse<TransactionResponse>> CreateAsync(TransactionCreateRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<TransactionResponse> CreateBillPaymentTransactionAsync(PostpaidCreateRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<TransactionResponse> CreateRechargeTransactionAsync(TransactionCreateRequest request)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                //kiem tra customer
                var customer = await _unitOfWork.Customers.GetByMobileNumberAsync(request.Customer.MobileNumber);
                if (customer == null)
                    throw new AppException(ErrorCode.CUSTOMER_NOT_FOUND);

                //tao transaction
                var transactionEntity = new Domain.Entities.Transaction
                {
                    CustomerId = customer.Id,
                    TransactionType = "Recharge",
                    Amount = request.Amount,
                    TransactionRef = $"RECH_{Guid.NewGuid():N}".ToUpper(),
                    PaymentMethod = request.PaymentMethod,
                    Status = StatusTransaction.Pending
                };

                // 3. Áp dụng Coupon (logic quan trọng)
                decimal finalAmount = request.Amount;
                CouponUsage? couponUsage = null;

                if (!string.IsNullOrWhiteSpace(request.CouponCode))
                {
                    var couponResult = await _couponService.ApplyCouponAsync(
                        request.CouponCode,
                        customer.Id,
                        request.Amount,
                        "Recharge");

                    if (couponResult.IsSuccess)
                    {
                        finalAmount = couponResult.FinalAmount;
                        couponUsage = couponResult.CouponUsage;

                        _logger.LogInformation("Applied coupon {Code} for transaction {Ref}. Original: {Original}, Final: {Final}",
                            request.CouponCode, transactionEntity.TransactionRef, request.Amount, finalAmount);
                    }
                    else
                    {
                        _logger.LogWarning("Coupon {Code} application failed: {Reason}", request.CouponCode, couponResult.Message);
                    }
                }

                transactionEntity.Amount = finalAmount;

                // 4. Lưu Transaction
                await _unitOfWork.Transactions.AddAsync(transactionEntity);

                // 5. Nếu có coupon usage thì lưu
                if (couponUsage != null)
                {
                    couponUsage.TransactionId = transactionEntity.Id;
                    await _unitOfWork.CouponUsages.AddAsync(couponUsage);
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Recharge transaction created successfully. Ref: {Ref}, Amount: {Amount}",
                    transactionEntity.TransactionRef, finalAmount);

                return TransactionResponse.FromEntity(transactionEntity);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public Task<TransactionResponse> CreateServiceActivationTransactionAsync(ServiceCreateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<TransactionResponse>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<TransactionResponse>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalSpentByCustomerAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public async Task<TransactionResponse> GetTransactionByRefAsync(string transactionRef)
        {
            var transaction = await _unitOfWork.Transactions.GetByTransactionRefAsync(transactionRef);
            if (transaction == null)
                throw new AppException(ErrorCode.TRANSACTION_NOT_FOUND);

            return TransactionResponse.FromEntity(transaction);
        }

        public async Task<PagedResult<TransactionResponse>> GetTransactionsByCustomerAsync(long customerId, int page = 1, int pageSize = 10)
        {
            DateTime? from = DateTime.UtcNow; 
            DateTime? to = DateTime.UtcNow.AddMonths(1);

            var transactions = await _unitOfWork.Transactions.GetTransactionsByCustomerAsync(customerId, from, to);

            var total = transactions.Count;
            var items = transactions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(TransactionResponse.FromEntity)
                .ToList();

            return new PagedResult<TransactionResponse>(items, total, pageSize);
        }
    }
}
