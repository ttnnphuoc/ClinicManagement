using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IInventoryRepository : IRepository<InventoryItem>
{
    Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync();
    Task<IEnumerable<InventoryItem>> GetExpiringItemsAsync(DateTime beforeDate);
    Task<IEnumerable<InventoryItem>> GetMedicineStockAsync(Guid medicineId);
    Task<decimal> GetAvailableStockAsync(Guid medicineId);
    Task<InventoryItem?> GetOldestStockAsync(Guid medicineId);
}