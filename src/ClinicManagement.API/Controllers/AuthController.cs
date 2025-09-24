using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.DTOs;
using ClinicManagement.API.Constants;
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
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Auth.FieldsRequired));
        }

        var (success, token, fullName, email, role) = await _authService.LoginAsync(request.EmailOrPhone, request.Password);

        if (!success)
        {
            return Unauthorized(ApiResponse.ErrorResponse(ResponseCodes.Auth.InvalidCredentials));
        }

        var loginData = new LoginResponse
        {
            Token = token,
            FullName = fullName,
            Email = email,
            Role = role
        };

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Auth.LoginSuccess, loginData));
    }
}