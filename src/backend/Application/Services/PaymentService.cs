using Application.DTOs.Requests.PaymentRequest;
using Application.DTOs.Responses.PaymentResponse;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Exceptions;
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

        public PaymentService(IUnitOfWork unitOfWork, ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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
                .Select(PaymentResponse.FromEntity)
                .ToList();

            return new PagedResult<PaymentResponse>(items, total, pageSize);
        }

        public async Task<decimal> GetTotalPaidAmountAsync(long customerId)
        {
            return await _unitOfWork.Payments.GetTotalPaidAmountAsync(customerId);
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentCreateRequest request)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Create Payment entity and save to database
                var payment = new Domain.Entities.Payment
                {
                    BillId = request.BillId,
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod,
                    PaymentRef = $"PAY_{Guid.NewGuid():N}".ToUpper(),
                    Status = Domain.ValueObjects.StatusPayment.Pending,
                    PaymentDate = DateTime.UtcNow
                };
                // 2. Create Transaction entity linked to the Payment and save to database
                if (request.TransactionId.HasValue)
                {
                    var trans = await _unitOfWork.Transactions.GetByIdAsync(request.TransactionId.Value);
                    if (trans != null)
                        payment.Transactions.Add(trans);
                }

                // 3. Commit transaction
                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Payment processed successfully with ref {PaymentRef}", payment.PaymentRef);
                return null;
                //return new PaymentResponse.FromEntity(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment");
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
