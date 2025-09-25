using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Repositories;

public class MedicineRepository : Repository<Medicine>, IMedicineRepository
{
    public MedicineRepository(ApplicationDbContext context, IClinicContext clinicContext) 
        : base(context, clinicContext)
    {
    }

    public async Task<IEnumerable<Medicine>> GetActiveMedicinesAsync()
    {
        return await _context.Medicines
            .Where(m => m.IsActive)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<Medicine?> GetMedicineWithInventoryAsync(Guid id)
    {
        return await _context.Medicines
            .Include(m => m.InventoryItems)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Medicine>> SearchMedicinesAsync(string searchTerm)
    {
        return await _context.Medicines
            .Where(m => m.IsActive && (
                m.Name.Contains(searchTerm) || 
                m.GenericName!.Contains(searchTerm) ||
                m.Manufacturer!.Contains(searchTerm)))
            .OrderBy(m => m.Name)
            .ToListAsync();
    }
}