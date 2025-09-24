namespace ClinicManagement.Core.Entities;

public class AppointmentService
{
    public Guid AppointmentId { get; set; }
    public Guid ServiceId { get; set; }
    public decimal Price { get; set; }

    public Appointment Appointment { get; set; } = null!;
    public Service Service { get; set; } = null!;
}