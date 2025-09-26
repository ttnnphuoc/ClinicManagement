using Microsoft.EntityFrameworkCore;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;

namespace ClinicManagement.Infrastructure.Repositories;

public class StaffRepository : Repository<Staff>, IStaffRepository
{
    public StaffRepository(ApplicationDbContext context, IClinicContext clinicContext) : base(context, clinicContext)
    {
    }

    public async Task<Staff?> GetByEmailOrPhoneAsync(string emailOrPhone)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => 
                (s.Email == emailOrPhone || s.PhoneNumber == emailOrPhone) && s.IsActive);
    }

    public async Task<Staff?> GetByEmailOrPhoneWithClinicsAsync(string emailOrPhone)
    {
        return await _dbSet
            .Include(s => s.StaffClinics)
                .ThenInclude(sc => sc.Clinic)
            .FirstOrDefaultAsync(s => 
                (s.Email == emailOrPhone || s.PhoneNumber == emailOrPhone) && s.IsActive);
    }

    public async Task<Staff?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
    }

    public async Task<bool> HasAccessToClinicAsync(Guid staffId, Guid clinicId)
    {
        return await _context.StaffClinics
            .AnyAsync(sc => sc.StaffId == staffId && sc.ClinicId == clinicId && sc.IsActive);
    }

    public async Task<List<Guid>> GetStaffClinicIdsAsync(Guid staffId)
    {
        return await _context.StaffClinics
            .Where(sc => sc.StaffId == staffId && sc.IsActive)
            .Select(sc => sc.ClinicId)
            .ToListAsync();
    }

    public async Task<Staff?> GetByIdWithClinicsAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.StaffClinics)
                .ThenInclude(sc => sc.Clinic)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }

    public async Task<int> GetTotalCountAsync(string? search = null)
    {
        var query = _dbSet.Where(s => !s.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => 
                s.FullName.Contains(search) || 
                s.Email.Contains(search) || 
                s.PhoneNumber.Contains(search));
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<Staff>> SearchStaffAsync(string? search, int page, int pageSize)
    {
        var query = _dbSet
            .Include(s => s.StaffClinics)
                .ThenInclude(sc => sc.Clinic)
            .Where(s => !s.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => 
                s.FullName.Contains(search) || 
                s.Email.Contains(search) || 
                s.PhoneNumber.Contains(search));
        }

        return await query
            .OrderBy(s => s.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}