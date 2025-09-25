using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface ITreatmentHistoryRepository : IRepository<TreatmentHistory>
{
    Task<IEnumerable<TreatmentHistory>> GetPatientTreatmentHistoryAsync(Guid patientId);
    Task<TreatmentHistory?> GetByAppointmentIdAsync(Guid appointmentId);
    Task<int> GetTotalCountAsync(Guid? patientId = null, string? search = null);
    Task<IEnumerable<TreatmentHistory>> SearchTreatmentHistoryAsync(Guid? patientId, string? search, int page, int pageSize);
}