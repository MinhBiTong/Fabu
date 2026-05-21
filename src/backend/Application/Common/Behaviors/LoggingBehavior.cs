using Application.Common.CQRS;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
        var requestName = typeof(TRequest).Name;
        var requestKind = ResolveRequestKind(request);

        _logger.LogInformation(
            "Handling {RequestKind} {RequestName}. RequestId: {RequestId}",
            requestKind,
            requestName,
            requestId);

        try
        {
            var response = await next();
            _logger.LogInformation(
                "Handled {RequestKind} {RequestName}. RequestId: {RequestId}",
                requestKind,
                requestName,
                requestId);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed {RequestKind} {RequestName}. RequestId: {RequestId}",
                requestKind,
                requestName,
                requestId);
            throw;
        }
    }

    private static string ResolveRequestKind(TRequest request)
    {
        return request switch
        {
            ICommand<TResponse> => "Command",
            IQuery<TResponse> => "Query",
            _ => "Request"
        };
    }
}
