namespace ClinicManagement.Core.Entities;

public class Receipt : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public Guid BillId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public string ReceiptType { get; set; } = "Receipt"; // Receipt, Invoice, Credit Note
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Generated"; // Generated, Sent, Cancelled
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public bool IsEmailSent { get; set; } = false;
    public DateTime? EmailSentDate { get; set; }
    public string? FilePath { get; set; }
    public Guid GeneratedByStaffId { get; set; }

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
    public Bill Bill { get; set; } = null!;
    public Staff GeneratedByStaff { get; set; } = null!;
}