using Application.Common.Interfaces.Services;
using Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);

            return Ok(new { message = "Kullanıcı başarıyla oluşturuldu.", userId = result.Id });
        }
    }
}