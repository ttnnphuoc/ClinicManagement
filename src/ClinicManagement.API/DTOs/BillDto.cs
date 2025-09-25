namespace ClinicManagement.API.DTOs;

public record CreateBillRequest
{
    public Guid PatientId { get; init; }
    public Guid? AppointmentId { get; init; }
    public IEnumerable<BillItemRequest> Items { get; init; } = new List<BillItemRequest>();
    public decimal DiscountAmount { get; init; } = 0;
    public decimal DiscountPercentage { get; init; } = 0;
    public string? Notes { get; init; }
}

public record UpdateBillRequest
{
    public IEnumerable<BillItemRequest> Items { get; init; } = new List<BillItemRequest>();
    public decimal DiscountAmount { get; init; } = 0;
    public decimal DiscountPercentage { get; init; } = 0;
    public string? Notes { get; init; }
}

public record BillItemRequest
{
    public Guid? ServiceId { get; init; }
    public Guid? MedicineId { get; init; }
    public string ItemName { get; init; } = string.Empty;
    public string ItemType { get; init; } = string.Empty; // Service, Medicine, Other
    public decimal Quantity { get; init; } = 1;
    public decimal UnitPrice { get; init; }
    public string? Notes { get; init; }
}

public record ProcessPaymentRequest
{
    public decimal Amount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string? Reference { get; init; }
    public string? Notes { get; init; }
}

public record BillResponse
{
    public Guid Id { get; init; }
    public Guid ClinicId { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public Guid? AppointmentId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public DateTime BillDate { get; init; }
    public decimal SubTotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal DiscountPercentage { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public IEnumerable<BillItemResponse> Items { get; init; } = new List<BillItemResponse>();
    public IEnumerable<PaymentResponse> Payments { get; init; } = new List<PaymentResponse>();
    public DateTime CreatedAt { get; init; }
}

public record BillItemResponse
{
    public Guid Id { get; init; }
    public Guid? ServiceId { get; init; }
    public string ServiceName { get; init; } = string.Empty;
    public Guid? MedicineId { get; init; }
    public string MedicineName { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string ItemType { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }
    public string? Notes { get; init; }
}

public record PaymentResponse
{
    public Guid Id { get; init; }
    public string PaymentNumber { get; init; } = string.Empty;
    public DateTime PaymentDate { get; init; }
    public decimal Amount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string? Reference { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public string ReceivedByStaffName { get; init; } = string.Empty;
}