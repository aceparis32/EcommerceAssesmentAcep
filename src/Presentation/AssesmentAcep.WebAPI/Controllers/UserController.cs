using Application.Features.User.Commands;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssesmentAcep.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserRepositoryService userRepositoryService;

        public UserController(IUserRepositoryService userRepositoryService)
        {
            this.userRepositoryService = userRepositoryService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] UserAuthCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [Authorize]
        [HttpGet("test-auth")]
        public async Task<IActionResult> TestAuthUser()
        {
            var userModel = new
            {
                Username = userRepositoryService.Username,
                Fullname = userRepositoryService.Fullname,
                Email = userRepositoryService.Email,
                Role = userRepositoryService.Role
            };
            return Ok(userModel);

        }
    }
}
