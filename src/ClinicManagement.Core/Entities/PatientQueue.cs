namespace ClinicManagement.Core.Entities;

public class PatientQueue : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public string QueueNumber { get; set; } = string.Empty;
    public DateTime QueueDate { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CalledTime { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? CompletionTime { get; set; }
    public string Status { get; set; } = "Waiting"; // Waiting, Called, InProgress, Completed, NoShow, Cancelled
    public string QueueType { get; set; } = "Appointment"; // Appointment, WalkIn, Emergency
    public int Priority { get; set; } = 0; // 0=Normal, 1=High, 2=Emergency
    public Guid? AssignedStaffId { get; set; }
    public Guid? RoomId { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public Appointment? Appointment { get; set; }
    public Staff? AssignedStaff { get; set; }
    public Room? Room { get; set; }
}