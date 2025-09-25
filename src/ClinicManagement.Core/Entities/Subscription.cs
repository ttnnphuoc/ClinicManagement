namespace ClinicManagement.Core.Entities;

public class Subscription : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid SubscriptionPackageId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool AutoRenew { get; set; } = true;
    public string Status { get; set; } = "Active"; // Active, Expired, Cancelled, PendingPayment
    public string? PaymentId { get; set; }
    public DateTime? LastPaymentDate { get; set; }

    public Staff User { get; set; } = null!;
    public SubscriptionPackage SubscriptionPackage { get; set; } = null!;
    public ICollection<UsageTracking> UsageTrackings { get; set; } = new List<UsageTracking>();
}