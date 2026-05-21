using Application.Common.Caching;
using Application.Common.CQRS;
using Application.DTOs.Responses;
using Application.DTOs.Responses.PaymentResponse;

namespace Application.Features.Payments.Queries;

public sealed record GetPaymentByRefQuery(string PaymentRef) : ICachedQuery<ApiResponse<PaymentResponse>>
{
    public string CacheKey => CacheKeyBuilder.Entity("payment", PaymentRef);
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheGroups => new[] { Application.Common.Caching.CacheGroups.Payments };
}
