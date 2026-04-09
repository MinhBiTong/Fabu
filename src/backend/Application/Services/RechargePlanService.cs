using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RechargePlanService : IRechargePlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RechargePlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<RechargePlanResponse>>> GetAllAsync()
        {
            var plans = await _unitOfWork.RechargePlans.GetAllAsync();
            var result = _mapper.Map<List<RechargePlanResponse>>(plans);
            return ApiResponse<List<RechargePlanResponse>>.Success(result);
        }

        public async Task<ApiResponse<RechargePlanResponse>> GetByIdAsync(int id)
        {
            var plan = await _unitOfWork.RechargePlans.GetByIdAsync(id);
            if (plan == null)
                return ApiResponse<RechargePlanResponse>.Fail(404, "Recharge plan not found.");

            var result = _mapper.Map<RechargePlanResponse>(plan);
            return ApiResponse<RechargePlanResponse>.Success(result);
        }

        public async Task<ApiResponse<RechargePlanResponse>> CreateAsync(CreateRechargePlanRequest request)
        {
            var plan = _mapper.Map<RechargePlan>(request);
            await _unitOfWork.RechargePlans.AddAsync(plan);
            await _unitOfWork.SaveChangesAsync();
            var result = _mapper.Map<RechargePlanResponse>(plan);
            return ApiResponse<RechargePlanResponse>.Success(result, "Created successfully.");
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int id, UpdateRechargePlanRequest request)
        {
            if (id != request.Id) return ApiResponse<bool>.Fail(400, "ID mismatch.");
            var plan = await _unitOfWork.RechargePlans.GetByIdAsync(id);
            if (plan == null) return ApiResponse<bool>.Fail(404, "Recharge plan not found.");
            _mapper.Map(request, plan);
            await _unitOfWork.RechargePlans.UpdateAsync(plan);
            return ApiResponse<bool>.Success(true, "Updated successfully.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var plan = await _unitOfWork.RechargePlans.GetByIdAsync(id);
            if (plan == null) return ApiResponse<bool>.Fail(404, "Recharge plan not found.");
            _unitOfWork.RechargePlans.Delete(plan);
            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "Deleted successfully.");
        }

        public async Task<ApiResponse<List<RechargePlanResponse>>> GetActivePlansAsync()
        {
            var plans = await _unitOfWork.RechargePlans.GetActivePlansAsync();
            return ApiResponse<List<RechargePlanResponse>>.Success(_mapper.Map<List<RechargePlanResponse>>(plans));
        }

        public async Task<ApiResponse<RechargePlanResponse>> GetByAmountAsync(decimal amount)
        {
            var plan = await _unitOfWork.RechargePlans.GetByAmountAsync(amount);
            if (plan == null) return ApiResponse<RechargePlanResponse>.Fail(404, "Không tìm thấy.");
            return ApiResponse<RechargePlanResponse>.Success(_mapper.Map<RechargePlanResponse>(plan));
        }

        public async Task<ApiResponse<List<RechargePlanResponse>>> GetPlansByPriceRangeAsync(decimal min, decimal max)
        {
            var plans = await _unitOfWork.RechargePlans.GetPlansByPriceRangeAsync(min, max);
            return ApiResponse<List<RechargePlanResponse>>.Success(_mapper.Map<List<RechargePlanResponse>>(plans));
        }

        public async Task<ApiResponse<List<RechargePlanResponse>>> GetPopularPlansAsync(int top)
        {
            var plans = await _unitOfWork.RechargePlans.GetPopularPlansAsync(top);
            return ApiResponse<List<RechargePlanResponse>>.Success(_mapper.Map<List<RechargePlanResponse>>(plans));
        }

        public async Task<ApiResponse<List<RechargePlanResponse>>> GetPlansByProviderAsync(string provider)
        {
            var plans = await _unitOfWork.RechargePlans.GetPlansByProviderAsync(provider);
            return ApiResponse<List<RechargePlanResponse>>.Success(_mapper.Map<List<RechargePlanResponse>>(plans));
        }

        public async Task<ApiResponse<bool>> IsPlanActiveAsync(long planId)
        {
            var isActive = await _unitOfWork.RechargePlans.IsPlanActiveAsync(planId);
            return ApiResponse<bool>.Success(isActive);
        }
    }
}