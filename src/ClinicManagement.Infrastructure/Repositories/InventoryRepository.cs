using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Repositories;

public class InventoryRepository : Repository<InventoryItem>, IInventoryRepository
{
    public InventoryRepository(ApplicationDbContext context, IClinicContext clinicContext) 
        : base(context, clinicContext)
    {
    }

    public async Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync()
    {
        return await _context.InventoryItems
            .Include(i => i.Medicine)
            .Where(i => i.Quantity <= i.ReorderLevel)
            .ToListAsync();
    }

    public async Task<IEnumerable<InventoryItem>> GetExpiringItemsAsync(DateTime beforeDate)
    {
        return await _context.InventoryItems
            .Include(i => i.Medicine)
            .Where(i => i.ExpiryDate.HasValue && i.ExpiryDate.Value <= beforeDate && i.Quantity > 0)
            .OrderBy(i => i.ExpiryDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<InventoryItem>> GetMedicineStockAsync(Guid medicineId)
    {
        return await _context.InventoryItems
            .Where(i => i.MedicineId == medicineId && i.Quantity > 0)
            .OrderBy(i => i.ExpiryDate)
            .ToListAsync();
    }

    public async Task<decimal> GetAvailableStockAsync(Guid medicineId)
    {
        return await _context.InventoryItems
            .Where(i => i.MedicineId == medicineId)
            .SumAsync(i => i.Quantity);
    }

    public async Task<InventoryItem?> GetOldestStockAsync(Guid medicineId)
    {
        return await _context.InventoryItems
            .Where(i => i.MedicineId == medicineId && i.Quantity > 0)
            .OrderBy(i => i.ExpiryDate)
            .ThenBy(i => i.ReceivedDate)
            .FirstOrDefaultAsync();
    }
}