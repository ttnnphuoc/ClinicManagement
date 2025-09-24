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
    private readonly IClinicRepository _clinicRepository;

    public ClinicsController(IClinicRepository clinicRepository)
    {
        _clinicRepository = clinicRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetClinics([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var clinics = await _clinicRepository.SearchClinicsAsync(search, page, pageSize);
        var total = await _clinicRepository.GetTotalCountAsync(search);

        var response = new
        {
            items = clinics.Select(c => MapToResponse(c)),
            total,
            page,
            pageSize
        };

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveClinics()
    {
        var clinics = await _clinicRepository.GetActiveClinicsAsync();
        var response = clinics.Select(c => MapToResponse(c));

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClinic(Guid id)
    {
        var clinic = await _clinicRepository.GetByIdAsync(id);

        if (clinic == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(clinic)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateClinic([FromBody] CreateClinicRequest request)
    {
        var clinic = new Clinic
        {
            Name = request.Name,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            IsActive = request.IsActive
        };

        await _clinicRepository.AddAsync(clinic);
        await _clinicRepository.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(clinic)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClinic(Guid id, [FromBody] UpdateClinicRequest request)
    {
        var clinic = await _clinicRepository.GetByIdAsync(id);

        if (clinic == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        clinic.Name = request.Name;
        clinic.Address = request.Address;
        clinic.PhoneNumber = request.PhoneNumber;
        clinic.Email = request.Email;
        clinic.IsActive = request.IsActive;

        await _clinicRepository.UpdateAsync(clinic);
        await _clinicRepository.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(clinic)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClinic(Guid id)
    {
        var clinic = await _clinicRepository.GetByIdAsync(id);

        if (clinic == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        await _clinicRepository.SoftDeleteAsync(clinic);
        await _clinicRepository.SaveChangesAsync();

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