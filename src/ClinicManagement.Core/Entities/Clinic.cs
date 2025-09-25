namespace ClinicManagement.Core.Entities;

public class Clinic : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? OwnerId { get; set; }

    public Staff? Owner { get; set; }
    public ICollection<StaffClinic> StaffClinics { get; set; } = new List<StaffClinic>();
    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}