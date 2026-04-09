using Application.DTOs.Requests;
using Application.DTOs.Requests.FeedbackRequest;

// Nếu có file DTO riêng cho Feedback thì thêm using ở đây, ví dụ: using Application.DTOs.Requests.FeedbackRequest;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbacksController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _feedbackService.GetAllAsync();
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _feedbackService.GetByIdAsync(id);
            if (response.Code != 200) return NotFound(response);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeedbackCreateRequest request)
        {
            var response = await _feedbackService.CreateAsync(request);
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] FeedbackUpdateRequest request) 
        {
            var response = await _feedbackService.UpdateAsync(id, request);
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _feedbackService.DeleteAsync(id);
            if (response.Code != 200) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending() => Ok(await _feedbackService.GetAllPendingAsync());

        [HttpPut("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(long id) => Ok(await _feedbackService.MarkAsReadAsync(id));

        [HttpPut("{id}/reply")]
        public async Task<IActionResult> MarkAsReplied(long id, [FromQuery] string replyNote) => Ok(await _feedbackService.MarkAsRepliedAsync(id, replyNote));
    }
}