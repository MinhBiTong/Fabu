using MediatR;

namespace Application.Common.Events;

public sealed record CacheInvalidationEvent(
    IReadOnlyCollection<string> CacheKeys,
    IReadOnlyCollection<string> CacheGroups,
    string Reason) : INotification;
