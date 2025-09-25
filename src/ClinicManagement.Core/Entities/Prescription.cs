namespace ClinicManagement.Core.Entities;

public class Prescription : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public Guid TreatmentHistoryId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public string PrescriptionNumber { get; set; } = string.Empty;
    public DateTime PrescriptionDate { get; set; }
    public string Status { get; set; } = "Active"; // Active, Dispensed, Cancelled
    public string? Notes { get; set; }

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
    public TreatmentHistory TreatmentHistory { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public Staff Doctor { get; set; } = null!;
    public ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; } = new List<PrescriptionMedicine>();
}