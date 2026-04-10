//using System;
using Application.DTOs;
using Application.DTOs.Requests.ServiceRequest;
using Application.DTOs.Response;
using Application.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IServiceService
    {
        Task<ApiResponse<List<ServiceResponse>>> GetAllAsync();
        Task<ApiResponse<ServiceResponse>> GetByIdAsync(long id);
        Task<ApiResponse<ServiceResponse>> CreateAsync(ServiceCreateRequest request);
        Task<ApiResponse<List<ServiceResponse>>> GetActiveServicesByCategoryAsync(string category);
        Task<ApiResponse<List<ServiceResponse>>> GetPopularServicesAsync(int top);
        Task<ApiResponse<ServiceResponse>> GetByCodeAsync(string code);
        Task<ApiResponse<bool>> IsServiceActiveAsync(long serviceId);
        Task<ApiResponse<List<ServiceResponse>>> SearchServicesAsync(string keyword);
    }
}
