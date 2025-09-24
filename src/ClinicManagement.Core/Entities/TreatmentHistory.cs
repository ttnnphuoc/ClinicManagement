namespace ClinicManagement.Core.Entities;

public class TreatmentHistory : BaseEntity
{
    public Guid PatientId { get; set; }
    public DateTime TreatmentDate { get; set; }
    public string Treatment { get; set; } = string.Empty;
    public string? Diagnosis { get; set; }
    public string? Prescription { get; set; }
    public string? Notes { get; set; }

    public Patient Patient { get; set; } = null!;
}