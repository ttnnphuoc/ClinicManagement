namespace ClinicManagement.API.DTOs;

public record CreateTreatmentHistoryRequest
{
    public Guid PatientId { get; init; }
    public Guid? AppointmentId { get; init; }
    public DateTime TreatmentDate { get; init; }
    public string? ChiefComplaint { get; init; }
    public string? Symptoms { get; init; }
    public string? BloodPressure { get; init; }
    public decimal? Temperature { get; init; }
    public int? HeartRate { get; init; }
    public int? RespiratoryRate { get; init; }
    public decimal? Weight { get; init; }
    public decimal? Height { get; init; }
    public string? PhysicalExamination { get; init; }
    public string? Diagnosis { get; init; }
    public string? DifferentialDiagnosis { get; init; }
    public string Treatment { get; init; } = string.Empty;
    public string? Prescription { get; init; }
    public string? TreatmentPlan { get; init; }
    public string? FollowUpInstructions { get; init; }
    public DateTime? NextAppointmentDate { get; init; }
    public string? Notes { get; init; }
}

public record UpdateTreatmentHistoryRequest
{
    public string? ChiefComplaint { get; init; }
    public string? Symptoms { get; init; }
    public string? BloodPressure { get; init; }
    public decimal? Temperature { get; init; }
    public int? HeartRate { get; init; }
    public int? RespiratoryRate { get; init; }
    public decimal? Weight { get; init; }
    public decimal? Height { get; init; }
    public string? PhysicalExamination { get; init; }
    public string? Diagnosis { get; init; }
    public string? DifferentialDiagnosis { get; init; }
    public string Treatment { get; init; } = string.Empty;
    public string? Prescription { get; init; }
    public string? TreatmentPlan { get; init; }
    public string? FollowUpInstructions { get; init; }
    public DateTime? NextAppointmentDate { get; init; }
    public string? Notes { get; init; }
}

public record TreatmentHistoryResponse
{
    public Guid Id { get; init; }
    public Guid ClinicId { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public Guid? AppointmentId { get; init; }
    public Guid StaffId { get; init; }
    public string StaffName { get; init; } = string.Empty;
    public DateTime TreatmentDate { get; init; }
    public string? ChiefComplaint { get; init; }
    public string? Symptoms { get; init; }
    public string? BloodPressure { get; init; }
    public decimal? Temperature { get; init; }
    public int? HeartRate { get; init; }
    public int? RespiratoryRate { get; init; }
    public decimal? Weight { get; init; }
    public decimal? Height { get; init; }
    public string? PhysicalExamination { get; init; }
    public string? Diagnosis { get; init; }
    public string? DifferentialDiagnosis { get; init; }
    public string Treatment { get; init; } = string.Empty;
    public string? Prescription { get; init; }
    public string? TreatmentPlan { get; init; }
    public string? FollowUpInstructions { get; init; }
    public DateTime? NextAppointmentDate { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
}