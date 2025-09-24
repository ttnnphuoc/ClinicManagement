using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EmailOrPhone) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Email/Phone and Password are required" });
        }

        var (success, token, fullName, email, role) = await _authService.LoginAsync(request.EmailOrPhone, request.Password);

        if (!success)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var response = new LoginResponse
        {
            Token = token,
            FullName = fullName,
            Email = email,
            Role = role
        };

        return Ok(response);
    }
}