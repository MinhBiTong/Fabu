using Application.DTOs;
using Application.DTOs.Requests;
using Application.DTOs.Requests.CustomerRequest;
using Application.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICustomerService
    {
        Task<ApiResponse<List<CustomerResponse>>> GetAllAsync();
        Task<ApiResponse<CustomerResponse>> GetByIdAsync(long id);
        Task<ApiResponse<CustomerResponse>> CreateAsync(CustomerCreateRequest request);
        Task<ApiResponse<bool>> UpdateAsync(long id, CustomerUpdateRequest request);
        Task<ApiResponse<bool>> DeleteAsync(long id);
    }
}