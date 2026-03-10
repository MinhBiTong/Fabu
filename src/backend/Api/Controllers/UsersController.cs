using Application.DTOs.Responses.UserResponse;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize] //global auth
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")] //object level: tu check claims userId
        public async Task<ActionResult<UserResponse>> GetCurrentUser()
        {
            var user = await _userService.GetCurrentUserAsync(); //inside: check claims.UserId == DB id - fix BOLA
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")] //policy check role
        public async Task<ActionResult<List<UserResponse>>> GetAllUsers([FromQuery] int page = 1, int pageSize = 20)
        {
            //pagination: skip((pagge-1)*pageSize).Take(pageSize)
            var users = await _userService.GetAllUsersPagedAsync(page, pageSize);
            return Ok(users);
        }
    }
}
