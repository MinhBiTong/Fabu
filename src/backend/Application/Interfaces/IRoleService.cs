using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Requests.RoleRequest;
using Application.DTOs.Responses.RoleResponse;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleResponse> CreateRoleAsync(RoleCreateRequest request);
        Task DeleteRoleAsync(int id);
        Task<RoleResponse> UpdateRoleAsync(int id, RoleUpdateRequest request);
        Task GetAllPoleAsync();

    }
}
