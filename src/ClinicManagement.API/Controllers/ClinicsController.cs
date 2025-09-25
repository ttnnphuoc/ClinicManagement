using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.Constants;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClinicsController : ControllerBase
{
    private readonly IClinicService _clinicService;
    private readonly IStaffRepository _staffRepository;
    private readonly IClinicContext _clinicContext;

    public ClinicsController(IClinicService clinicService, IStaffRepository staffRepository, IClinicContext clinicContext)
    {
        _clinicService = clinicService;
        _staffRepository = staffRepository;
        _clinicContext = clinicContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetClinics([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (items, total) = await _clinicService.SearchClinicsAsync(search, page, pageSize);

        var response = new
        {
            items = items.Select(c => MapToResponse(c)),
            total,
            page,
            pageSize
        };

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveClinics()
    {
        var clinics = await _clinicService.GetActiveClinicsAsync();
        var response = clinics.Select(c => MapToResponse(c));

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("my-clinics")]
    public async Task<IActionResult> GetMyClinics()
    {
        if (!_clinicContext.CurrentUserId.HasValue)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Auth.Unauthorized));
        }

        var staff = await _staffRepository.GetByIdWithClinicsAsync(_clinicContext.CurrentUserId.Value);
        if (staff == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        var myClinics = staff.StaffClinics
            .Where(sc => sc.IsActive)
            .Select(sc => new
            {
                id = sc.Clinic.Id.ToString(),
                name = sc.Clinic.Name,
                address = sc.Clinic.Address,
                isActive = sc.Clinic.IsActive
            });

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, myClinics));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClinic(Guid id)
    {
        var clinic = await _clinicService.GetClinicByIdAsync(id);

        if (clinic == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(clinic)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateClinic([FromBody] CreateClinicRequest request)
    {
        var (success, errorCode, clinic) = await _clinicService.CreateClinicAsync(
            request.Name,
            request.Address,
            request.PhoneNumber,
            request.Email,
            request.IsActive);

        if (!success)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(clinic!)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClinic(Guid id, [FromBody] UpdateClinicRequest request)
    {
        var (success, errorCode, clinic) = await _clinicService.UpdateClinicAsync(
            id,
            request.Name,
            request.Address,
            request.PhoneNumber,
            request.Email,
            request.IsActive);

        if (!success)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(clinic!)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClinic(Guid id)
    {
        var (success, errorCode) = await _clinicService.DeleteClinicAsync(id);

        if (!success)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
    }

    private static ClinicResponse MapToResponse(Clinic c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address,
        PhoneNumber = c.PhoneNumber,
        Email = c.Email,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt
    };
}