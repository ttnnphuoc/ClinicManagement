namespace ClinicManagement.Core.Entities;

public class Inventory : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }

    public Clinic Clinic { get; set; } = null!;
}