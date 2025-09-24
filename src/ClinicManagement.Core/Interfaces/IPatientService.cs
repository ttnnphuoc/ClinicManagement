using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IPatientService
{
    Task<(bool Success, string? ErrorCode, Patient? Patient)> CreatePatientAsync(
        Guid clinicId,
        string fullName,
        string phoneNumber,
        string? email,
        DateTime? dateOfBirth,
        string? address,
        string? gender,
        string? allergies,
        string? chronicConditions,
        string? emergencyContactName,
        string? emergencyContactPhone,
        string? bloodType,
        string? idNumber,
        string? insuranceNumber,
        string? insuranceProvider,
        string? occupation,
        string? referralSource,
        bool receivePromotions,
        string? notes);

    Task<(bool Success, string? ErrorCode, Patient? Patient)> UpdatePatientAsync(
        Guid id,
        string fullName,
        string phoneNumber,
        string? email,
        DateTime? dateOfBirth,
        string? address,
        string? gender,
        string? allergies,
        string? chronicConditions,
        string? emergencyContactName,
        string? emergencyContactPhone,
        string? bloodType,
        string? idNumber,
        string? insuranceNumber,
        string? insuranceProvider,
        string? occupation,
        string? referralSource,
        bool receivePromotions,
        string? notes);

    Task<(bool Success, string? ErrorCode)> DeletePatientAsync(Guid id);

    Task<Patient?> GetPatientByIdAsync(Guid id);

    Task<(IEnumerable<Patient> Items, int Total)> SearchPatientsAsync(string? search, int page, int pageSize);
}