namespace ClinicManagement.Core.Entities;

public class PackageLimit : BaseEntity
{
    public Guid SubscriptionPackageId { get; set; }
    public string LimitType { get; set; } = string.Empty; // e.g., "MaxClinics", "MaxPatients", "MaxStaff"
    public int LimitValue { get; set; } // -1 for unlimited
    public bool IsActive { get; set; } = true;

    public SubscriptionPackage SubscriptionPackage { get; set; } = null!;
}