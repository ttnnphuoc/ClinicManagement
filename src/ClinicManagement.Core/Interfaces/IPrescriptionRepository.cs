using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IPrescriptionRepository : IRepository<Prescription>
{
    Task<Prescription?> GetPrescriptionWithDetailsAsync(Guid id);
    Task<IEnumerable<Prescription>> GetPatientPrescriptionsAsync(Guid patientId);
    Task<IEnumerable<Prescription>> GetActivePrescriptionsAsync();
    Task<string> GeneratePrescriptionNumberAsync();
}