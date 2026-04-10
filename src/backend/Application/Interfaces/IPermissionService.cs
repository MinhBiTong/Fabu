using Application.DTOs.Requests.PermissionRequest;
using Application.DTOs.Responses.PermissionResponse;

namespace Application.Interfaces
{
    public interface IPermissionService
    {
        Task<PermissionResponse> CreatePermissionAsync(PermissionCreateRequest request);
        Task DeletePermissionAsync(int id);
        Task<PermissionResponse> UpdatePermissionAsync(int id, PermissionUpdateRequest request);
        Task<List<PermissionResponse>> GetAllPermissionAsync();
        Task<PermissionResponse> GetPermissionByNameAsync(string name);
    }
}
