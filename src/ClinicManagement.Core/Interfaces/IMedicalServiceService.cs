using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IMedicalServiceService
{
    Task<(bool Success, string? ErrorCode, Service? Service)> CreateServiceAsync(
        Guid? clinicId,
        string name,
        string? description,
        decimal price,
        int durationMinutes,
        bool isActive);

    Task<(bool Success, string? ErrorCode, Service? Service)> UpdateServiceAsync(
        Guid id,
        string name,
        string? description,
        decimal price,
        int durationMinutes,
        bool isActive);

    Task<(bool Success, string? ErrorCode)> DeleteServiceAsync(Guid id);

    Task<Service?> GetServiceByIdAsync(Guid id);

    Task<(IEnumerable<Service> Items, int Total)> SearchServicesAsync(string? search, int page, int pageSize);

    Task<IEnumerable<Service>> GetActiveServicesAsync();
}