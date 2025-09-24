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
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IClinicContext _clinicContext;

    public AppointmentsController(IAppointmentRepository appointmentRepository, IClinicContext clinicContext)
    {
        _appointmentRepository = appointmentRepository;
        _clinicContext = clinicContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointments([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var start = startDate ?? DateTime.UtcNow.Date;
        var end = endDate ?? DateTime.UtcNow.Date.AddMonths(1);

        var appointments = await _appointmentRepository.GetByDateRangeAsync(start, end);
        var response = appointments.Select(a => MapToResponse(a));

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointment(Guid id)
    {
        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(id);

        if (appointment == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(appointment)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
    {
        var isAvailable = await _appointmentRepository.IsTimeSlotAvailableAsync(request.StaffId, request.AppointmentDate);
        
        if (!isAvailable)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Appointment.TimeSlotNotAvailable));
        }

        var clinicId = _clinicContext.CurrentClinicId!.Value;

        var appointment = new Appointment
        {
            ClinicId = clinicId,
            PatientId = request.PatientId,
            StaffId = request.StaffId,
            AppointmentDate = request.AppointmentDate,
            Status = request.Status,
            Notes = request.Notes
        };

        await _appointmentRepository.AddAsync(appointment);
        await _appointmentRepository.SaveChangesAsync();

        var created = await _appointmentRepository.GetByIdWithDetailsAsync(appointment.Id);
        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(created!)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentRequest request)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);

        if (appointment == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        var isAvailable = await _appointmentRepository.IsTimeSlotAvailableAsync(request.StaffId, request.AppointmentDate, id);
        
        if (!isAvailable)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Appointment.TimeSlotNotAvailable));
        }

        appointment.PatientId = request.PatientId;
        appointment.StaffId = request.StaffId;
        appointment.AppointmentDate = request.AppointmentDate;
        appointment.Status = request.Status;
        appointment.Notes = request.Notes;

        await _appointmentRepository.UpdateAsync(appointment);
        await _appointmentRepository.SaveChangesAsync();

        var updated = await _appointmentRepository.GetByIdWithDetailsAsync(id);
        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(updated!)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);

        if (appointment == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        await _appointmentRepository.SoftDeleteAsync(appointment);
        await _appointmentRepository.SaveChangesAsync();

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