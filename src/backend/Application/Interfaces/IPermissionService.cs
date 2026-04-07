using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Requests.PermissionRequest;
using Application.DTOs.Requests.RoleRequest;
using Application.DTOs.Responses.PermissionResponse;
using Application.DTOs.Responses.RoleResponse;

namespace Application.Interfaces
{
    public interface IPermissionService
    {
        Task<PermissionResponse> CreatePermissionAsync(PermissionCreateRequest request);
        Task DeletePermissionAsync(int id);
        Task<PermissionUpdateRequest> UpdatePermissionAsync(long id, PermissionUpdateRequest request);
        Task<List<PermissionResponse>> GetAllPermissionAsync();
        Task<PermissionResponse> GetPermissionByNameAsync(string name);
    }
}
