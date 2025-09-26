namespace ClinicManagement.API.DTOs;

public class InventoryItemDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public DateTime? LastRestocked { get; set; }
    public string Status { get; set; } = string.Empty; // "in_stock", "low_stock", "out_of_stock"
    public DateTime CreatedAt { get; set; }
}