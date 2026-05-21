using Application.DTOs.Requests.PaymentRequest;
using Application.DTOs.Responses.PaymentResponse;
using Domain.Options;

namespace Application.Interfaces;

public interface IPaymentTransactionSagaService
{
    Task<PaymentResponse> StartAsync(PaymentCreateRequest request, CancellationToken cancellationToken = default);
    Task<PaymentCallbackResult> CompleteAsync(string providerName, Dictionary<string, string> callbackData, CancellationToken cancellationToken = default);
}
