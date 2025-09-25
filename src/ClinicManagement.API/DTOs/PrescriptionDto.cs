namespace ClinicManagement.API.DTOs;

public record CreatePrescriptionRequest
{
    public Guid TreatmentHistoryId { get; init; }
    public IEnumerable<PrescriptionMedicineRequest> Medicines { get; init; } = new List<PrescriptionMedicineRequest>();
    public string? Notes { get; init; }
}

public record UpdatePrescriptionRequest
{
    public IEnumerable<PrescriptionMedicineRequest> Medicines { get; init; } = new List<PrescriptionMedicineRequest>();
    public string? Notes { get; init; }
}

public record PrescriptionMedicineRequest
{
    public Guid MedicineId { get; init; }
    public decimal Quantity { get; init; }
    public string Dosage { get; init; } = string.Empty;
    public string Frequency { get; init; } = string.Empty;
    public int DurationDays { get; init; }
    public string Instructions { get; init; } = string.Empty;
}

public record DispenseMedicineRequest
{
    public Guid MedicineId { get; init; }
    public decimal QuantityDispensed { get; init; }
}

public record PrescriptionResponse
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public Guid DoctorId { get; init; }
    public string DoctorName { get; init; } = string.Empty;
    public string PrescriptionNumber { get; init; } = string.Empty;
    public DateTime PrescriptionDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public IEnumerable<PrescriptionMedicineResponse> Medicines { get; init; } = new List<PrescriptionMedicineResponse>();
    public DateTime CreatedAt { get; init; }
}

public record PrescriptionMedicineResponse
{
    public Guid Id { get; init; }
    public Guid MedicineId { get; init; }
    public string MedicineName { get; init; } = string.Empty;
    public string MedicineGenericName { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string Dosage { get; init; } = string.Empty;
    public string Frequency { get; init; } = string.Empty;
    public int DurationDays { get; init; }
    public string Instructions { get; init; } = string.Empty;
    public decimal QuantityDispensed { get; init; }
    public bool IsDispensed { get; init; }
}