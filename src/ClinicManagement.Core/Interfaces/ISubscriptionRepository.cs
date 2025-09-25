using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription?> GetActiveSubscriptionByUserIdAsync(Guid userId);
    Task<IEnumerable<Subscription>> GetExpiringSubscriptionsAsync(DateTime expirationDate);
}

public interface ISubscriptionPackageRepository : IRepository<SubscriptionPackage>
{
    Task<IEnumerable<SubscriptionPackage>> GetActivePackagesAsync();
    Task<SubscriptionPackage?> GetPackageWithLimitsAsync(Guid packageId);
}

public interface IUsageTrackingRepository : IRepository<UsageTracking>
{
    Task<UsageTracking?> GetUsageBySubscriptionAndTypeAsync(Guid subscriptionId, string resourceType);
    Task<IEnumerable<UsageTracking>> GetUsageBySubscriptionIdAsync(Guid subscriptionId);
}