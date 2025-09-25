namespace ClinicManagement.API.DTOs;

public record CreateAppointmentRequest
{
    public Guid PatientId { get; init; }
    public Guid StaffId { get; init; }
    public DateTime AppointmentDate { get; init; }
    public string Status { get; init; } = "Scheduled";
    public string? Notes { get; init; }
}

public record UpdateAppointmentRequest
{
    public Guid PatientId { get; init; }
    public Guid StaffId { get; init; }
    public DateTime AppointmentDate { get; init; }
    public string Status { get; init; } = "Scheduled";
    public string? Notes { get; init; }
}

public record UpdateAppointmentStatusRequest
{
    public string Status { get; init; } = string.Empty;
}

public record AppointmentResponse
{
    public Guid Id { get; init; }
    public Guid ClinicId { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public Guid StaffId { get; init; }
    public string StaffName { get; init; } = string.Empty;
    public DateTime AppointmentDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
}