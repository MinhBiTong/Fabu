using Application.DTOs.Requests;
using Application.DTOs.Requests.CustomerRequest;

using Application.DTOs.Requests.ServiceRequest;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _serviceService.GetAllAsync();
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await _serviceService.GetByIdAsync(id);
            if (response.Code != 200) return NotFound(response);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceCreateRequest request)
        {
            var response = await _serviceService.CreateAsync(request);
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] ServiceCreateRequest request)
        {
            var response = await _serviceService.UpdateAsync(id, request);

            if (response.Code != 200)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _serviceService.DeleteAsync(id);

            if (response.Code != 200)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpGet("category/{category}/active")]
        public async Task<IActionResult> GetActiveByCategory(string category) => Ok(await _serviceService.GetActiveServicesByCategoryAsync(category));

        [HttpGet("popular/{top}")]
        public async Task<IActionResult> GetPopular(int top) => Ok(await _serviceService.GetPopularServicesAsync(top));

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code) => Ok(await _serviceService.GetByCodeAsync(code));

        [HttpGet("{id}/is-active")]
        public async Task<IActionResult> IsActive(long id) => Ok(await _serviceService.IsServiceActiveAsync(id));

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword) => Ok(await _serviceService.SearchServicesAsync(keyword));

    }
}