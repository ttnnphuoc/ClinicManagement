using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.Constants;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Controllers;

[Authorize(Policy = Policies.ManageStaff)]
[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStaff([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (items, total) = await _staffService.SearchStaffAsync(search, page, pageSize);

        var response = new
        {
            items = items.Select(s => MapToResponse(s)),
            total,
            page,
            pageSize
        };

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStaffById(Guid id)
    {
        var staff = await _staffService.GetStaffByIdAsync(id);

        if (staff == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(staff)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
    {
        var (success, errorCode, staff) = await _staffService.CreateStaffAsync(
            request.FullName,
            request.Email,
            request.Password,
            request.PhoneNumber,
            request.Role,
            request.ClinicIds,
            request.IsActive);

        if (!success)
        {
            return errorCode == "AUTH_EMAIL_EXISTS" 
                ? BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Auth.EmailExists))
                : BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.InvalidInput));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(staff!)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStaff(Guid id, [FromBody] UpdateStaffRequest request)
    {
        var (success, errorCode, staff) = await _staffService.UpdateStaffAsync(
            id,
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.Role,
            request.ClinicIds,
            request.IsActive);

        if (!success)
        {
            return errorCode == "NOT_FOUND"
                ? NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound))
                : BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.InvalidInput));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(staff!)));
    }

    [HttpPut("{id}/password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request)
    {
        var (success, errorCode) = await _staffService.ChangePasswordAsync(id, request.NewPassword);

        if (!success)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaff(Guid id)
    {
        var (success, errorCode) = await _staffService.DeleteStaffAsync(id);

        if (!success)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
    }

    private static StaffResponse MapToResponse(Staff s) => new()
    {
        Id = s.Id,
        FullName = s.FullName,
        Email = s.Email,
        PhoneNumber = s.PhoneNumber,
        Role = s.Role.ToString(),
        IsActive = s.IsActive,
        Clinics = s.StaffClinics.Select(sc => new StaffClinicResponse
        {
            ClinicId = sc.ClinicId,
            ClinicName = sc.Clinic?.Name ?? "",
            IsActive = sc.IsActive
        }).ToList(),
        CreatedAt = s.CreatedAt
    };
}