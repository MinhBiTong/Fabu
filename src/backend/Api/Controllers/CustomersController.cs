using Application.DTOs.Requests;
using Application.DTOs.Requests.CustomerRequest;

// using Application.DTOs.Requests.CustomerRequest; // Bỏ comment nếu bạn để Request trong thư mục riêng
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
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
        [HttpGet("mobile/{mobile}")]
        public async Task<IActionResult> GetByMobile(string mobile) => Ok(await _customerService.GetByMobileNumberAsync(mobile));

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(long userId) => Ok(await _customerService.GetByUserIdAsync(userId));

        [HttpPost("{customerId}/link-user/{userId}")]
        public async Task<IActionResult> LinkUser(long customerId, long userId) => Ok(await _customerService.LinkUserToCustomerAsync(customerId, userId));

        [HttpGet("exists-mobile/{mobile}")]
        public async Task<IActionResult> ExistsByMobile(string mobile) => Ok(await _customerService.ExistsByMobileAsync(mobile));

        [HttpGet("active")]
        public async Task<IActionResult> GetActive() => Ok(await _customerService.GetActiveCustomersAsync());

        [HttpGet("{id}/with-account")]
        public async Task<IActionResult> GetWithAccount(long id) => Ok(await _customerService.GetWithAccountAsync(id));

        [HttpGet("top-spending/{top}")]
        public async Task<IActionResult> GetTopSpending(int top) => Ok(await _customerService.GetTopCustomersBySpendingAsync(top));

        [HttpGet("unpaid-bills")]
        public async Task<IActionResult> GetUnpaidBills() => Ok(await _customerService.GetCustomersWithUnpaidBillsAsync());
    }
}