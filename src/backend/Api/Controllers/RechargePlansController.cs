using Application.DTOs.Requests;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RechargePlansController : ControllerBase
    {
        private readonly IRechargePlanService _rechargePlanService;

        public RechargePlansController(IRechargePlanService rechargePlanService)
        {
            _rechargePlanService = rechargePlanService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _rechargePlanService.GetAllAsync();
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _rechargePlanService.GetByIdAsync(id);
            if (response.Code != 200) return NotFound(response);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRechargePlanRequest request)
        {
            var response = await _rechargePlanService.CreateAsync(request);
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRechargePlanRequest request)
        {
            var response = await _rechargePlanService.UpdateAsync(id, request);
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _rechargePlanService.DeleteAsync(id);
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }
    }
}