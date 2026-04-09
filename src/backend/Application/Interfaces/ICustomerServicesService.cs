using Application.DTOs;
using Application.DTOs.Requests.CustomerServiceRequest;
using Application.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICustomerServicesService
    {
        Task<ApiResponse<List<CustomerServiceResponse>>> GetAllAsync();
        Task<ApiResponse<CustomerServiceResponse>> GetByIdAsync(long id);
        Task<ApiResponse<CustomerServiceResponse>> CreateAsync(CustomerServiceCreateRequest request);
    }
}