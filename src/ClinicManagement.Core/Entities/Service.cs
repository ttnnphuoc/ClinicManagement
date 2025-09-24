namespace ClinicManagement.Core.Entities;

public class Service : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsActive { get; set; } = true;

    public Clinic Clinic { get; set; } = null!;

    public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
}