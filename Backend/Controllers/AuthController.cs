using Microsoft.AspNetCore.Mvc;
using USMS.DTOs.Auth;
using USMS.Services;

namespace USMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterStudentAsync(dto);
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(result.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(result.Data);
    }
}