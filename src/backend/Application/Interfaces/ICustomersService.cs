using Application.DTOs;
using Application.DTOs.Requests;
using Application.DTOs.Requests.CustomerRequest;
using Application.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICustomersService
    {
        Task<ApiResponse<List<CustomerResponse>>> GetAllAsync();
        Task<ApiResponse<CustomerResponse>> GetByIdAsync(long id);
        Task<ApiResponse<CustomerResponse>> CreateAsync(CustomerCreateRequest request);
    }
}