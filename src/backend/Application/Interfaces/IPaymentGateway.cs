using Application.DTOs.Requests.PaymentRequest;
using Domain.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPaymentGateway
    {
        Task<string> CreatePaymentUrlAsync(PaymentCreateRequest request);
        Task<PaymentCallbackResult> HandleCallbackAsync(Dictionary<string, string> callbackData);
        string GetProviderName();
    }
}
