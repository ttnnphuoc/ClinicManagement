namespace ClinicManagement.Core.Entities;

public class StaffClinic : BaseEntity
{
    public Guid StaffId { get; set; }
    public Guid ClinicId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

    public Staff Staff { get; set; } = null!;
    public Clinic Clinic { get; set; } = null!;
}