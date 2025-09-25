namespace ClinicManagement.Core.Entities;

public class InventoryItem : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public Guid MedicineId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal ReorderLevel { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal CostPrice { get; set; }
    public string? Supplier { get; set; }
    public DateTime? ReceivedDate { get; set; }

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
    public Medicine Medicine { get; set; } = null!;
}