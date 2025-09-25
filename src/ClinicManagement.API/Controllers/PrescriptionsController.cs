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
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService;
    private readonly IClinicContext _clinicContext;

    public PrescriptionsController(IPrescriptionService prescriptionService, IClinicContext clinicContext)
    {
        _prescriptionService = prescriptionService;
        _clinicContext = clinicContext;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPrescription(Guid id)
    {
        var prescription = await _prescriptionService.GetPrescriptionAsync(id);

        if (prescription == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(prescription)));
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientPrescriptions(Guid patientId)
    {
        var prescriptions = await _prescriptionService.GetPatientPrescriptionsAsync(patientId);
        var response = prescriptions.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("active")]
    [Authorize(Policy = Policies.ManageInventory)]
    public async Task<IActionResult> GetActivePrescriptions()
    {
        var prescriptions = await _prescriptionService.GetActivePrescriptionsAsync();
        var response = prescriptions.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpPost]
    [Authorize(Policy = Policies.ViewPatientRecords)]
    public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionRequest request)
    {
        var prescriptionMedicines = request.Medicines.Select(m => new PrescriptionMedicine
        {
            MedicineId = m.MedicineId,
            Quantity = m.Quantity,
            Dosage = m.Dosage,
            Frequency = m.Frequency,
            DurationDays = m.DurationDays,
            Instructions = m.Instructions
        });

        var (success, errorCode, prescription) = await _prescriptionService.CreatePrescriptionAsync(
            request.TreatmentHistoryId,
            prescriptionMedicines,
            request.Notes);

        if (!success)
        {
            return BadRequest(ApiResponse.ErrorResponse(errorCode ?? ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(prescription!)));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Policies.ViewPatientRecords)]
    public async Task<IActionResult> UpdatePrescription(Guid id, [FromBody] UpdatePrescriptionRequest request)
    {
        var prescriptionMedicines = request.Medicines.Select(m => new PrescriptionMedicine
        {
            MedicineId = m.MedicineId,
            Quantity = m.Quantity,
            Dosage = m.Dosage,
            Frequency = m.Frequency,
            DurationDays = m.DurationDays,
            Instructions = m.Instructions
        });

        var (success, errorCode, prescription) = await _prescriptionService.UpdatePrescriptionAsync(
            id,
            prescriptionMedicines,
            request.Notes);

        if (!success)
        {
            return errorCode == "NOT_FOUND" 
                ? NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound))
                : BadRequest(ApiResponse.ErrorResponse(errorCode ?? ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(prescription!)));
    }

    [HttpPost("{id}/dispense")]
    [Authorize(Policy = Policies.ManageInventory)]
    public async Task<IActionResult> DispenseMedicine(Guid id, [FromBody] DispenseMedicineRequest request)
    {
        var (success, errorCode) = await _prescriptionService.DispenseMedicineAsync(
            id,
            request.MedicineId,
            request.QuantityDispensed);

        if (!success)
        {
            return errorCode == "NOT_FOUND" 
                ? NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound))
                : BadRequest(ApiResponse.ErrorResponse(errorCode ?? ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
    }

    private static PrescriptionResponse MapToResponse(Prescription prescription) => new()
    {
        Id = prescription.Id,
        PatientId = prescription.PatientId,
        PatientName = prescription.Patient?.FullName ?? string.Empty,
        DoctorId = prescription.DoctorId,
        DoctorName = prescription.Doctor?.FullName ?? string.Empty,
        PrescriptionNumber = prescription.PrescriptionNumber,
        PrescriptionDate = prescription.PrescriptionDate,
        Status = prescription.Status,
        Notes = prescription.Notes,
        Medicines = prescription.PrescriptionMedicines?.Select(MapPrescriptionMedicineToResponse) ?? Enumerable.Empty<PrescriptionMedicineResponse>(),
        CreatedAt = prescription.CreatedAt
    };

    private static PrescriptionMedicineResponse MapPrescriptionMedicineToResponse(PrescriptionMedicine pm) => new()
    {
        Id = pm.Id,
        MedicineId = pm.MedicineId,
        MedicineName = pm.Medicine?.Name ?? string.Empty,
        MedicineGenericName = pm.Medicine?.GenericName ?? string.Empty,
        Quantity = pm.Quantity,
        Dosage = pm.Dosage,
        Frequency = pm.Frequency,
        DurationDays = pm.DurationDays,
        Instructions = pm.Instructions,
        QuantityDispensed = pm.QuantityDispensed,
        IsDispensed = pm.IsDispensed
    };
}