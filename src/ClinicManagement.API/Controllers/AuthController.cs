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
    private readonly IClinicRepository _clinicRepository;

    public AuthController(IAuthService authService, IClinicRepository clinicRepository)
    {
        _authService = authService;
        _clinicRepository = clinicRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EmailOrPhone) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Auth.FieldsRequired));
        }

        var (success, token, fullName, email, role, clinicIds) = await _authService.LoginAsync(request.EmailOrPhone, request.Password);

        if (!success)
        {
            return Unauthorized(ApiResponse.ErrorResponse(ResponseCodes.Auth.InvalidCredentials));
        }

        var clinics = await _clinicRepository.FindAsync(c => clinicIds.Contains(c.Id));

        var loginData = new LoginResponse
        {
            Token = token,
            FullName = fullName,
            Email = email,
            Role = role,
            Clinics = clinics.Select(c => new ClinicInfo { Id = c.Id, Name = c.Name }).ToList()
        };

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Auth.LoginSuccess, loginData));
    }

    [HttpPost("select-clinic")]
    public async Task<IActionResult> SelectClinic([FromBody] SelectClinicRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EmailOrPhone) || string.IsNullOrWhiteSpace(request.Password) || request.ClinicId == Guid.Empty)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Auth.FieldsRequired));
        }

        var (success, token, fullName, email, role, clinicId) = await _authService.LoginWithClinicAsync(request.EmailOrPhone, request.Password, request.ClinicId);

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