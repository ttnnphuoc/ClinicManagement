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
}