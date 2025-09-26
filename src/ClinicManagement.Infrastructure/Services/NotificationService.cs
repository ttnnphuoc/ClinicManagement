using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Infrastructure.Data;

namespace ClinicManagement.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IClinicContext _clinicContext;

    public NotificationService(ApplicationDbContext context, IClinicContext clinicContext)
    {
        _context = context;
        _clinicContext = clinicContext;
    }

    public async Task<(bool Success, string? ErrorCode, Notification? Notification)> ScheduleNotificationAsync(
        string notificationType,
        string deliveryMethod,
        string recipient,
        string subject,
        string message,
        DateTime scheduledTime,
        Guid? patientId = null,
        Guid? appointmentId = null)
    {
        var notification = new Notification
        {
            ClinicId = _clinicContext.CurrentClinicId!.Value,
            PatientId = patientId,
            AppointmentId = appointmentId,
            NotificationType = notificationType,
            DeliveryMethod = deliveryMethod,
            Recipient = recipient,
            Subject = subject,
            Message = message,
            ScheduledTime = scheduledTime,
            Status = "Pending"
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return (true, null, notification);
    }

    public async Task<(bool Success, string? ErrorCode)> SendNotificationAsync(Guid notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification == null)
        {
            return (false, "NOTIFICATION_NOT_FOUND");
        }

        // TODO: Implement actual sending logic (SMS, Email, Push notification)
        // For now, just mark as sent
        notification.Status = "Sent";
        notification.SentTime = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? ErrorCode)> SendAppointmentReminderAsync(Guid appointmentId, int hoursBeforeAppointment = 24)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);

        if (appointment == null)
        {
            return (false, "APPOINTMENT_NOT_FOUND");
        }

        var reminderTime = appointment.AppointmentDate.AddHours(-hoursBeforeAppointment);
        var message = $"Dear {appointment.Patient.FullName}, you have an appointment scheduled for {appointment.AppointmentDate:MMM dd, yyyy 'at' HH:mm}. Please arrive 15 minutes early.";

        var (success, errorCode, _) = await ScheduleNotificationAsync(
            "AppointmentReminder",
            "SMS",
            appointment.Patient.PhoneNumber,
            "Appointment Reminder",
            message,
            reminderTime,
            appointment.PatientId,
            appointmentId);

        return (success, errorCode);
    }

    public async Task<(bool Success, string? ErrorCode)> SendPaymentReminderAsync(Guid billId)
    {
        var bill = await _context.Bills
            .Include(b => b.Patient)
            .FirstOrDefaultAsync(b => b.Id == billId);

        if (bill == null)
        {
            return (false, "BILL_NOT_FOUND");
        }

        var message = $"Dear {bill.Patient.FullName}, you have an outstanding bill of {bill.TotalAmount:C}. Bill number: {bill.BillNumber}. Please make payment at your earliest convenience.";

        var (success, errorCode, _) = await ScheduleNotificationAsync(
            "PaymentReminder",
            "SMS",
            bill.Patient.PhoneNumber,
            "Payment Reminder",
            message,
            DateTime.UtcNow,
            bill.PatientId);

        return (success, errorCode);
    }

    public async Task<(bool Success, string? ErrorCode)> ScheduleFollowUpReminderAsync(Guid treatmentHistoryId, DateTime followUpDate)
    {
        var treatment = await _context.TreatmentHistories
            .Include(th => th.Patient)
            .FirstOrDefaultAsync(th => th.Id == treatmentHistoryId);

        if (treatment == null)
        {
            return (false, "TREATMENT_NOT_FOUND");
        }

        var message = $"Dear {treatment.Patient.FullName}, this is a reminder for your follow-up appointment scheduled for {followUpDate:MMM dd, yyyy}. Please contact us to confirm your appointment.";

        var reminderTime = followUpDate.AddDays(-1); // Remind 1 day before

        var (success, errorCode, _) = await ScheduleNotificationAsync(
            "FollowUp",
            "SMS",
            treatment.Patient.PhoneNumber,
            "Follow-up Reminder",
            message,
            reminderTime,
            treatment.PatientId);

        return (success, errorCode);
    }

    public async Task<IEnumerable<Notification>> GetPendingNotificationsAsync()
    {
        return await _context.Notifications
            .Where(n => n.Status == "Pending" && n.ScheduledTime <= DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetPatientNotificationsAsync(Guid patientId)
    {
        return await _context.Notifications
            .Where(n => n.PatientId == patientId)
            .OrderByDescending(n => n.ScheduledTime)
            .ToListAsync();
    }

    public async Task ProcessPendingNotificationsAsync()
    {
        var pendingNotifications = await GetPendingNotificationsAsync();

        foreach (var notification in pendingNotifications)
        {
            await SendNotificationAsync(notification.Id);
        }
    }

    // Additional methods for user notification management
    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool? isRead = null, string? type = null)
    {
        var query = _context.Notifications.Where(n => n.UserId == userId);
        
        if (isRead.HasValue)
            query = query.Where(n => n.IsRead == isRead.Value);
            
        if (!string.IsNullOrEmpty(type))
            query = query.Where(n => n.Type == type);
            
        return await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task<Notification?> GetNotificationAsync(Guid id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<Notification> CreateNotificationAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
            
        if (notification != null)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();
            
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteNotificationAsync(Guid notificationId, Guid userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
            
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SendSystemNotificationAsync(string title, string message, string type, string priority, List<Guid>? userIds = null, string? roleFilter = null)
    {
        var targetUsers = new List<Guid>();
        
        if (userIds != null)
        {
            targetUsers.AddRange(userIds);
        }
        else if (!string.IsNullOrEmpty(roleFilter))
        {
            // Get users by role - this would need proper implementation
            var clinicId = _clinicContext.CurrentClinicId ?? Guid.Empty;
            var staffInRole = await _context.Staff
                .Where(s => s.Role.ToString() == roleFilter && s.StaffClinics.Any(sc => sc.ClinicId == clinicId))
                .Select(s => s.Id)
                .ToListAsync();
            targetUsers.AddRange(staffInRole);
        }

        foreach (var userId in targetUsers)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                Priority = priority,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ClinicId = _clinicContext.CurrentClinicId ?? Guid.Empty
            };
            
            _context.Notifications.Add(notification);
        }
        
        await _context.SaveChangesAsync();
    }
}