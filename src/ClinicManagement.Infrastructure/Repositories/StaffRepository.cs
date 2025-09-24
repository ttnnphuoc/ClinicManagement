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

    public async Task<bool> HasAccessToClinicAsync(Guid staffId, Guid clinicId)
    {
        return await _context.StaffClinics
            .AnyAsync(sc => sc.StaffId == staffId && sc.ClinicId == clinicId && sc.IsActive);
    }
}