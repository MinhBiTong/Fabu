using Application.DTOs.Requests.CustomerServiceRequest;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class CustomerServicesController : ControllerBase
	{
		private readonly ICustomerServicesService _customerServicesService;

		public CustomerServicesController(ICustomerServicesService customerServicesService)
		{
			_customerServicesService = customerServicesService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var response = await _customerServicesService.GetAllAsync();
			return StatusCode(response.Code, response);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CustomerServiceCreateRequest request)
		{
			var response = await _customerServicesService.CreateAsync(request);
			return StatusCode(response.Code, response);
		}
	}
}