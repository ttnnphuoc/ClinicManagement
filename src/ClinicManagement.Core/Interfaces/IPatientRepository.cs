using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IPatientRepository : IRepository<Patient>
{
    Task<IEnumerable<Patient>> SearchPatientsAsync(string? searchTerm, int page, int pageSize);
    Task<int> GetTotalCountAsync(string? searchTerm);
    Task<int> GetClinicPatientCountAsync(Guid clinicId);
}