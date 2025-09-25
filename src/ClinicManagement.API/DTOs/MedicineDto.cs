namespace ClinicManagement.API.DTOs;

public record CreateMedicineRequest
{
    public string Name { get; init; } = string.Empty;
    public string? GenericName { get; init; }
    public string? Manufacturer { get; init; }
    public string? Dosage { get; init; }
    public string? Form { get; init; }
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
}

public record UpdateMedicineRequest
{
    public string Name { get; init; } = string.Empty;
    public string? GenericName { get; init; }
    public string? Manufacturer { get; init; }
    public string? Dosage { get; init; }
    public string? Form { get; init; }
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
}

public record MedicineResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? GenericName { get; init; }
    public string? Manufacturer { get; init; }
    public string? Dosage { get; init; }
    public string? Form { get; init; }
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public decimal TotalStock { get; init; }
    public DateTime CreatedAt { get; init; }
}