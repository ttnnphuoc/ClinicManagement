namespace ClinicManagement.Core.Entities;

public class Patient : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public string PatientCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? Gender { get; set; }
    
    public string? Allergies { get; set; }
    public string? ChronicConditions { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    
    public string? BloodType { get; set; }
    public string? IdNumber { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? Occupation { get; set; }
    public string? ReferralSource { get; set; }
    public DateTime? FirstVisitDate { get; set; }
    public bool ReceivePromotions { get; set; } = false;
    
    public string? Notes { get; set; }

    public Clinic Clinic { get; set; } = null!;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<TreatmentHistory> TreatmentHistories { get; set; } = new List<TreatmentHistory>();
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public ICollection<PatientQueue> PatientQueues { get; set; } = new List<PatientQueue>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}