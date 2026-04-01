using Application.DTOs.Requests;
using Application.DTOs.Requests.CustomerRequest;

// using Application.DTOs.Requests.CustomerRequest; // Bỏ comment nếu bạn để Request trong thư mục riêng
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _customerService.GetAllAsync();
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await _customerService.GetByIdAsync(id);
            if (response.Code != 200) return NotFound(response);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerCreateRequest request)
        {
            var response = await _customerService.CreateAsync(request);
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] CustomerUpdateRequest request)
        {
            var response = await _customerService.UpdateAsync(id, request);
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _customerService.DeleteAsync(id);
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }
    }
}