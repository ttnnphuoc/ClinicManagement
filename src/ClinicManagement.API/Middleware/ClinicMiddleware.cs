using System.Security.Claims;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Middleware;

public class ClinicMiddleware
{
    private readonly RequestDelegate _next;

    public ClinicMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IClinicContext clinicContext, IStaffRepository staffRepository, IClinicRepository clinicRepository)
    {
        var logger = context.RequestServices.GetService<ILogger<ClinicMiddleware>>();
        
        var clinicIdClaim = context.User.FindFirst("ClinicId")?.Value;
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                          context.User.FindFirst("sub")?.Value;
        var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;

        logger?.LogInformation("ClinicMiddleware - User: {UserId}, Role: {Role}, ClinicId: {ClinicId}", 
            userIdClaim, roleClaim, clinicIdClaim);

        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
        {
            clinicContext.SetUserId(userId);
        }

        if (!string.IsNullOrEmpty(roleClaim))
        {
            clinicContext.SetUserRole(roleClaim);
        }

        if (!string.IsNullOrEmpty(clinicIdClaim) && Guid.TryParse(clinicIdClaim, out var clinicId))
        {
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId2))
            {
                var hasAccess = await staffRepository.HasAccessToClinicAsync(userId2, clinicId);
                
                if (!hasAccess)
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        success = false, 
                        code = "AUTH_CLINIC_ACCESS_DENIED",
                        message = "You don't have permission to access this clinic" 
                    });
                    return;
                }
            }

            clinicContext.SetClinicId(clinicId);
            logger?.LogInformation("ClinicMiddleware - Set clinic context: {ClinicId}", clinicId);
        }
        else if (roleClaim == "SuperAdmin" && !string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var superAdminUserId))
        {
            // For SuperAdmin without clinic context, auto-select first available clinic
            var staffWithClinics = await staffRepository.GetByIdWithClinicsAsync(superAdminUserId);
            if (staffWithClinics?.StaffClinics?.Any() == true)
            {
                var firstClinicId = staffWithClinics.StaffClinics.First().ClinicId;
                clinicContext.SetClinicId(firstClinicId);
                logger?.LogInformation("ClinicMiddleware - SuperAdmin auto-selected clinic: {ClinicId}", firstClinicId);
            }
            else
            {
                logger?.LogWarning("ClinicMiddleware - SuperAdmin {UserId} has no clinic associations. Trying to get any available clinic.", superAdminUserId);
                
                // If SuperAdmin has no specific clinic associations, just use the first available clinic
                // This is a fallback for SuperAdmin who should have access to all clinics
                var allClinics = await clinicRepository.GetAllAsync();
                var firstClinic = allClinics.FirstOrDefault();
                if (firstClinic != null)
                {
                    clinicContext.SetClinicId(firstClinic.Id);
                    logger?.LogInformation("ClinicMiddleware - SuperAdmin fallback to first available clinic: {ClinicId}", firstClinic.Id);
                }
                else
                {
                    logger?.LogError("ClinicMiddleware - No clinics available in the system for SuperAdmin");
                }
            }
        }
        else if (!string.IsNullOrEmpty(userIdClaim) && string.IsNullOrEmpty(clinicIdClaim))
        {
            // Non-SuperAdmin user without clinic context - try to find their first clinic
            if (Guid.TryParse(userIdClaim, out var regularUserId))
            {
                var staffWithClinics = await staffRepository.GetByIdWithClinicsAsync(regularUserId);
                if (staffWithClinics?.StaffClinics?.Any() == true)
                {
                    var firstClinicId = staffWithClinics.StaffClinics.First().ClinicId;
                    clinicContext.SetClinicId(firstClinicId);
                    logger?.LogInformation("ClinicMiddleware - Non-SuperAdmin user auto-selected clinic: {ClinicId}", firstClinicId);
                }
                else
                {
                    logger?.LogWarning("ClinicMiddleware - User {UserId} has no clinic associations", regularUserId);
                }
            }
        }

        await _next(context);
    }
}