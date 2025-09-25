using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentService(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<(bool Success, string? ErrorCode, Appointment? Appointment)> CreateAppointmentAsync(
        Guid clinicId,
        Guid patientId,
        Guid staffId,
        DateTime appointmentDate,
        string status,
        string? notes)
    {
        var isAvailable = await _appointmentRepository.IsTimeSlotAvailableAsync(staffId, appointmentDate);
        
        if (!isAvailable)
        {
            return (false, "APPOINTMENT_TIME_SLOT_NOT_AVAILABLE", null);
        }

        var appointment = new Appointment
        {
            ClinicId = clinicId,
            PatientId = patientId,
            StaffId = staffId,
            AppointmentDate = appointmentDate,
            Status = status,
            Notes = notes
        };

        await _appointmentRepository.AddAsync(appointment);
        await _appointmentRepository.SaveChangesAsync();

        var created = await _appointmentRepository.GetByIdWithDetailsAsync(appointment.Id);
        return (true, null, created);
    }

    public async Task<(bool Success, string? ErrorCode, Appointment? Appointment)> UpdateAppointmentAsync(
        Guid id,
        Guid patientId,
        Guid staffId,
        DateTime appointmentDate,
        string status,
        string? notes)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
        {
            return (false, "NOT_FOUND", null);
        }

        var isAvailable = await _appointmentRepository.IsTimeSlotAvailableAsync(staffId, appointmentDate, id);
        
        if (!isAvailable)
        {
            return (false, "APPOINTMENT_TIME_SLOT_NOT_AVAILABLE", null);
        }

        appointment.PatientId = patientId;
        appointment.StaffId = staffId;
        appointment.AppointmentDate = appointmentDate;
        appointment.Status = status;
        appointment.Notes = notes;

        await _appointmentRepository.UpdateAsync(appointment);
        await _appointmentRepository.SaveChangesAsync();

        var updated = await _appointmentRepository.GetByIdWithDetailsAsync(id);
        return (true, null, updated);
    }

    public async Task<(bool Success, string? ErrorCode)> DeleteAppointmentAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
        {
            return (false, "NOT_FOUND");
        }

        await _appointmentRepository.SoftDeleteAsync(appointment);
        await _appointmentRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorCode, Appointment? Appointment)> UpdateAppointmentStatusAsync(Guid id, string status)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
        {
            return (false, "NOT_FOUND", null);
        }

        appointment.Status = status;

        await _appointmentRepository.UpdateAsync(appointment);
        await _appointmentRepository.SaveChangesAsync();

        var updated = await _appointmentRepository.GetByIdWithDetailsAsync(id);
        return (true, null, updated);
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(Guid id)
    {
        return await _appointmentRepository.GetByIdWithDetailsAsync(id);
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime? startDate, DateTime? endDate)
    {
        var start = startDate ?? DateTime.UtcNow.Date;
        var end = endDate ?? DateTime.UtcNow.Date.AddMonths(1);

        return await _appointmentRepository.GetByDateRangeAsync(start, end);
    }
}