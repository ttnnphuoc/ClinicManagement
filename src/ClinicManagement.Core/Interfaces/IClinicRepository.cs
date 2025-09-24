using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IClinicRepository : IRepository<Clinic>
{
    Task<List<Clinic>> GetClinicsByStaffIdAsync(Guid staffId);
}