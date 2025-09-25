using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IAppointmentService
{
    Task<(bool Success, string? ErrorCode, Appointment? Appointment)> CreateAppointmentAsync(
        Guid clinicId,
        Guid patientId,
        Guid staffId,
        DateTime appointmentDate,
        string status,
        string? notes);

    Task<(bool Success, string? ErrorCode, Appointment? Appointment)> UpdateAppointmentAsync(
        Guid id,
        Guid patientId,
        Guid staffId,
        DateTime appointmentDate,
        string status,
        string? notes);

    Task<(bool Success, string? ErrorCode)> DeleteAppointmentAsync(Guid id);

    Task<(bool Success, string? ErrorCode, Appointment? Appointment)> UpdateAppointmentStatusAsync(Guid id, string status);

    Task<Appointment?> GetAppointmentByIdAsync(Guid id);

    Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime? startDate, DateTime? endDate);
}