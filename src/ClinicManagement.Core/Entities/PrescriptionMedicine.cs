namespace ClinicManagement.Core.Entities;

public class PrescriptionMedicine : BaseEntity
{
    public Guid PrescriptionId { get; set; }
    public Guid MedicineId { get; set; }
    public decimal Quantity { get; set; }
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public decimal QuantityDispensed { get; set; } = 0;
    public bool IsDispensed { get; set; } = false;

    // Navigation properties
    public Prescription Prescription { get; set; } = null!;
    public Medicine Medicine { get; set; } = null!;
}