using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IInventoryService
{
    Task<(bool Success, string? ErrorCode, InventoryItem? Item)> AddStockAsync(
        Guid medicineId,
        string batchNumber,
        decimal quantity,
        decimal costPrice,
        DateTime? expiryDate = null,
        string? supplier = null);

    Task<(bool Success, string? ErrorCode)> UpdateStockAsync(
        Guid inventoryItemId,
        decimal quantity,
        decimal reorderLevel);

    Task<(bool Success, string? ErrorCode)> DeductStockAsync(
        Guid medicineId,
        decimal quantity,
        string reason = "Dispensed");

    Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync();
    
    Task<IEnumerable<InventoryItem>> GetExpiringItemsAsync(int daysAhead = 30);
    
    Task<IEnumerable<InventoryItem>> GetMedicineStockAsync(Guid medicineId);
    
    Task<decimal> GetAvailableStockAsync(Guid medicineId);
    
    Task<IEnumerable<InventoryItem>> GetAllStockAsync();
}