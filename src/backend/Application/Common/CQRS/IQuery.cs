using MediatR;

namespace Application.Common.CQRS;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}

public interface ICachedQuery<TResponse> : IQuery<TResponse>
{
    string CacheKey { get; }
    TimeSpan? CacheDuration { get; }
    IReadOnlyCollection<string> CacheGroups { get; }
}
