namespace ClinicManagement.API.DTOs;

public class NotificationDto
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "info", "warning", "error", "appointment", "system"
    public string? Priority { get; set; } = "medium"; // "low", "medium", "high"
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
}