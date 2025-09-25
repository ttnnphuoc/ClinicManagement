namespace ClinicManagement.API.DTOs;

public record AddPatientToQueueRequest
{
    public Guid PatientId { get; init; }
    public Guid? AppointmentId { get; init; }
    public string QueueType { get; init; } = "Appointment";
    public int Priority { get; init; } = 0;
    public string? Notes { get; init; }
}

public record UpdateQueueStatusRequest
{
    public string Status { get; init; } = string.Empty;
    public Guid? AssignedStaffId { get; init; }
    public Guid? RoomId { get; init; }
}

public record PatientQueueResponse
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public Guid? AppointmentId { get; init; }
    public string QueueNumber { get; init; } = string.Empty;
    public DateTime QueueDate { get; init; }
    public DateTime CheckInTime { get; init; }
    public DateTime? CalledTime { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? CompletionTime { get; init; }
    public string Status { get; init; } = string.Empty;
    public string QueueType { get; init; } = string.Empty;
    public int Priority { get; init; }
    public string? AssignedStaffName { get; init; }
    public string? RoomName { get; init; }
    public string? Notes { get; init; }
    public int EstimatedWaitTime { get; init; }
}