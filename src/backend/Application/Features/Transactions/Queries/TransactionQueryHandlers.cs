using Application.DTOs.Responses;
using Application.DTOs.Responses.TransactionResponse;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Transactions.Queries;

public sealed class TransactionQueryHandlers :
    IRequestHandler<GetTransactionByRefQuery, ApiResponse<TransactionResponse>>,
    IRequestHandler<GetTransactionsByCustomerQuery, ApiResponse<PagedResult<TransactionResponse>>>
{
    private readonly ITransactionService _transactionService;

    public TransactionQueryHandlers(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    public async Task<ApiResponse<TransactionResponse>> Handle(
        GetTransactionByRefQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByRefAsync(request.TransactionRef);
            return ApiResponse<TransactionResponse>.Success(transaction);
        }
        catch (AppException)
        {
            return ApiResponse<TransactionResponse>.Fail(404, "Transaction not found.");
        }
    }

    public async Task<ApiResponse<PagedResult<TransactionResponse>>> Handle(
        GetTransactionsByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsByCustomerAsync(
            request.CustomerId,
            request.Page,
            request.PageSize);

        return ApiResponse<PagedResult<TransactionResponse>>.Success(transactions);
    }
}
