namespace ClinicManagement.Core.Entities;

public class Payment : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public Guid BillId { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // Cash, Card, Transfer, Insurance
    public string? Reference { get; set; } // Transaction ID, Check number, etc.
    public string Status { get; set; } = "Completed"; // Completed, Failed, Refunded
    public string? Notes { get; set; }
    public Guid ReceivedByStaffId { get; set; }

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
    public Bill Bill { get; set; } = null!;
    public Staff ReceivedByStaff { get; set; } = null!;
}