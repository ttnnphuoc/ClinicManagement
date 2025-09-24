namespace ClinicManagement.API.DTOs;

public record CreateStaffRequest
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public List<Guid> ClinicIds { get; init; } = new();
    public bool IsActive { get; init; } = true;
}

public record UpdateStaffRequest
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public List<Guid> ClinicIds { get; init; } = new();
    public bool IsActive { get; init; }
}

public record ChangePasswordRequest
{
    public string NewPassword { get; init; } = string.Empty;
}

public record StaffResponse
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public List<StaffClinicResponse> Clinics { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

public record StaffClinicResponse
{
    public Guid ClinicId { get; init; }
    public string ClinicName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}