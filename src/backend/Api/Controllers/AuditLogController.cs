using Application.DTOs.Requests.AuditLogRequest;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace greenginger.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuditLogCreateRequest request)
        {
            try
            {
                var userId = User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                request.UserId = long.Parse(userId);

                var result = await _auditLogService.CreateLogAsync(request);
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
                await _auditLogService.DeleteLogAsync(id);
                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            try
            {
                var result = await _auditLogService.GetCurrentUserLogAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            try
            {
                var result = await _auditLogService.GetAllLogPagedAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
