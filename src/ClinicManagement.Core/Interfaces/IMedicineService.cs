using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IMedicineService
{
    Task<(bool Success, string? ErrorCode, Medicine? Medicine)> CreateMedicineAsync(
        string name,
        string? genericName,
        string? manufacturer,
        string? dosage,
        string? form,
        decimal price,
        string? description);

    Task<(bool Success, string? ErrorCode, Medicine? Medicine)> UpdateMedicineAsync(
        Guid id,
        string name,
        string? genericName,
        string? manufacturer,
        string? dosage,
        string? form,
        decimal price,
        string? description,
        bool isActive);

    Task<(bool Success, string? ErrorCode)> DeleteMedicineAsync(Guid id);
    Task<Medicine?> GetMedicineAsync(Guid id);
    Task<IEnumerable<Medicine>> GetActiveMedicinesAsync();
    Task<IEnumerable<Medicine>> SearchMedicinesAsync(string searchTerm);
}