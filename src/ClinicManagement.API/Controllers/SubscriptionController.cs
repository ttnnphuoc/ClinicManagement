using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.Constants;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IClinicContext _clinicContext;

    public SubscriptionController(ISubscriptionService subscriptionService, IClinicContext clinicContext)
    {
        _subscriptionService = subscriptionService;
        _clinicContext = clinicContext;
    }

    [HttpGet("packages")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPackages()
    {
        var packages = await _subscriptionService.GetAvailablePackagesAsync();
        var response = packages.Select(MapToPackageResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentSubscription()
    {
        if (!_clinicContext.CurrentUserId.HasValue)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Auth.Unauthorized));
        }

        var subscription = await _subscriptionService.GetActiveSubscriptionAsync(_clinicContext.CurrentUserId.Value);
        if (subscription == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Subscription.NoActiveSubscription));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToSubscriptionResponse(subscription)));
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request)
    {
        if (!_clinicContext.CurrentUserId.HasValue)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Auth.Unauthorized));
        }

        var (success, errorCode, subscription) = await _subscriptionService.CreateSubscriptionAsync(
            _clinicContext.CurrentUserId.Value, 
            request.PackageId);

        if (!success)
        {
            var responseCode = errorCode switch
            {
                "SUBSCRIPTION_EXISTS" => ResponseCodes.Subscription.SubscriptionExists,
                "PACKAGE_NOT_FOUND" => ResponseCodes.Subscription.PackageNotFound,
                _ => ResponseCodes.Common.BadRequest
            };
            return BadRequest(ApiResponse.ErrorResponse(responseCode));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToSubscriptionResponse(subscription!)));
    }

    [HttpPut("upgrade")]
    public async Task<IActionResult> UpgradeSubscription([FromBody] UpgradeSubscriptionRequest request)
    {
        if (!_clinicContext.CurrentUserId.HasValue)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Auth.Unauthorized));
        }

        var (success, errorCode, subscription) = await _subscriptionService.UpgradeSubscriptionAsync(
            _clinicContext.CurrentUserId.Value, 
            request.NewPackageId);

        if (!success)
        {
            var responseCode = errorCode switch
            {
                "NO_ACTIVE_SUBSCRIPTION" => ResponseCodes.Subscription.NoActiveSubscription,
                "PACKAGE_NOT_FOUND" => ResponseCodes.Subscription.PackageNotFound,
                "INVALID_UPGRADE" => ResponseCodes.Subscription.InvalidUpgrade,
                _ => ResponseCodes.Common.BadRequest
            };
            return BadRequest(ApiResponse.ErrorResponse(responseCode));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToSubscriptionResponse(subscription!)));
    }

    [HttpDelete("cancel")]
    public async Task<IActionResult> CancelSubscription()
    {
        if (!_clinicContext.CurrentUserId.HasValue)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Auth.Unauthorized));
        }

        var (success, errorCode) = await _subscriptionService.CancelSubscriptionAsync(_clinicContext.CurrentUserId.Value);

        if (!success)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Subscription.NoActiveSubscription));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
    }

    [HttpGet("usage")]
    public async Task<IActionResult> GetUsageTracking()
    {
        if (!_clinicContext.CurrentUserId.HasValue)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Auth.Unauthorized));
        }

        var subscription = await _subscriptionService.GetActiveSubscriptionAsync(_clinicContext.CurrentUserId.Value);
        if (subscription == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Subscription.NoActiveSubscription));
        }

        var resourceTypes = new[] { "Clinics", "Patients", "Staff", "Appointments" };
        var usage = new List<UsageTrackingResponse>();

        foreach (var resourceType in resourceTypes)
        {
            var tracking = await _subscriptionService.GetUsageTrackingAsync(_clinicContext.CurrentUserId.Value, resourceType);
            var limit = subscription.SubscriptionPackage.PackageLimits
                .FirstOrDefault(pl => pl.LimitType == resourceType)?.LimitValue;

            usage.Add(new UsageTrackingResponse
            {
                ResourceType = resourceType,
                CurrentUsage = tracking?.CurrentUsage ?? 0,
                Limit = limit == -1 ? null : limit,
                LastUpdated = tracking?.LastUpdated ?? DateTime.UtcNow
            });
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, usage));
    }

    private static SubscriptionPackageResponse MapToPackageResponse(SubscriptionPackage package)
    {
        return new SubscriptionPackageResponse
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Price = package.Price,
            DurationInDays = package.DurationInDays,
            IsTrialPackage = package.IsTrialPackage,
            PackageLimits = package.PackageLimits.Select(pl => new PackageLimitResponse
            {
                LimitType = pl.LimitType,
                LimitValue = pl.LimitValue,
                DisplayText = FormatLimitDisplay(pl.LimitType, pl.LimitValue)
            }).ToList()
        };
    }

    private static SubscriptionResponse MapToSubscriptionResponse(Subscription subscription)
    {
        return new SubscriptionResponse
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            SubscriptionPackage = MapToPackageResponse(subscription.SubscriptionPackage),
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            Status = subscription.Status,
            AutoRenew = subscription.AutoRenew,
            LastPaymentDate = subscription.LastPaymentDate,
            UsageTrackings = subscription.UsageTrackings.Select(ut => new UsageTrackingResponse
            {
                ResourceType = ut.ResourceType,
                CurrentUsage = ut.CurrentUsage,
                LastUpdated = ut.LastUpdated
            }).ToList()
        };
    }

    private static string FormatLimitDisplay(string limitType, int limitValue)
    {
        if (limitValue == -1)
        {
            return "Unlimited";
        }

        return limitType switch
        {
            "Clinics" => $"Up to {limitValue} clinic{(limitValue == 1 ? "" : "s")}",
            "Patients" => $"Up to {limitValue} patient{(limitValue == 1 ? "" : "s")}",
            "Staff" => $"Up to {limitValue} staff member{(limitValue == 1 ? "" : "s")}",
            "Appointments" => $"Up to {limitValue} appointment{(limitValue == 1 ? "" : "s")} per month",
            _ => $"{limitValue} {limitType.ToLower()}"
        };
    }
}