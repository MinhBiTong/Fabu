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
            serviceService = _serviceService;
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
            
            return null;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            return null;
        }
    }
}