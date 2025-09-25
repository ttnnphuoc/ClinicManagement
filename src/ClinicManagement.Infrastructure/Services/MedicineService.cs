using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class MedicineService : IMedicineService
{
    private readonly IMedicineRepository _medicineRepository;
    private readonly IClinicContext _clinicContext;

    public MedicineService(IMedicineRepository medicineRepository, IClinicContext clinicContext)
    {
        _medicineRepository = medicineRepository;
        _clinicContext = clinicContext;
    }

    public async Task<(bool Success, string? ErrorCode, Medicine? Medicine)> CreateMedicineAsync(
        string name,
        string? genericName,
        string? manufacturer,
        string? dosage,
        string? form,
        decimal price,
        string? description)
    {
        var medicine = new Medicine
        {
            ClinicId = _clinicContext.CurrentClinicId!.Value,
            Name = name,
            GenericName = genericName,
            Manufacturer = manufacturer,
            Dosage = dosage,
            Form = form,
            Price = price,
            Description = description,
            IsActive = true
        };

        await _medicineRepository.AddAsync(medicine);
        await _medicineRepository.SaveChangesAsync();

        return (true, null, medicine);
    }

    public async Task<(bool Success, string? ErrorCode, Medicine? Medicine)> UpdateMedicineAsync(
        Guid id,
        string name,
        string? genericName,
        string? manufacturer,
        string? dosage,
        string? form,
        decimal price,
        string? description,
        bool isActive)
    {
        var medicine = await _medicineRepository.GetByIdAsync(id);
        if (medicine == null)
        {
            return (false, "NOT_FOUND", null);
        }

        medicine.Name = name;
        medicine.GenericName = genericName;
        medicine.Manufacturer = manufacturer;
        medicine.Dosage = dosage;
        medicine.Form = form;
        medicine.Price = price;
        medicine.Description = description;
        medicine.IsActive = isActive;

        await _medicineRepository.UpdateAsync(medicine);
        await _medicineRepository.SaveChangesAsync();

        return (true, null, medicine);
    }

    public async Task<(bool Success, string? ErrorCode)> DeleteMedicineAsync(Guid id)
    {
        var medicine = await _medicineRepository.GetByIdAsync(id);
        if (medicine == null)
        {
            return (false, "NOT_FOUND");
        }

        await _medicineRepository.SoftDeleteAsync(medicine);
        await _medicineRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<Medicine?> GetMedicineAsync(Guid id)
    {
        return await _medicineRepository.GetMedicineWithInventoryAsync(id);
    }

    public async Task<IEnumerable<Medicine>> GetActiveMedicinesAsync()
    {
        return await _medicineRepository.GetActiveMedicinesAsync();
    }

    public async Task<IEnumerable<Medicine>> SearchMedicinesAsync(string searchTerm)
    {
        return await _medicineRepository.SearchMedicinesAsync(searchTerm);
    }
}