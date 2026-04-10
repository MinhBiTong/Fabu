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

        [HttpGet("active")]
        public async Task<IActionResult> GetActive() => Ok(await _rechargePlanService.GetActivePlansAsync());

        [HttpGet("amount/{amount}")]
        public async Task<IActionResult> GetByAmount(decimal amount) => Ok(await _rechargePlanService.GetByAmountAsync(amount));

        [HttpGet("price-range")]
        public async Task<IActionResult> GetByPriceRange([FromQuery] decimal min, [FromQuery] decimal max) => Ok(await _rechargePlanService.GetPlansByPriceRangeAsync(min, max));

        [HttpGet("popular/{top}")]
        public async Task<IActionResult> GetPopular(int top) => Ok(await _rechargePlanService.GetPopularPlansAsync(top));

        [HttpGet("provider/{provider}")]
        public async Task<IActionResult> GetByProvider(string provider) => Ok(await _rechargePlanService.GetPlansByProviderAsync(provider));

        [HttpGet("{id}/is-active")]
        public async Task<IActionResult> IsActive(long id) => Ok(await _rechargePlanService.IsPlanActiveAsync(id));
    }
}