using MediatR;

namespace Application.Common.CQRS;

public interface ICommand<TResponse> : IRequest<TResponse>
{
}

public interface ITransactionalCommand<TResponse> : ICommand<TResponse>
{
}
