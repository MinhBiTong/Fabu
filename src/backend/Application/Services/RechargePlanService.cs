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

            // Sử dụng CommitAsync để kích hoạt UpdateAuditFields (CreatedDate, CreatedBy...)
            await _unitOfWork.CommitAsync();

            var result = _mapper.Map<RechargePlanResponse>(plan);
            return ApiResponse<RechargePlanResponse>.Success(result, "Created successfully.");
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int id, UpdateRechargePlanRequest request)
        {
            // 1. Kiểm tra ID
            if (id != request.Id) return ApiResponse<bool>.Fail(400, "ID mismatch.");

            // 2. Lấy dữ liệu hiện tại từ DB (EF bắt đầu Tracking)
            var plan = await _unitOfWork.RechargePlans.GetByIdAsync(id);
            if (plan == null) return ApiResponse<bool>.Fail(404, "Recharge plan not found.");

            // 3. Map dữ liệu mới đè lên Object cũ. 
            // EF Change Tracker sẽ đánh dấu Object này là 'Modified'
            _mapper.Map(request, plan);

            // 4. CHỖ THAY ĐỔI: Không gọi _unitOfWork.RechargePlans.UpdateAsync nữa.
            // Chỉ cần CommitAsync, logic tự động điền ModifiedDate/ModifiedBy trong UoW sẽ chạy.
            var result = await _unitOfWork.CommitAsync();

            return result > 0
                ? ApiResponse<bool>.Success(true, "Updated successfully.")
                : ApiResponse<bool>.Fail(500, "No changes were saved.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var plan = await _unitOfWork.RechargePlans.GetByIdAsync(id);
            if (plan == null) return ApiResponse<bool>.Fail(404, "Recharge plan not found.");

            // Tận dụng Soft Delete: Hàm UpdateAuditFields trong UoW sẽ tự chuyển 
            // lệnh Delete thành Update IsDeleted = true
            _unitOfWork.RechargePlans.Delete(plan);

            await _unitOfWork.CommitAsync();
            return ApiResponse<bool>.Success(true, "Deleted successfully.");
        }
    }
}