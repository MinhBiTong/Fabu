using Application.DTOs.Requests.UserRequest;
using Application.DTOs.Responses.UserResponse;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);
        Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<UserResponse> DeleteUserAsync(int id);
        Task<UserResponse> GetCurrentUserAsync();
        Task<List<UserResponse>> GetAllUsersPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
