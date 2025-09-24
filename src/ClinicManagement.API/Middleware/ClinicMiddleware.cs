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

    public async Task InvokeAsync(HttpContext context, IClinicContext clinicContext, IStaffRepository staffRepository)
    {
        var clinicIdClaim = context.User.FindFirst("ClinicId")?.Value;
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                          context.User.FindFirst("sub")?.Value;

        if (!string.IsNullOrEmpty(clinicIdClaim) && Guid.TryParse(clinicIdClaim, out var clinicId))
        {
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
            {
                var hasAccess = await staffRepository.HasAccessToClinicAsync(userId, clinicId);
                
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
        }

        await _next(context);
    }
}