using Application.DTOs.Responses;
using Application.DTOs.Responses.PaymentResponse;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Payments.Queries;

public sealed class PaymentQueryHandlers :
    IRequestHandler<GetPaymentByRefQuery, ApiResponse<PaymentResponse>>
{
    private readonly IPaymentService _paymentService;

    public PaymentQueryHandlers(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<ApiResponse<PaymentResponse>> Handle(GetPaymentByRefQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var payment = await _paymentService.GetPaymentByRefAsync(request.PaymentRef);
            return ApiResponse<PaymentResponse>.Success(payment);
        }
        catch (AppException)
        {
            return ApiResponse<PaymentResponse>.Fail(404, "Payment not found.");
        }
    }
}
