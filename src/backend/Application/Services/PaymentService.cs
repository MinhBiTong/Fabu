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
        private readonly IPaymentTransactionSagaService _paymentTransactionSaga;

        public PaymentService(
            IUnitOfWork unitOfWork,
            ILogger<PaymentService> logger,
            IMapper mapper,
            IEnumerable<IPaymentGateway> paymentGateways,
            IPaymentTransactionSagaService paymentTransactionSaga)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _paymentGateways = paymentGateways;
            _paymentTransactionSaga = paymentTransactionSaga;
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentCreateRequest request)
        {
            return await _paymentTransactionSaga.StartAsync(request);
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
            return await _paymentTransactionSaga.StartAsync(request);
        }

        public async Task<PaymentCallbackResult> HandlePaymentCallbackAsync(string providerName, Dictionary<string, string> callbackData)
        {
            return await _paymentTransactionSaga.CompleteAsync(providerName, callbackData);
        }

        public async Task<PaymentCallbackResult> HandleVNPayCallbackAsync(Dictionary<string, string> callbackData)
        {
            return await HandlePaymentCallbackAsync("VNPay", callbackData);
        }
    }
}
