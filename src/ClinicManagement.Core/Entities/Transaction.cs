namespace ClinicManagement.Core.Entities;

public class Transaction : BaseEntity
{
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
}