using Application.DTOs.Requests.PermissionRequest;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace greenginger.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PermissionCreateRequest request)
        {
            try
            {
                var result = await _permissionService.CreatePermissionAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _permissionService.GetAllPermissionAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("permission/{name}")]
        public async Task<IActionResult> GetPermissionByName(string name)
        {
            try
            {
                var result = await _permissionService.GetPermissionByNameAsync(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _permissionService.DeletePermissionAsync(id);
                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, PermissionUpdateRequest request)
        {
            var result = await _permissionService.UpdatePermissionAsync(id, request);
            return Ok(result);
        }
    }
}
