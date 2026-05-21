using Application.Common.Caching;
using Application.Common.CQRS;
using Application.DTOs.Responses;
using Application.DTOs.Responses.TransactionResponse;
using Domain.Abstractions;

namespace Application.Features.Transactions.Queries;

public sealed record GetTransactionByRefQuery(string TransactionRef) : ICachedQuery<ApiResponse<TransactionResponse>>
{
    public string CacheKey => CacheKeyBuilder.Entity("transaction", TransactionRef);
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheGroups => new[] { Application.Common.Caching.CacheGroups.Transactions };
}

public sealed record GetTransactionsByCustomerQuery(long CustomerId, int Page, int PageSize)
    : ICachedQuery<ApiResponse<PagedResult<TransactionResponse>>>
{
    public string CacheKey => CacheKeyBuilder.Page("transaction", Page, PageSize, new { CustomerId });
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheGroups => new[] { Application.Common.Caching.CacheGroups.Transactions };
}
