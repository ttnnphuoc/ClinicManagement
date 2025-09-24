using Microsoft.EntityFrameworkCore;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;

namespace ClinicManagement.Infrastructure.Repositories;

public class ServiceRepository : Repository<Service>, IServiceRepository
{
    public ServiceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Service>> GetActiveServicesAsync()
    {
        return await _context.Set<Service>()
            .Where(s => s.IsActive && !s.IsDeleted)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? search = null)
    {
        var query = _context.Set<Service>().Where(s => !s.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => s.Name.Contains(search) || (s.Description != null && s.Description.Contains(search)));
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<Service>> SearchServicesAsync(string? search, int page, int pageSize)
    {
        var query = _context.Set<Service>().Where(s => !s.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => s.Name.Contains(search) || (s.Description != null && s.Description.Contains(search)));
        }

        return await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}