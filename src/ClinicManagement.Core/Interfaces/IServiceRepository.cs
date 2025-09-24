using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IServiceRepository : IRepository<Service>
{
    Task<IEnumerable<Service>> GetActiveServicesAsync();
    Task<int> GetTotalCountAsync(string? search = null);
    Task<IEnumerable<Service>> SearchServicesAsync(string? search, int page, int pageSize);
}