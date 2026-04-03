using Application.DTOs.Requests.FeedbackRequest;
using Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFeedbackService
    {
        Task<ApiResponse<List<FeedbackResponse>>> GetAllAsync();
        Task<ApiResponse<FeedbackResponse>> GetByIdAsync(long id);
        Task<ApiResponse<FeedbackResponse>> CreateAsync(FeedbackCreateRequest request);
        Task<ApiResponse<bool>> UpdateAsync(long id, FeedbackUpdateRequest request);
        Task<ApiResponse<bool>> DeleteAsync(long id);
    }
}
