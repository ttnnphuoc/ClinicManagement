using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class MedicalServiceService : IMedicalServiceService
{
    private readonly IServiceRepository _serviceRepository;

    public MedicalServiceService(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<(bool Success, string? ErrorCode, Service? Service)> CreateServiceAsync(
        Guid? clinicId,
        string name,
        string? description,
        decimal price,
        int durationMinutes,
        bool isActive)
    {
        if (!clinicId.HasValue)
        {
            return (false, "AUTH_UNAUTHORIZED", null);
        }

        var service = new Service
        {
            ClinicId = clinicId.Value,
            Name = name,
            Description = description,
            Price = price,
            DurationMinutes = durationMinutes,
            IsActive = isActive
        };

        await _serviceRepository.AddAsync(service);
        await _serviceRepository.SaveChangesAsync();

        return (true, null, service);
    }

    public async Task<(bool Success, string? ErrorCode, Service? Service)> UpdateServiceAsync(
        Guid id,
        string name,
        string? description,
        decimal price,
        int durationMinutes,
        bool isActive)
    {
        var service = await _serviceRepository.GetByIdAsync(id);
        if (service == null)
        {
            return (false, "NOT_FOUND", null);
        }

        service.Name = name;
        service.Description = description;
        service.Price = price;
        service.DurationMinutes = durationMinutes;
        service.IsActive = isActive;

        await _serviceRepository.UpdateAsync(service);
        await _serviceRepository.SaveChangesAsync();

        return (true, null, service);
    }

    public async Task<(bool Success, string? ErrorCode)> DeleteServiceAsync(Guid id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);
        if (service == null)
        {
            return (false, "NOT_FOUND");
        }

        await _serviceRepository.SoftDeleteAsync(service);
        await _serviceRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<Service?> GetServiceByIdAsync(Guid id)
    {
        return await _serviceRepository.GetByIdAsync(id);
    }

    public async Task<(IEnumerable<Service> Items, int Total)> SearchServicesAsync(string? search, int page, int pageSize)
    {
        var items = await _serviceRepository.SearchServicesAsync(search, page, pageSize);
        var total = await _serviceRepository.GetTotalCountAsync(search);
        return (items, total);
    }

    public async Task<IEnumerable<Service>> GetActiveServicesAsync()
    {
        return await _serviceRepository.GetActiveServicesAsync();
    }
}