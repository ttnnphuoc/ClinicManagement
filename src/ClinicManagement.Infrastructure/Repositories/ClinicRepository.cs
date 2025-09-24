using Microsoft.EntityFrameworkCore;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;

namespace ClinicManagement.Infrastructure.Repositories;

public class ClinicRepository : Repository<Clinic>, IClinicRepository
{
    public ClinicRepository(ApplicationDbContext context, IClinicContext clinicContext) : base(context, clinicContext)
    {
    }

    public async Task<List<Clinic>> GetClinicsByStaffIdAsync(Guid staffId)
    {
        return await _context.StaffClinics
            .Where(sc => sc.StaffId == staffId && sc.IsActive)
            .Select(sc => sc.Clinic)
            .ToListAsync();
    }

    public async Task<IEnumerable<Clinic>> GetActiveClinicsAsync()
    {
        return await _context.Set<Clinic>()
            .Where(c => c.IsActive && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? search = null)
    {
        var query = _context.Set<Clinic>().Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search) || c.Address.Contains(search));
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<Clinic>> SearchClinicsAsync(string? search, int page, int pageSize)
    {
        var query = _context.Set<Clinic>().Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search) || c.Address.Contains(search));
        }

        return await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}