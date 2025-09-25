namespace ClinicManagement.Core.Entities;

public class Bill : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Paid, Partial, Cancelled
    public string PaymentMethod { get; set; } = string.Empty; // Cash, Card, Transfer, Insurance
    public string? Notes { get; set; }
    public Guid CreatedByStaffId { get; set; }

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public Appointment? Appointment { get; set; }
    public Staff CreatedByStaff { get; set; } = null!;
    public ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}