using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionPackageRepository _packageRepository;
    private readonly IUsageTrackingRepository _usageRepository;

    public SubscriptionService(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionPackageRepository packageRepository,
        IUsageTrackingRepository usageRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _packageRepository = packageRepository;
        _usageRepository = usageRepository;
    }

    public async Task<(bool Success, string? ErrorCode, Subscription? Subscription)> CreateSubscriptionAsync(Guid userId, Guid packageId)
    {
        var existingSubscription = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
        if (existingSubscription != null)
        {
            return (false, "SUBSCRIPTION_EXISTS", null);
        }

        var package = await _packageRepository.GetPackageWithLimitsAsync(packageId);
        if (package == null)
        {
            return (false, "PACKAGE_NOT_FOUND", null);
        }

        var subscription = new Subscription
        {
            UserId = userId,
            SubscriptionPackageId = packageId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(package.DurationInDays),
            IsActive = true,
            Status = "Active",
            AutoRenew = true
        };

        await _subscriptionRepository.AddAsync(subscription);
        await _subscriptionRepository.SaveChangesAsync();

        await InitializeUsageTrackingAsync(subscription.Id, package);

        return (true, null, subscription);
    }

    public async Task<(bool Success, string? ErrorCode, Subscription? Subscription)> UpgradeSubscriptionAsync(Guid userId, Guid newPackageId)
    {
        var currentSubscription = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
        if (currentSubscription == null)
        {
            return (false, "NO_ACTIVE_SUBSCRIPTION", null);
        }

        var newPackage = await _packageRepository.GetPackageWithLimitsAsync(newPackageId);
        if (newPackage == null)
        {
            return (false, "PACKAGE_NOT_FOUND", null);
        }

        if (newPackage.Price <= currentSubscription.SubscriptionPackage.Price)
        {
            return (false, "INVALID_UPGRADE", null);
        }

        currentSubscription.Status = "Upgraded";
        currentSubscription.IsActive = false;
        await _subscriptionRepository.UpdateAsync(currentSubscription);

        var newSubscription = new Subscription
        {
            UserId = userId,
            SubscriptionPackageId = newPackageId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(newPackage.DurationInDays),
            IsActive = true,
            Status = "Active",
            AutoRenew = currentSubscription.AutoRenew
        };

        await _subscriptionRepository.AddAsync(newSubscription);
        await _subscriptionRepository.SaveChangesAsync();

        await InitializeUsageTrackingAsync(newSubscription.Id, newPackage);

        return (true, null, newSubscription);
    }

    public async Task<(bool Success, string? ErrorCode)> CancelSubscriptionAsync(Guid userId)
    {
        var subscription = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
        if (subscription == null)
        {
            return (false, "NO_ACTIVE_SUBSCRIPTION");
        }

        subscription.Status = "Cancelled";
        subscription.AutoRenew = false;
        await _subscriptionRepository.UpdateAsync(subscription);
        await _subscriptionRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<Subscription?> GetActiveSubscriptionAsync(Guid userId)
    {
        return await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
    }

    public async Task<IEnumerable<SubscriptionPackage>> GetAvailablePackagesAsync()
    {
        return await _packageRepository.GetActivePackagesAsync();
    }

    public async Task<SubscriptionPackage?> GetPackageByIdAsync(Guid packageId)
    {
        return await _packageRepository.GetPackageWithLimitsAsync(packageId);
    }

    public async Task<bool> ValidateUsageLimitAsync(Guid userId, string resourceType, int requestedAmount = 1)
    {
        var subscription = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
        if (subscription == null)
        {
            return false;
        }

        var limit = subscription.SubscriptionPackage.PackageLimits
            .FirstOrDefault(pl => pl.LimitType == resourceType && pl.IsActive);

        if (limit == null)
        {
            return true;
        }

        if (limit.LimitValue == -1)
        {
            return true;
        }

        var usage = await _usageRepository.GetUsageBySubscriptionAndTypeAsync(subscription.Id, resourceType);
        var currentUsage = usage?.CurrentUsage ?? 0;

        return currentUsage + requestedAmount <= limit.LimitValue;
    }

    public async Task<UsageTracking?> GetUsageTrackingAsync(Guid userId, string resourceType)
    {
        var subscription = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
        if (subscription == null)
        {
            return null;
        }

        return await _usageRepository.GetUsageBySubscriptionAndTypeAsync(subscription.Id, resourceType);
    }

    public async Task UpdateUsageAsync(Guid userId, string resourceType, int amount)
    {
        var subscription = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
        if (subscription == null)
        {
            return;
        }

        var usage = await _usageRepository.GetUsageBySubscriptionAndTypeAsync(subscription.Id, resourceType);
        if (usage == null)
        {
            usage = new UsageTracking
            {
                SubscriptionId = subscription.Id,
                ResourceType = resourceType,
                CurrentUsage = amount,
                LastUpdated = DateTime.UtcNow
            };
            await _usageRepository.AddAsync(usage);
        }
        else
        {
            usage.CurrentUsage += amount;
            usage.LastUpdated = DateTime.UtcNow;
            await _usageRepository.UpdateAsync(usage);
        }

        await _usageRepository.SaveChangesAsync();
    }

    public async Task<(bool Success, string? ErrorCode)> ProcessSubscriptionRenewalAsync(Guid subscriptionId)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId);
        if (subscription == null)
        {
            return (false, "SUBSCRIPTION_NOT_FOUND");
        }

        if (!subscription.AutoRenew || subscription.Status != "Active")
        {
            return (false, "RENEWAL_NOT_ALLOWED");
        }

        var package = await _packageRepository.GetByIdAsync(subscription.SubscriptionPackageId);
        if (package == null)
        {
            return (false, "PACKAGE_NOT_FOUND");
        }

        subscription.StartDate = subscription.EndDate;
        subscription.EndDate = subscription.EndDate.AddDays(package.DurationInDays);
        subscription.LastPaymentDate = DateTime.UtcNow;

        await _subscriptionRepository.UpdateAsync(subscription);
        await _subscriptionRepository.SaveChangesAsync();

        var usageTrackings = await _usageRepository.GetUsageBySubscriptionIdAsync(subscriptionId);
        foreach (var usage in usageTrackings)
        {
            usage.CurrentUsage = 0;
            usage.LastUpdated = DateTime.UtcNow;
            await _usageRepository.UpdateAsync(usage);
        }
        await _usageRepository.SaveChangesAsync();

        return (true, null);
    }

    private async Task InitializeUsageTrackingAsync(Guid subscriptionId, SubscriptionPackage package)
    {
        var resourceTypes = new[] { "Clinics", "Patients", "Staff", "Appointments" };
        
        foreach (var resourceType in resourceTypes)
        {
            var usage = new UsageTracking
            {
                SubscriptionId = subscriptionId,
                ResourceType = resourceType,
                CurrentUsage = 0,
                LastUpdated = DateTime.UtcNow
            };
            
            await _usageRepository.AddAsync(usage);
        }
        
        await _usageRepository.SaveChangesAsync();
    }
}