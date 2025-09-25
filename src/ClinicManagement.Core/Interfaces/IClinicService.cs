using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IClinicService
{
    Task<(bool Success, string? ErrorCode, Clinic? Clinic)> CreateClinicAsync(
        string name,
        string address,
        string phoneNumber,
        string? email,
        bool isActive);

    Task<(bool Success, string? ErrorCode, Clinic? Clinic)> CreateClinicWithPackageAsync(
        string name,
        string address,
        string phoneNumber,
        string? email,
        bool isActive,
        Guid ownerId,
        Guid packageId);

    Task<(bool Success, string? ErrorCode, Clinic? Clinic)> UpdateClinicAsync(
        Guid id,
        string name,
        string address,
        string phoneNumber,
        string? email,
        bool isActive);

    Task<(bool Success, string? ErrorCode)> DeleteClinicAsync(Guid id);

    Task<Clinic?> GetClinicByIdAsync(Guid id);

    Task<(IEnumerable<Clinic> Items, int Total)> SearchClinicsAsync(string? search, int page, int pageSize);

    Task<IEnumerable<Clinic>> GetActiveClinicsAsync();
}