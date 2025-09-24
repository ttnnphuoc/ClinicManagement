namespace ClinicManagement.Core.Entities;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid StaffId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = "Scheduled";
    public string? Notes { get; set; }

    public Patient Patient { get; set; } = null!;
    public Staff Staff { get; set; } = null!;
    public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
}