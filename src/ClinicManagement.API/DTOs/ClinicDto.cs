namespace ClinicManagement.API.DTOs;

public record CreateClinicRequest
{
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public bool IsActive { get; init; } = true;
}

public record UpdateClinicRequest
{
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public bool IsActive { get; init; }
}

public record ClinicResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}