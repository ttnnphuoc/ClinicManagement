namespace ClinicManagement.Core.Entities;

public class UsageTracking : BaseEntity
{
    public Guid SubscriptionId { get; set; }
    public string ResourceType { get; set; } = string.Empty; // e.g., "Clinics", "Patients", "Staff", "Appointments"
    public int CurrentUsage { get; set; }
    public DateTime LastUpdated { get; set; }

    public Subscription Subscription { get; set; } = null!;
}