using Application.DTOs.Requests.PaymentRequest;
using Application.DTOs.Requests.PostpaidRequest;
using Application.DTOs.Requests.ServiceRequest;
using Application.DTOs.Requests.TransactionRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.TransactionResponse;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Exceptions;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionService> _logger;
        private readonly IPaymentTransactionSagaService _paymentTransactionSaga;

        public TransactionService(
            IUnitOfWork unitOfWork,
            ILogger<TransactionService> logger,
            IPaymentTransactionSagaService paymentTransactionSaga)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _paymentTransactionSaga = paymentTransactionSaga;
        }

        public async Task<ApiResponse<TransactionResponse>> CreateAsync(TransactionCreateRequest request)
        {
            var response = await CreateRechargeTransactionAsync(request);
            return ApiResponse<TransactionResponse>.Success(response, "Transaction created successfully.");
        }

        public async Task<TransactionResponse> CreateRechargeTransactionAsync(TransactionCreateRequest request)
        {
            var payment = await _paymentTransactionSaga.StartAsync(new PaymentCreateRequest
            {
                CustomerId = request.CustomerId,
                MobileNumber = request.MobileNumber,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                CouponCode = request.CouponCode,
                TransactionType = string.IsNullOrWhiteSpace(request.TransactionType) ? "Recharge" : request.TransactionType,
                UseAccountBalance = request.PaymentMethod == PaymentMethod.Cash,
                PaymentRef = string.IsNullOrWhiteSpace(request.TransactionRef) ? null : request.TransactionRef
            });

            return new TransactionResponse
            {
                CustomerId = payment.CustomerId,
                PaymentId = payment.PaymentId,
                TransactionRef = payment.TransactionRef ?? string.Empty,
                TransactionType = string.IsNullOrWhiteSpace(request.TransactionType) ? "Recharge" : request.TransactionType,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                CompletedAt = payment.Status == StatusPayment.Completed.ToString() ? DateTime.UtcNow : null
            };
        }

        public Task<TransactionResponse> CreateServiceActivationTransactionAsync(ServiceCreateRequest request)
        {
            throw new AppException(
                ErrorCode.INVALID_KEY,
                "Use PaymentController /api/v{version}/Payment/package with CustomerId and ServiceId to activate a package transaction.");
        }

        public Task<TransactionResponse> CreateBillPaymentTransactionAsync(PostpaidCreateRequest request)
        {
            throw new AppException(
                ErrorCode.INVALID_KEY,
                "Use PostpaidController /api/v{version}/Postpaid/bills/{billId}/pay to pay a postpaid bill.");
        }

        public async Task<ApiResponse<List<TransactionResponse>>> GetAllAsync()
        {
            var transactions = await _unitOfWork.Transactions.GetAllAsync();
            var response = transactions
                .OrderByDescending(transaction => transaction.CreatedDate)
                .Select(TransactionResponse.FromEntity)
                .ToList();

            return ApiResponse<List<TransactionResponse>>.Success(response);
        }

        public async Task<ApiResponse<TransactionResponse>> GetByIdAsync(int id)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(id);
            if (transaction is null)
                return ApiResponse<TransactionResponse>.Fail(404, "Transaction not found.");

            return ApiResponse<TransactionResponse>.Success(TransactionResponse.FromEntity(transaction));
        }

        public async Task<decimal> GetTotalSpentByCustomerAsync(long customerId)
        {
            return await _unitOfWork.Transactions.GetTotalAmountByCustomerAsync(customerId);
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
            var transactions = await _unitOfWork.Transactions.GetTransactionsByCustomerAsync(customerId, null, null);

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
