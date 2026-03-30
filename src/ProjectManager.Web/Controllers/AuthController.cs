using ProjectManager.Application.Interfaces;
using ProjectManager.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManager.Web.Controllers.Api
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthApiController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message) = await _authService.RegisterAsync(dto);

            if (!success) return BadRequest(new { Message = message });

            return Ok(new { Message = message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, token, email) = await _authService.LoginAsync(dto);

            if (!success) return Unauthorized(new { Message = "Invalid credentials" });

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = email,
                Expiration = DateTime.Now.AddDays(1)
            });
        }
    }
}
