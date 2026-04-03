using Application.DTOs.Requests.AuditLogRequest;
using Application.Interfaces;
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
            var result = await _auditLogService.CreateLogAsync(request);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _auditLogService.DeleteLogAsync(id);
            return Ok(new { message = "Deleted successfully" });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var result = await _auditLogService.GetCurrentUserLogAsync(userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var result = await _auditLogService.GetAllLogPagedAsync(page, pageSize);
            return Ok(result);
        }
    }
}
