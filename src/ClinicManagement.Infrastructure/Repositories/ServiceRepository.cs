using Microsoft.EntityFrameworkCore;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;

namespace ClinicManagement.Infrastructure.Repositories;

public class ServiceRepository : Repository<Service>, IServiceRepository
{
    public ServiceRepository(ApplicationDbContext context, IClinicContext clinicContext) : base(context, clinicContext)
    {
    }

    public async Task<IEnumerable<Service>> GetActiveServicesAsync()
    {
        var query = _context.Set<Service>()
            .Where(s => s.IsActive && !s.IsDeleted);
            
        // Filter by clinic if user is not SuperAdmin
        if (_clinicContext.CurrentClinicId.HasValue)
        {
            query = query.Where(s => s.ClinicId == _clinicContext.CurrentClinicId.Value);
        }
            
        return await query
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? search = null)
    {
        var query = _context.Set<Service>().Where(s => !s.IsDeleted);

        // Filter by clinic if user is not SuperAdmin
        if (_clinicContext.CurrentClinicId.HasValue)
        {
            query = query.Where(s => s.ClinicId == _clinicContext.CurrentClinicId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => s.Name.Contains(search) || (s.Description != null && s.Description.Contains(search)));
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<Service>> SearchServicesAsync(string? search, int page, int pageSize)
    {
        var query = _context.Set<Service>().Where(s => !s.IsDeleted);

        // Filter by clinic if user is not SuperAdmin  
        if (_clinicContext.CurrentClinicId.HasValue)
        {
            query = query.Where(s => s.ClinicId == _clinicContext.CurrentClinicId.Value);
        }

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