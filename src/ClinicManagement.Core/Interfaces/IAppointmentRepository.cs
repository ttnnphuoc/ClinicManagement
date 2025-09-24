using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<Appointment>> GetByStaffIdAsync(Guid staffId);
    Task<Appointment?> GetByIdWithDetailsAsync(Guid id);
    Task<bool> IsTimeSlotAvailableAsync(Guid staffId, DateTime appointmentDate, Guid? excludeAppointmentId = null);
}