using Application.DTOs.Requests.RoleRequest;
using Application.DTOs.Responses.RoleResponse;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleResponse> CreateRoleAsync(RoleCreateRequest request);
        Task DeleteRoleAsync(long id);
        Task<RoleResponse> UpdateRoleAsync(long id, RoleUpdateRequest request);
        Task<List<RoleResponse>> GetAllRoleAsync();
        Task<RoleResponse> GetRoleByNameAsync(string name);
    }
}
