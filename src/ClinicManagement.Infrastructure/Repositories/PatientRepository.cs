using Microsoft.EntityFrameworkCore;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;

namespace ClinicManagement.Infrastructure.Repositories;

public class PatientRepository : Repository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext context, IClinicContext clinicContext) : base(context, clinicContext)
    {
    }

    public async Task<IEnumerable<Patient>> SearchPatientsAsync(string? searchTerm, int page, int pageSize)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => 
                p.FullName.Contains(searchTerm) || 
                p.PhoneNumber.Contains(searchTerm) ||
                (p.Email != null && p.Email.Contains(searchTerm)));
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? searchTerm)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => 
                p.FullName.Contains(searchTerm) || 
                p.PhoneNumber.Contains(searchTerm) ||
                (p.Email != null && p.Email.Contains(searchTerm)));
        }

        return await query.CountAsync();
    }

    public async Task<int> GetClinicPatientCountAsync(Guid clinicId)
    {
        return await _dbSet.CountAsync(p => p.ClinicId == clinicId);
    }
}