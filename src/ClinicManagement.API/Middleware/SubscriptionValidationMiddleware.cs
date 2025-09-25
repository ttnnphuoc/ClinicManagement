using ClinicManagement.Core.Interfaces;
using ClinicManagement.API.Constants;
using ClinicManagement.API.DTOs;
using System.Text.Json;

namespace ClinicManagement.API.Middleware;

public class SubscriptionValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<SubscriptionValidationMiddleware> _logger;

    private static readonly Dictionary<string, string> ResourceTypeMapping = new()
    {
        { "/api/clinics", "Clinics" },
        { "/api/patients", "Patients" },
        { "/api/staff", "Staff" },
        { "/api/appointments", "Appointments" }
    };

    public SubscriptionValidationMiddleware(
        RequestDelegate next, 
        IServiceScopeFactory serviceScopeFactory,
        ILogger<SubscriptionValidationMiddleware> logger)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldSkipValidation(context))
        {
            await _next(context);
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
        var clinicContext = scope.ServiceProvider.GetRequiredService<IClinicContext>();

        if (!clinicContext.CurrentUserId.HasValue)
        {
            await _next(context);
            return;
        }

        var subscription = await subscriptionService.GetActiveSubscriptionAsync(clinicContext.CurrentUserId.Value);
        if (subscription == null)
        {
            await WriteErrorResponse(context, ResponseCodes.Subscription.NoActiveSubscription, "No active subscription found");
            return;
        }

        if (subscription.EndDate <= DateTime.UtcNow)
        {
            await WriteErrorResponse(context, ResponseCodes.Subscription.SubscriptionExpired, "Subscription has expired");
            return;
        }

        var resourceType = GetResourceTypeFromPath(context.Request.Path);
        if (!string.IsNullOrEmpty(resourceType) && IsCreateRequest(context))
        {
            var canProceed = await subscriptionService.ValidateUsageLimitAsync(clinicContext.CurrentUserId.Value, resourceType);
            if (!canProceed)
            {
                await WriteErrorResponse(context, ResponseCodes.Subscription.LimitExceeded, $"{resourceType} limit exceeded for your current package");
                return;
            }
        }

        await _next(context);

        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300 && 
            !string.IsNullOrEmpty(resourceType) && IsCreateRequest(context))
        {
            try
            {
                await subscriptionService.UpdateUsageAsync(clinicContext.CurrentUserId.Value, resourceType, 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update usage tracking for resource type {ResourceType}", resourceType);
            }
        }
    }

    private static bool ShouldSkipValidation(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();
        return path == null ||
               path.StartsWith("/api/auth") ||
               path.StartsWith("/api/subscription") ||
               path.StartsWith("/api/packages") ||
               context.Request.Method == "GET" ||
               context.Request.Method == "OPTIONS";
    }

    private static string? GetResourceTypeFromPath(PathString path)
    {
        var pathValue = path.Value?.ToLower();
        if (pathValue == null) return null;

        foreach (var mapping in ResourceTypeMapping)
        {
            if (pathValue.StartsWith(mapping.Key))
            {
                return mapping.Value;
            }
        }

        return null;
    }

    private static bool IsCreateRequest(HttpContext context)
    {
        return context.Request.Method == "POST";
    }

    private static async Task WriteErrorResponse(HttpContext context, string errorCode, string message)
    {
        context.Response.StatusCode = 403;
        context.Response.ContentType = "application/json";

        var response = ApiResponse.ErrorResponse(errorCode, message);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}