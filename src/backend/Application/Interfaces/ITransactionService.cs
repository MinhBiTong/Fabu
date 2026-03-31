using Application.DTOs.Requests.PostpaidRequest;
using Application.DTOs.Requests.RechargePlanRequest;
using Application.DTOs.Requests.ServiceRequest;
using Application.DTOs.Requests.TransactionRequest;
using Application.DTOs.Responses.TransactionResponse;
using Domain.Abstractions;
using Domain.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITransactionService 
    {
        Task<TransactionResponse> CreateRechargeTransactionAsync(TransactionCreateRequest request);
        Task<TransactionResponse> CreateServiceActivationTransactionAsync(ServiceCreateRequest request);
        Task<TransactionResponse> CreateBillPaymentTransactionAsync(PostpaidCreateRequest request);
        Task<PagedResult<TransactionResponse>> GetTransactionsByCustomerAsync(long customerId, int page = 1, int pageSize = 10);
        Task<TransactionResponse> GetTransactionByRefAsync(string transactionRef);
        Task<decimal> GetTotalSpentByCustomerAsync(long customerId);
    }
}
