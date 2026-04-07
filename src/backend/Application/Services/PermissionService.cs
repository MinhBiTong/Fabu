using Application.DTOs.Requests.PermissionRequest;
using Application.DTOs.Responses.PermissionResponse;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PermissionService : IPermissionService
    {
        public Task<PermissionResponse> CreatePermissionAsync(PermissionCreateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task DeletePermissionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PermissionResponse>> GetAllPermissionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PermissionResponse> GetPermissionByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<PermissionUpdateRequest> UpdatePermissionAsync(long id, PermissionUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
