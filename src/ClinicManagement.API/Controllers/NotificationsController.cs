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
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IClinicContext _clinicContext;

    public NotificationsController(INotificationService notificationService, IClinicContext clinicContext)
    {
        _notificationService = notificationService;
        _clinicContext = clinicContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] bool? isRead, [FromQuery] string? type)
    {
        var userId = GetCurrentUserId();
        var notifications = await _notificationService.GetUserNotificationsAsync(userId, isRead, type);
        var response = notifications.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = GetCurrentUserId();
        var count = await _notificationService.GetUnreadCountAsync(userId);
        
        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, new { count }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotification(Guid id)
    {
        var notification = await _notificationService.GetNotificationAsync(id);
        if (notification == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        var response = MapToResponse(notification);
        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] NotificationDto notificationDto)
    {
        try
        {
            var notification = await MapToEntity(notificationDto);
            var createdNotification = await _notificationService.CreateNotificationAsync(notification);
            var response = MapToResponse(createdNotification);

            return CreatedAtAction(nameof(GetNotification), new { id = createdNotification.Id }, 
                ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    [HttpPut("{id}/mark-read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _notificationService.MarkAsReadAsync(id, userId);
            return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    [HttpPut("mark-all-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _notificationService.DeleteNotificationAsync(id, userId);
            return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    [HttpPost("send-system-notification")]
    [Authorize(Policy = Policies.ManageStaff)]
    public async Task<IActionResult> SendSystemNotification([FromBody] SystemNotificationRequest request)
    {
        try
        {
            await _notificationService.SendSystemNotificationAsync(
                request.Title, 
                request.Message, 
                request.Type, 
                request.Priority,
                request.UserIds,
                request.RoleFilter);

            return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    [HttpPost("send-appointment-reminder")]
    public async Task<IActionResult> SendAppointmentReminder([FromBody] AppointmentReminderRequest request)
    {
        try
        {
            await _notificationService.SendAppointmentReminderAsync(request.AppointmentId);
            return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    private NotificationDto MapToResponse(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            Priority = notification.Priority,
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            CreatedAt = notification.CreatedAt,
            UserId = notification.UserId ?? Guid.Empty
        };
    }

    private async Task<Notification> MapToEntity(NotificationDto dto)
    {
        return new Notification
        {
            Id = dto.Id ?? Guid.NewGuid(),
            Title = dto.Title,
            Message = dto.Message,
            Type = dto.Type,
            Priority = dto.Priority ?? "medium",
            IsRead = false,
            UserId = dto.UserId,
            ClinicId = _clinicContext.CurrentClinicId ?? throw new InvalidOperationException("No clinic context available")
        };
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found"));
    }

    public class SystemNotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "system";
        public string Priority { get; set; } = "medium";
        public List<Guid>? UserIds { get; set; }
        public string? RoleFilter { get; set; }
    }

    public class AppointmentReminderRequest
    {
        public Guid AppointmentId { get; set; }
    }
}