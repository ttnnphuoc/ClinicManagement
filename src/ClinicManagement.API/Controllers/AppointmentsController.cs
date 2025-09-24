using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.Constants;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Controllers;

[Authorize(Policy = Policies.ManageAppointments)]
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly IClinicContext _clinicContext;

    public AppointmentsController(IAppointmentService appointmentService, IClinicContext clinicContext)
    {
        _appointmentService = appointmentService;
        _clinicContext = clinicContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointments([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var appointments = await _appointmentService.GetAppointmentsByDateRangeAsync(startDate, endDate);
        var response = appointments.Select(a => MapToResponse(a));

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointment(Guid id)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

        if (appointment == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(appointment)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
    {
        var clinicId = _clinicContext.CurrentClinicId!.Value;

        var (success, errorCode, appointment) = await _appointmentService.CreateAppointmentAsync(
            clinicId,
            request.PatientId,
            request.StaffId,
            request.AppointmentDate,
            request.Status,
            request.Notes);

        if (!success)
        {
            return errorCode == "APPOINTMENT_TIME_SLOT_NOT_AVAILABLE"
                ? BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Appointment.TimeSlotNotAvailable))
                : BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(appointment!)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentRequest request)
    {
        var (success, errorCode, appointment) = await _appointmentService.UpdateAppointmentAsync(
            id,
            request.PatientId,
            request.StaffId,
            request.AppointmentDate,
            request.Status,
            request.Notes);

        if (!success)
        {
            return errorCode == "APPOINTMENT_TIME_SLOT_NOT_AVAILABLE"
                ? BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Appointment.TimeSlotNotAvailable))
                : errorCode == "NOT_FOUND"
                ? NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound))
                : BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(appointment!)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        var (success, errorCode) = await _appointmentService.DeleteAppointmentAsync(id);

        if (!success)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
    }

    private static AppointmentResponse MapToResponse(Appointment a) => new()
    {
        Id = a.Id,
        ClinicId = a.ClinicId,
        PatientId = a.PatientId,
        PatientName = a.Patient?.FullName ?? string.Empty,
        StaffId = a.StaffId,
        StaffName = a.Staff?.FullName ?? string.Empty,
        AppointmentDate = a.AppointmentDate,
        Status = a.Status,
        Notes = a.Notes,
        CreatedAt = a.CreatedAt
    };
}