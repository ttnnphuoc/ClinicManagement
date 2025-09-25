namespace ClinicManagement.Core.Entities;

public class Room : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty; // Consultation, Procedure, Laboratory, Pharmacy, etc.
    public int Capacity { get; set; } = 1;
    public string? Equipment { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal? HourlyRate { get; set; }

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
    public ICollection<RoomBooking> RoomBookings { get; set; } = new List<RoomBooking>();
}