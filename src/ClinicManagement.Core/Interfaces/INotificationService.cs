using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface INotificationService
{
    Task<(bool Success, string? ErrorCode, Notification? Notification)> ScheduleNotificationAsync(
        string notificationType,
        string deliveryMethod,
        string recipient,
        string subject,
        string message,
        DateTime scheduledTime,
        Guid? patientId = null,
        Guid? appointmentId = null);

    Task<(bool Success, string? ErrorCode)> SendNotificationAsync(Guid notificationId);
    
    Task<(bool Success, string? ErrorCode)> SendAppointmentReminderAsync(Guid appointmentId, int hoursBeforeAppointment = 24);
    
    Task<(bool Success, string? ErrorCode)> SendPaymentReminderAsync(Guid billId);
    
    Task<(bool Success, string? ErrorCode)> ScheduleFollowUpReminderAsync(Guid treatmentHistoryId, DateTime followUpDate);
    
    Task<IEnumerable<Notification>> GetPendingNotificationsAsync();
    
    Task<IEnumerable<Notification>> GetPatientNotificationsAsync(Guid patientId);
    
    Task ProcessPendingNotificationsAsync();

    // Additional methods for user notification management
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool? isRead = null, string? type = null);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<Notification?> GetNotificationAsync(Guid id);
    Task<Notification> CreateNotificationAsync(Notification notification);
    Task MarkAsReadAsync(Guid notificationId, Guid userId);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteNotificationAsync(Guid notificationId, Guid userId);
    Task SendSystemNotificationAsync(string title, string message, string type, string priority, List<Guid>? userIds = null, string? roleFilter = null);
}