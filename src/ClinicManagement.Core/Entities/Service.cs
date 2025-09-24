namespace ClinicManagement.Core.Entities;

public class Service : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
}