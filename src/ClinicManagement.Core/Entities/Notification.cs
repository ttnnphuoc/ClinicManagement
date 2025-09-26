namespace ClinicManagement.Core.Entities;

public class Notification : BaseEntity, IClinicEntity
{
    public Guid ClinicId { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public string NotificationType { get; set; } = string.Empty; // AppointmentReminder, PaymentReminder, FollowUp, Promotion
    public string DeliveryMethod { get; set; } = string.Empty; // SMS, Email, Push, InApp
    public string Recipient { get; set; } = string.Empty; // Phone number or email
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime ScheduledTime { get; set; }
    public DateTime? SentTime { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Sent, Failed, Cancelled
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; } = 0;
    public DateTime? NextRetry { get; set; }

    // Additional properties for user notifications
    public Guid? UserId { get; set; }
    public string? Title { get; set; }
    public string? Type { get; set; } // "info", "warning", "error", "appointment", "system"
    public string? Priority { get; set; } = "medium"; // "low", "medium", "high"
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
    public Patient? Patient { get; set; }
    public Appointment? Appointment { get; set; }
}