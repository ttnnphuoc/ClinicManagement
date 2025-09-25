namespace ClinicManagement.Core.Entities;

public class TreatmentHistory : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public Guid StaffId { get; set; }
    public DateTime TreatmentDate { get; set; }
    
    // Chief Complaint & Symptoms
    public string? ChiefComplaint { get; set; }
    public string? Symptoms { get; set; }
    
    // Vital Signs
    public string? BloodPressure { get; set; }
    public decimal? Temperature { get; set; }
    public int? HeartRate { get; set; }
    public int? RespiratoryRate { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    
    // Examination
    public string? PhysicalExamination { get; set; }
    public string? Diagnosis { get; set; }
    public string? DifferentialDiagnosis { get; set; }
    
    // Treatment
    public string Treatment { get; set; } = string.Empty;
    public string? PrescriptionNotes { get; set; }
    public string? TreatmentPlan { get; set; }
    
    // Follow-up
    public string? FollowUpInstructions { get; set; }
    public DateTime? NextAppointmentDate { get; set; }
    
    // Notes
    public string? Notes { get; set; }

    // Navigation Properties
    public Clinic Clinic { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public Appointment? Appointment { get; set; }
    public Staff Staff { get; set; } = null!;
    public Prescription? Prescription { get; set; }
}