using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IClinicContext _clinicContext;

    public InventoryService(IInventoryRepository inventoryRepository, IClinicContext clinicContext)
    {
        _inventoryRepository = inventoryRepository;
        _clinicContext = clinicContext;
    }

    public async Task<(bool Success, string? ErrorCode, InventoryItem? Item)> AddStockAsync(
        Guid medicineId,
        string batchNumber,
        decimal quantity,
        decimal costPrice,
        DateTime? expiryDate = null,
        string? supplier = null)
    {
        var inventoryItem = new InventoryItem
        {
            ClinicId = _clinicContext.CurrentClinicId!.Value,
            MedicineId = medicineId,
            BatchNumber = batchNumber,
            Quantity = quantity,
            ReorderLevel = 10, // Default reorder level
            ExpiryDate = expiryDate,
            CostPrice = costPrice,
            Supplier = supplier,
            ReceivedDate = DateTime.UtcNow
        };

        await _inventoryRepository.AddAsync(inventoryItem);
        await _inventoryRepository.SaveChangesAsync();

        return (true, null, inventoryItem);
    }

    public async Task<(bool Success, string? ErrorCode)> UpdateStockAsync(
        Guid inventoryItemId,
        decimal quantity,
        decimal reorderLevel)
    {
        var item = await _inventoryRepository.GetByIdAsync(inventoryItemId);
        if (item == null)
        {
            return (false, "INVENTORY_ITEM_NOT_FOUND");
        }

        item.Quantity = quantity;
        item.ReorderLevel = reorderLevel;

        await _inventoryRepository.UpdateAsync(item);
        await _inventoryRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorCode)> DeductStockAsync(
        Guid medicineId,
        decimal quantity,
        string reason = "Dispensed")
    {
        var availableStock = await GetAvailableStockAsync(medicineId);
        if (availableStock < quantity)
        {
            return (false, "INSUFFICIENT_STOCK");
        }

        var remainingQuantity = quantity;
        var stockItems = await GetMedicineStockAsync(medicineId);

        // Use FIFO (First In, First Out) - oldest stock first
        foreach (var item in stockItems.OrderBy(i => i.ExpiryDate).ThenBy(i => i.ReceivedDate))
        {
            if (remainingQuantity <= 0) break;

            var deductFromThis = Math.Min(item.Quantity, remainingQuantity);
            item.Quantity -= deductFromThis;
            remainingQuantity -= deductFromThis;

            await _inventoryRepository.UpdateAsync(item);
        }

        await _inventoryRepository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync()
    {
        return await _inventoryRepository.GetLowStockItemsAsync();
    }

    public async Task<IEnumerable<InventoryItem>> GetExpiringItemsAsync(int daysAhead = 30)
    {
        var expiryDate = DateTime.UtcNow.AddDays(daysAhead);
        return await _inventoryRepository.GetExpiringItemsAsync(expiryDate);
    }

    public async Task<IEnumerable<InventoryItem>> GetMedicineStockAsync(Guid medicineId)
    {
        return await _inventoryRepository.GetMedicineStockAsync(medicineId);
    }

    public async Task<decimal> GetAvailableStockAsync(Guid medicineId)
    {
        return await _inventoryRepository.GetAvailableStockAsync(medicineId);
    }

    public async Task<IEnumerable<InventoryItem>> GetAllStockAsync()
    {
        return await _inventoryRepository.GetAllAsync();
    }
}