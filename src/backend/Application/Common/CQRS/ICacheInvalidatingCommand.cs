using Application.Common.Events;

namespace Application.Common.CQRS;

public interface ICacheInvalidatingCommand<TResponse> : ITransactionalCommand<TResponse>
{
    CacheInvalidationEvent BuildCacheInvalidationEvent(TResponse response);
}
