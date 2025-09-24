namespace ClinicManagement.API.DTOs;

public record CreateServiceRequest
{
    public Guid? ClinicId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int DurationMinutes { get; init; }
    public bool IsActive { get; init; } = true;
}

public record UpdateServiceRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int DurationMinutes { get; init; }
    public bool IsActive { get; init; }
}

public record ServiceResponse
{
    public Guid Id { get; init; }
    public Guid ClinicId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int DurationMinutes { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}