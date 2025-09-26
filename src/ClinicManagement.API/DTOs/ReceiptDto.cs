namespace ClinicManagement.API.DTOs;

public class ReceiptDto
{
    public Guid? Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime CreatedAt { get; set; }
}