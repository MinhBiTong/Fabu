using Application.DTOs;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRechargePlanService
    {
        Task<ApiResponse<List<RechargePlanResponse>>> GetAllAsync();
        Task<ApiResponse<RechargePlanResponse>> GetByIdAsync(int id);
        Task<ApiResponse<RechargePlanResponse>> CreateAsync(CreateRechargePlanRequest request);
        Task<ApiResponse<bool>> UpdateAsync(int id, UpdateRechargePlanRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}