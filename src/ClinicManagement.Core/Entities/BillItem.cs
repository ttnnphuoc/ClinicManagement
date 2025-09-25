namespace ClinicManagement.Core.Entities;

public class BillItem : BaseEntity
{
    public Guid BillId { get; set; }
    public Guid? ServiceId { get; set; }
    public Guid? MedicineId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty; // Service, Medicine, Other
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Bill Bill { get; set; } = null!;
    public Service? Service { get; set; }
    public Medicine? Medicine { get; set; }
}