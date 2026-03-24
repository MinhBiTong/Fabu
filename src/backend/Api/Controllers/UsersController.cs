using Application.DTOs.Requests.UserRequest;
using Application.DTOs.Responses.UserResponse;
using Application.Interfaces;
using FluentValidation;
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
        private readonly IValidator<CreateUserRequest> _createValidator; //fluentValid
        private readonly IValidator<UpdateUserRequest> _updateValidator; //fluentValid

        public UsersController(
            IUserService userService,
            IValidator<UpdateUserRequest> updateValidator,
            IValidator<CreateUserRequest> createValidator)
        {
            _userService = userService;
            _updateValidator = updateValidator;
            _createValidator = createValidator;
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
