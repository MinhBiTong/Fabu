using Application.DTOs.Requests.PaymentRequest;
using Application.DTOs.Requests.TransactionRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.PaymentResponse;
using Application.DTOs.Responses.TransactionResponse;
using Domain.Abstractions;
using Domain.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<List<PaymentResponse>>> GetAllAsync();
        Task<ApiResponse<PaymentResponse>> GetByIdAsync(int id);
        Task<ApiResponse<bool>> UpdateAsync(int id, PaymentUpdateRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<PaymentResponse> ProcessPaymentAsync(PaymentCreateRequest request);
        Task<PaymentResponse> GetPaymentByRefAsync(string paymentRef);
        Task<PagedResult<PaymentResponse>> GetPaymentsByCustomerAsync(long customerId, int page = 1, int pageSize = 10);
        Task<decimal> GetTotalPaidAmountAsync(long customerId);
        Task<PaymentResponse> CreatePaymentAsync(PaymentCreateRequest request);
        Task<PaymentCallbackResult> HandleVNPayCallbackAsync(Dictionary<string, string> callbackData);
    }
}
