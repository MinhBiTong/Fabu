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
        Task<ApiResponse<List<RechargePlanResponse>>> GetActivePlansAsync();
        Task<ApiResponse<RechargePlanResponse>> GetByAmountAsync(decimal amount);
        Task<ApiResponse<List<RechargePlanResponse>>> GetPlansByPriceRangeAsync(decimal min, decimal max);
        Task<ApiResponse<List<RechargePlanResponse>>> GetPopularPlansAsync(int top);
        Task<ApiResponse<List<RechargePlanResponse>>> GetPlansByProviderAsync(string provider);
        Task<ApiResponse<bool>> IsPlanActiveAsync(long planId);
    }
}