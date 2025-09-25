namespace ClinicManagement.Core.Entities;

public class SubscriptionPackage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationInDays { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsTrialPackage { get; set; } = false;

    public ICollection<PackageLimit> PackageLimits { get; set; } = new List<PackageLimit>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}