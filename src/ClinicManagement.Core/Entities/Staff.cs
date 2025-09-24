using ClinicManagement.Core.Enums;

namespace ClinicManagement.Core.Entities;

public class Staff : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Receptionist;
    public bool IsActive { get; set; } = true;

    public ICollection<StaffClinic> StaffClinics { get; set; } = new List<StaffClinic>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}