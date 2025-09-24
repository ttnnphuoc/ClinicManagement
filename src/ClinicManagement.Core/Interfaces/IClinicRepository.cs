using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IClinicRepository : IRepository<Clinic>
{
    Task<List<Clinic>> GetClinicsByStaffIdAsync(Guid staffId);
    Task<IEnumerable<Clinic>> GetActiveClinicsAsync();
    Task<int> GetTotalCountAsync(string? search = null);
    Task<IEnumerable<Clinic>> SearchClinicsAsync(string? search, int page, int pageSize);
}