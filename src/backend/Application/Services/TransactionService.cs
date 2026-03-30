using Application.DTOs.Requests.PostpaidRequest;
using Application.DTOs.Requests.RechargePlanRequest;
using Application.DTOs.Requests.ServiceRequest;
using Application.DTOs.Requests.TransactionRequest;
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
        public TransactionService(IUnitOfWork unitOfWork, ILogger<TransactionService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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
                var transactionEntity = new Transaction
                {
                    CustomerId = customer.Id,
                    TransactionType = "Recharge",
                    Amount = request.Amount,
                    TransactionRef = $"RECH_{Guid.NewGuid():N}".ToUpper(),
                    Status = StatusTransaction.Pending,
                    PaymentMethod = request.PaymentMethod,
                };

                await _unitOfWork.Transactions.AddAsync(transactionEntity);

                //ap dung coupon
                if (!string.IsNullOrEmpty(request.CouponUsages.ToString()))
                {
                    //logic ap coupon
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Recharge transaction created: {Ref}", transactionEntity.TransactionRef);

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

        public Task<decimal> GetTotalSpentByCustomerAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionResponse> GetTransactionByRefAsync(string transactionRef)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<TransactionResponse>> GetTransactionsByCustomerAsync(long customerId, int page = 1, int pageSize = 10)
        {
            // 1. Gọi Repository để lấy dữ liệu đã phân trang từ SQL
            var skip = (page - 1) * pageSize;
            var transactions = await _unitOfWork.Transactions.GetAllPagedAsync(skip, pageSize);

            // 2. Lấy tổng số dòng để hiển thị ở UI
            var total = await _unitOfWork.Transactions.CountAsync(t => t.CustomerId == customerId);

            var items = transactions
                .Select(TransactionResponse.FromEntity)
                .ToList();

            return new PagedResult<TransactionResponse>(items, total, pageSize);
        }
    }
}
