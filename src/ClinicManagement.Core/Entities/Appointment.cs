namespace ClinicManagement.Core.Entities;

public class Appointment : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public Guid StaffId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = "Scheduled";
    public string? Notes { get; set; }

    public Clinic Clinic { get; set; } = null!;

    public Patient Patient { get; set; } = null!;
    public Staff Staff { get; set; } = null!;
    public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    public TreatmentHistory? TreatmentHistory { get; set; }
    public Bill? Bill { get; set; }
    public RoomBooking? RoomBooking { get; set; }
    public PatientQueue? PatientQueue { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}