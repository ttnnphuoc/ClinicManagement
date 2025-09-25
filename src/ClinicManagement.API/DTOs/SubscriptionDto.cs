namespace ClinicManagement.API.DTOs;

public record CreateSubscriptionRequest
{
    public Guid PackageId { get; init; }
}

public record UpgradeSubscriptionRequest
{
    public Guid NewPackageId { get; init; }
}

public record SubscriptionPackageResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int DurationInDays { get; init; }
    public bool IsTrialPackage { get; init; }
    public List<PackageLimitResponse> PackageLimits { get; init; } = new();
}

public record PackageLimitResponse
{
    public string LimitType { get; init; } = string.Empty;
    public int LimitValue { get; init; }
    public string DisplayText { get; init; } = string.Empty;
}

public record SubscriptionResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public SubscriptionPackageResponse SubscriptionPackage { get; init; } = null!;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public bool AutoRenew { get; init; }
    public DateTime? LastPaymentDate { get; init; }
    public List<UsageTrackingResponse> UsageTrackings { get; init; } = new();
}

public record UsageTrackingResponse
{
    public string ResourceType { get; init; } = string.Empty;
    public int CurrentUsage { get; init; }
    public int? Limit { get; init; }
    public DateTime LastUpdated { get; init; }
}

public record CreateClinicWithPackageRequest
{
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public bool IsActive { get; init; } = true;
    public Guid PackageId { get; init; }
}