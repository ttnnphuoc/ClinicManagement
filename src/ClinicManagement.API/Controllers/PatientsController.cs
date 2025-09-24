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
    private readonly IPatientRepository _patientRepository;

    public PatientsController(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetPatients([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var patients = await _patientRepository.SearchPatientsAsync(search, page, pageSize);
        var total = await _patientRepository.GetTotalCountAsync(search);

        var response = new
        {
            items = patients.Select(p => MapToResponse(p)),
            total,
            page,
            pageSize
        };

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatient(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);

        if (patient == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(patient)));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request, [FromServices] IClinicContext clinicContext)
    {
        var clinicId = clinicContext.CurrentClinicId!.Value;
        var clinicPatientCount = await _patientRepository.GetClinicPatientCountAsync(clinicId);
        var patientCount = clinicPatientCount + 1;
        
        var patient = new Patient
        {
            PatientCode = $"PT{patientCount:D5}",
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth,
            Address = request.Address,
            Gender = request.Gender,
            Allergies = request.Allergies,
            ChronicConditions = request.ChronicConditions,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone,
            BloodType = request.BloodType,
            IdNumber = request.IdNumber,
            InsuranceNumber = request.InsuranceNumber,
            InsuranceProvider = request.InsuranceProvider,
            Occupation = request.Occupation,
            ReferralSource = request.ReferralSource,
            FirstVisitDate = DateTime.UtcNow,
            ReceivePromotions = request.ReceivePromotions,
            Notes = request.Notes
        };

        await _patientRepository.AddAsync(patient);
        await _patientRepository.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(patient)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] UpdatePatientRequest request)
    {
        var patient = await _patientRepository.GetByIdAsync(id);

        if (patient == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        patient.FullName = request.FullName;
        patient.PhoneNumber = request.PhoneNumber;
        patient.Email = request.Email;
        patient.DateOfBirth = request.DateOfBirth;
        patient.Address = request.Address;
        patient.Gender = request.Gender;
        patient.Allergies = request.Allergies;
        patient.ChronicConditions = request.ChronicConditions;
        patient.EmergencyContactName = request.EmergencyContactName;
        patient.EmergencyContactPhone = request.EmergencyContactPhone;
        patient.BloodType = request.BloodType;
        patient.IdNumber = request.IdNumber;
        patient.InsuranceNumber = request.InsuranceNumber;
        patient.InsuranceProvider = request.InsuranceProvider;
        patient.Occupation = request.Occupation;
        patient.ReferralSource = request.ReferralSource;
        patient.ReceivePromotions = request.ReceivePromotions;
        patient.Notes = request.Notes;

        await _patientRepository.UpdateAsync(patient);
        await _patientRepository.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(patient)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);

        if (patient == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        await _patientRepository.SoftDeleteAsync(patient);
        await _patientRepository.SaveChangesAsync();

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