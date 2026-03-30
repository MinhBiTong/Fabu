using Application.DTOs.Requests.PaymentRequest;
using Application.DTOs.Responses.PaymentResponse;
using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentCreateRequest request);
        Task<PaymentResponse> GetPaymentByRefAsync(string paymentRef);
        Task<PagedResult<PaymentResponse>> GetPaymentsByCustomerAsync(long customerId, int page = 1, int pageSize = 10);
        Task<decimal> GetTotalPaidAmountAsync(long customerId);
    }
}
