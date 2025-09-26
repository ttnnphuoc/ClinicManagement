using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.Constants;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Controllers;

[Authorize(Policy = Policies.ManagePatients)]
[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IClinicContext _clinicContext;

    public PatientsController(IPatientService patientService, IClinicContext clinicContext)
    {
        _patientService = patientService;
        _clinicContext = clinicContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetPatients([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (items, total) = await _patientService.SearchPatientsAsync(search, page, pageSize);

        var response = new
        {
            items = items.Select(p => MapToResponse(p)),
            total,
            page,
            pageSize
        };

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatient(Guid id)
    {
        var patient = await _patientService.GetPatientByIdAsync(id);

        if (patient == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(patient)));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request)
    {
        if (!_clinicContext.CurrentClinicId.HasValue)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Auth.Unauthorized, 
                $"No clinic context available. User: {_clinicContext.CurrentUserId}, Role: {_clinicContext.CurrentUserRole}"));
        }
        
        var clinicId = _clinicContext.CurrentClinicId.Value;

        var (success, errorCode, patient) = await _patientService.CreatePatientAsync(
            clinicId,
            request.FullName,
            request.PhoneNumber,
            request.Email,
            request.DateOfBirth,
            request.Address,
            request.Gender,
            request.Allergies,
            request.ChronicConditions,
            request.EmergencyContactName,
            request.EmergencyContactPhone,
            request.BloodType,
            request.IdNumber,
            request.InsuranceNumber,
            request.InsuranceProvider,
            request.Occupation,
            request.ReferralSource,
            request.ReceivePromotions,
            request.Notes);

        if (!success)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(patient!)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] UpdatePatientRequest request)
    {
        var (success, errorCode, patient) = await _patientService.UpdatePatientAsync(
            id,
            request.FullName,
            request.PhoneNumber,
            request.Email,
            request.DateOfBirth,
            request.Address,
            request.Gender,
            request.Allergies,
            request.ChronicConditions,
            request.EmergencyContactName,
            request.EmergencyContactPhone,
            request.BloodType,
            request.IdNumber,
            request.InsuranceNumber,
            request.InsuranceProvider,
            request.Occupation,
            request.ReferralSource,
            request.ReceivePromotions,
            request.Notes);

        if (!success)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(patient!)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(Guid id)
    {
        var (success, errorCode) = await _patientService.DeletePatientAsync(id);

        if (!success)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
    }

    private static PatientResponse MapToResponse(Patient p) => new()
    {
        Id = p.Id,
        ClinicId = p.ClinicId,
        PatientCode = p.PatientCode,
        FullName = p.FullName,
        PhoneNumber = p.PhoneNumber,
        Email = p.Email,
        DateOfBirth = p.DateOfBirth,
        Address = p.Address,
        Gender = p.Gender,
        Allergies = p.Allergies,
        ChronicConditions = p.ChronicConditions,
        EmergencyContactName = p.EmergencyContactName,
        EmergencyContactPhone = p.EmergencyContactPhone,
        BloodType = p.BloodType,
        IdNumber = p.IdNumber,
        InsuranceNumber = p.InsuranceNumber,
        InsuranceProvider = p.InsuranceProvider,
        Occupation = p.Occupation,
        ReferralSource = p.ReferralSource,
        FirstVisitDate = p.FirstVisitDate,
        ReceivePromotions = p.ReceivePromotions,
        Notes = p.Notes,
        CreatedAt = p.CreatedAt
    };
}