using Application.DTOs.Requests.PostpaidRequest;
using Application.DTOs.Responses.PaymentResponse;
using Application.DTOs.Responses.PostpaidResponse;

namespace Application.Interfaces
{
    public interface IPostpaidBillService
    {
        Task<PostpaidBillResponse> CreateAsync(PostpaidCreateRequest request);
        Task<List<PostpaidBillResponse>> GetUnpaidBillsByCustomerAsync(long customerId);
        Task<PostpaidBillResponse> GetLatestBillAsync(long customerId);
        Task<List<PostpaidBillResponse>> GetOverdueBillsAsync();
        Task<PaymentResponse> PayBillAsync(long billId, PostpaidPaymentRequest request);
        Task UpdateBillStatusAfterPayment(long billId, decimal paidAmount);
    }
}
