using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.Constants;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Controllers;

[Authorize(Policy = Policies.ViewPatientRecords)]
[ApiController]
[Route("api/[controller]")]
public class TreatmentHistoryController : ControllerBase
{
    private readonly ITreatmentHistoryService _treatmentHistoryService;
    private readonly IClinicContext _clinicContext;

    public TreatmentHistoryController(ITreatmentHistoryService treatmentHistoryService, IClinicContext clinicContext)
    {
        _treatmentHistoryService = treatmentHistoryService;
        _clinicContext = clinicContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetTreatmentHistory([FromQuery] Guid? patientId, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (items, total) = await _treatmentHistoryService.SearchTreatmentHistoryAsync(patientId, search, page, pageSize);

        var response = new
        {
            items = items.Select(th => MapToResponse(th)),
            total,
            page,
            pageSize
        };

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientTreatmentHistory(Guid patientId)
    {
        var treatments = await _treatmentHistoryService.GetPatientTreatmentHistoryAsync(patientId);
        var response = treatments.Select(th => MapToResponse(th));

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("appointment/{appointmentId}")]
    public async Task<IActionResult> GetTreatmentByAppointment(Guid appointmentId)
    {
        var treatment = await _treatmentHistoryService.GetTreatmentByAppointmentIdAsync(appointmentId);

        if (treatment == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(treatment)));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTreatment(Guid id)
    {
        var treatment = await _treatmentHistoryService.GetTreatmentByIdAsync(id);

        if (treatment == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(treatment)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTreatment([FromBody] CreateTreatmentHistoryRequest request)
    {
        var staffId = _clinicContext.CurrentUserId ?? Guid.Empty;
        
        var (success, errorCode, treatment) = await _treatmentHistoryService.CreateTreatmentAsync(
            request.PatientId,
            request.AppointmentId,
            staffId,
            request.TreatmentDate,
            request.ChiefComplaint,
            request.Symptoms,
            request.BloodPressure,
            request.Temperature,
            request.HeartRate,
            request.RespiratoryRate,
            request.Weight,
            request.Height,
            request.PhysicalExamination,
            request.Diagnosis,
            request.DifferentialDiagnosis,
            request.Treatment,
            request.Prescription,
            request.TreatmentPlan,
            request.FollowUpInstructions,
            request.NextAppointmentDate,
            request.Notes);

        if (!success)
        {
            return errorCode == "AUTH_UNAUTHORIZED"
                ? BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Auth.Unauthorized))
                : BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(treatment!)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTreatment(Guid id, [FromBody] UpdateTreatmentHistoryRequest request)
    {
        var (success, errorCode, treatment) = await _treatmentHistoryService.UpdateTreatmentAsync(
            id,
            request.ChiefComplaint,
            request.Symptoms,
            request.BloodPressure,
            request.Temperature,
            request.HeartRate,
            request.RespiratoryRate,
            request.Weight,
            request.Height,
            request.PhysicalExamination,
            request.Diagnosis,
            request.DifferentialDiagnosis,
            request.Treatment,
            request.Prescription,
            request.TreatmentPlan,
            request.FollowUpInstructions,
            request.NextAppointmentDate,
            request.Notes);

        if (!success)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(treatment!)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTreatment(Guid id)
    {
        var (success, errorCode) = await _treatmentHistoryService.DeleteTreatmentAsync(id);

        if (!success)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
    }

    private static TreatmentHistoryResponse MapToResponse(TreatmentHistory th) => new()
    {
        Id = th.Id,
        ClinicId = th.ClinicId,
        PatientId = th.PatientId,
        PatientName = th.Patient?.FullName ?? string.Empty,
        AppointmentId = th.AppointmentId,
        StaffId = th.StaffId,
        StaffName = th.Staff?.FullName ?? string.Empty,
        TreatmentDate = th.TreatmentDate,
        ChiefComplaint = th.ChiefComplaint,
        Symptoms = th.Symptoms,
        BloodPressure = th.BloodPressure,
        Temperature = th.Temperature,
        HeartRate = th.HeartRate,
        RespiratoryRate = th.RespiratoryRate,
        Weight = th.Weight,
        Height = th.Height,
        PhysicalExamination = th.PhysicalExamination,
        Diagnosis = th.Diagnosis,
        DifferentialDiagnosis = th.DifferentialDiagnosis,
        Treatment = th.Treatment,
        Prescription = th.PrescriptionNotes,
        TreatmentPlan = th.TreatmentPlan,
        FollowUpInstructions = th.FollowUpInstructions,
        NextAppointmentDate = th.NextAppointmentDate,
        Notes = th.Notes,
        CreatedAt = th.CreatedAt
    };
}