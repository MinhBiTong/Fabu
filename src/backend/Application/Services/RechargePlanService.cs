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
    }
}