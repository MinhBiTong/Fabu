using Application.DTOs.Requests;
using Application.DTOs.Requests.FeedbackRequest;
using Application.DTOs.Responses;  
using Application.Interfaces;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;             
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<FeedbackResponse>>> GetAllAsync()
        {
            var feedbacks = await _unitOfWork.Feedbacks.GetAllAsync();
            var result = _mapper.Map<List<FeedbackResponse>>(feedbacks);
            return ApiResponse<List<FeedbackResponse>>.Success(result);
        }

        public async Task<ApiResponse<FeedbackResponse>> GetByIdAsync(long id)
        {
            var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(id);
            if (feedback == null) return ApiResponse<FeedbackResponse>.Fail(404, "Feedback not found.");
            var result = _mapper.Map<FeedbackResponse>(feedback);
            return ApiResponse<FeedbackResponse>.Success(result);
        }

        public async Task<ApiResponse<FeedbackResponse>> CreateAsync(FeedbackCreateRequest request)
        {
            var feedback = _mapper.Map<Feedback>(request);
            await _unitOfWork.Feedbacks.AddAsync(feedback);
            await _unitOfWork.SaveChangesAsync();
            var result = _mapper.Map<FeedbackResponse>(feedback);
            return ApiResponse<FeedbackResponse>.Success(result, "Feedback created.");
        }

        public async Task<ApiResponse<bool>> UpdateAsync(long id, FeedbackUpdateRequest request)
        {
            var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(id);
            if (feedback == null) return ApiResponse<bool>.Fail(404, "Feedback not found.");
            _mapper.Map(request, feedback);
            _unitOfWork.Feedbacks.Update(feedback); 
            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "Feedback updated.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(id);
            if (feedback == null) return ApiResponse<bool>.Fail(404, "Feedback not found.");
            _unitOfWork.Feedbacks.Delete(feedback);
            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "Feedback deleted.");
        }
    }
}