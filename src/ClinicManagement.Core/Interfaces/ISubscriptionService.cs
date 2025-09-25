using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface ISubscriptionService
{
    Task<(bool Success, string? ErrorCode, Subscription? Subscription)> CreateSubscriptionAsync(Guid userId, Guid packageId);
    Task<(bool Success, string? ErrorCode, Subscription? Subscription)> UpgradeSubscriptionAsync(Guid userId, Guid newPackageId);
    Task<(bool Success, string? ErrorCode)> CancelSubscriptionAsync(Guid userId);
    Task<Subscription?> GetActiveSubscriptionAsync(Guid userId);
    Task<IEnumerable<SubscriptionPackage>> GetAvailablePackagesAsync();
    Task<SubscriptionPackage?> GetPackageByIdAsync(Guid packageId);
    Task<bool> ValidateUsageLimitAsync(Guid userId, string resourceType, int requestedAmount = 1);
    Task<UsageTracking?> GetUsageTrackingAsync(Guid userId, string resourceType);
    Task UpdateUsageAsync(Guid userId, string resourceType, int amount);
    Task<(bool Success, string? ErrorCode)> ProcessSubscriptionRenewalAsync(Guid subscriptionId);
}