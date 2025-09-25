namespace ClinicManagement.Core.Entities;

public class Medicine : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public string? Manufacturer { get; set; }
    public string? Dosage { get; set; }
    public string? Form { get; set; } // Tablet, Capsule, Syrup, Injection, etc.
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
    public ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    public ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; } = new List<PrescriptionMedicine>();
}