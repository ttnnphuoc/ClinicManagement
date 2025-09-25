using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class ClinicService : IClinicService
{
    private readonly IClinicRepository _clinicRepository;
    private readonly ISubscriptionService _subscriptionService;

    public ClinicService(IClinicRepository clinicRepository, ISubscriptionService subscriptionService)
    {
        _clinicRepository = clinicRepository;
        _subscriptionService = subscriptionService;
    }

    public async Task<(bool Success, string? ErrorCode, Clinic? Clinic)> CreateClinicAsync(
        string name,
        string address,
        string phoneNumber,
        string? email,
        bool isActive)
    {
        var clinic = new Clinic
        {
            Name = name,
            Address = address,
            PhoneNumber = phoneNumber,
            Email = email,
            IsActive = isActive
        };

        await _clinicRepository.AddAsync(clinic);
        await _clinicRepository.SaveChangesAsync();

        return (true, null, clinic);
    }

    public async Task<(bool Success, string? ErrorCode, Clinic? Clinic)> CreateClinicWithPackageAsync(
        string name,
        string address,
        string phoneNumber,
        string? email,
        bool isActive,
        Guid ownerId,
        Guid packageId)
    {
        var canCreateClinic = await _subscriptionService.ValidateUsageLimitAsync(ownerId, "Clinics");
        if (!canCreateClinic)
        {
            return (false, "CLINIC_LIMIT_EXCEEDED", null);
        }

        var activeSubscription = await _subscriptionService.GetActiveSubscriptionAsync(ownerId);
        if (activeSubscription == null)
        {
            var (subscriptionSuccess, subscriptionError, subscription) = await _subscriptionService.CreateSubscriptionAsync(ownerId, packageId);
            if (!subscriptionSuccess)
            {
                return (false, subscriptionError, null);
            }
        }

        var clinic = new Clinic
        {
            Name = name,
            Address = address,
            PhoneNumber = phoneNumber,
            Email = email,
            IsActive = isActive,
            OwnerId = ownerId
        };

        await _clinicRepository.AddAsync(clinic);
        await _clinicRepository.SaveChangesAsync();

        await _subscriptionService.UpdateUsageAsync(ownerId, "Clinics", 1);

        return (true, null, clinic);
    }

    public async Task<(bool Success, string? ErrorCode, Clinic? Clinic)> UpdateClinicAsync(
        Guid id,
        string name,
        string address,
        string phoneNumber,
        string? email,
        bool isActive)
    {
        var clinic = await _clinicRepository.GetByIdAsync(id);
        if (clinic == null)
        {
            return (false, "NOT_FOUND", null);
        }

        clinic.Name = name;
        clinic.Address = address;
        clinic.PhoneNumber = phoneNumber;
        clinic.Email = email;
        clinic.IsActive = isActive;

        await _clinicRepository.UpdateAsync(clinic);
        await _clinicRepository.SaveChangesAsync();

        return (true, null, clinic);
    }

    public async Task<(bool Success, string? ErrorCode)> DeleteClinicAsync(Guid id)
    {
        var clinic = await _clinicRepository.GetByIdAsync(id);
        if (clinic == null)
        {
            return (false, "NOT_FOUND");
        }

        await _clinicRepository.SoftDeleteAsync(clinic);
        await _clinicRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<Clinic?> GetClinicByIdAsync(Guid id)
    {
        return await _clinicRepository.GetByIdAsync(id);
    }

    public async Task<(IEnumerable<Clinic> Items, int Total)> SearchClinicsAsync(string? search, int page, int pageSize)
    {
        var items = await _clinicRepository.SearchClinicsAsync(search, page, pageSize);
        var total = await _clinicRepository.GetTotalCountAsync(search);
        return (items, total);
    }

    public async Task<IEnumerable<Clinic>> GetActiveClinicsAsync()
    {
        return await _clinicRepository.GetActiveClinicsAsync();
    }
}