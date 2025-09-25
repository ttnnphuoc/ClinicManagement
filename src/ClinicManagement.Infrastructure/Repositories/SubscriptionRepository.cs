using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Repositories;

public class SubscriptionRepository : Repository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(ApplicationDbContext context, IClinicContext clinicContext) : base(context, clinicContext)
    {
    }

    public async Task<Subscription?> GetActiveSubscriptionByUserIdAsync(Guid userId)
    {
        return await _context.Subscriptions
            .Include(s => s.SubscriptionPackage)
            .ThenInclude(sp => sp.PackageLimits)
            .Include(s => s.UsageTrackings)
            .FirstOrDefaultAsync(s => s.UserId == userId && 
                                     s.IsActive && 
                                     s.Status == "Active" && 
                                     s.EndDate > DateTime.UtcNow);
    }

    public async Task<IEnumerable<Subscription>> GetExpiringSubscriptionsAsync(DateTime expirationDate)
    {
        return await _context.Subscriptions
            .Where(s => s.IsActive && 
                       s.EndDate <= expirationDate && 
                       s.Status == "Active")
            .ToListAsync();
    }
}

public class SubscriptionPackageRepository : Repository<SubscriptionPackage>, ISubscriptionPackageRepository
{
    public SubscriptionPackageRepository(ApplicationDbContext context, IClinicContext clinicContext) : base(context, clinicContext)
    {
    }

    public async Task<IEnumerable<SubscriptionPackage>> GetActivePackagesAsync()
    {
        return await _context.SubscriptionPackages
            .Where(sp => sp.IsActive)
            .Include(sp => sp.PackageLimits)
            .OrderBy(sp => sp.Price)
            .ToListAsync();
    }

    public async Task<SubscriptionPackage?> GetPackageWithLimitsAsync(Guid packageId)
    {
        return await _context.SubscriptionPackages
            .Include(sp => sp.PackageLimits)
            .FirstOrDefaultAsync(sp => sp.Id == packageId && sp.IsActive);
    }
}

public class UsageTrackingRepository : Repository<UsageTracking>, IUsageTrackingRepository
{
    public UsageTrackingRepository(ApplicationDbContext context, IClinicContext clinicContext) : base(context, clinicContext)
    {
    }

    public async Task<UsageTracking?> GetUsageBySubscriptionAndTypeAsync(Guid subscriptionId, string resourceType)
    {
        return await _context.UsageTrackings
            .FirstOrDefaultAsync(ut => ut.SubscriptionId == subscriptionId && 
                                      ut.ResourceType == resourceType);
    }

    public async Task<IEnumerable<UsageTracking>> GetUsageBySubscriptionIdAsync(Guid subscriptionId)
    {
        return await _context.UsageTrackings
            .Where(ut => ut.SubscriptionId == subscriptionId)
            .ToListAsync();
    }
}