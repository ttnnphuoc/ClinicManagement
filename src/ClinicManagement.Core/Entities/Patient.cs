namespace ClinicManagement.Core.Entities;

public class Patient : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? Gender { get; set; }
    public string? Notes { get; set; }

    public Clinic Clinic { get; set; } = null!;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<TreatmentHistory> TreatmentHistories { get; set; } = new List<TreatmentHistory>();
}