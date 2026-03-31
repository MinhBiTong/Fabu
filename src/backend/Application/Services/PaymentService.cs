using Application.DTOs.Requests.PaymentRequest;
using Application.DTOs.Responses.PaymentResponse;
using Application.Interfaces;
using Domain.Abstractions;
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

        public Task<PaymentResponse> GetPaymentByRefAsync(string paymentRef)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<PaymentResponse>> GetPaymentsByCustomerAsync(long customerId, int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalPaidAmountAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentResponse> ProcessPaymentAsync(PaymentCreateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
