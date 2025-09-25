namespace ClinicManagement.Core.Entities;

public class RoomBooking : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public Guid RoomId { get; set; }
    public Guid? AppointmentId { get; set; }
    public Guid BookedByStaffId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string Status { get; set; } = "Booked"; // Booked, InUse, Completed, Cancelled
    public string? Notes { get; set; }

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
    public Room Room { get; set; } = null!;
    public Appointment? Appointment { get; set; }
    public Staff BookedByStaff { get; set; } = null!;
}