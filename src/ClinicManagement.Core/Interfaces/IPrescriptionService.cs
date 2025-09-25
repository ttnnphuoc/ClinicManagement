using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IPrescriptionService
{
    Task<(bool Success, string? ErrorCode, Prescription? Prescription)> CreatePrescriptionAsync(
        Guid treatmentHistoryId,
        IEnumerable<PrescriptionMedicine> medicines,
        string? notes = null);

    Task<(bool Success, string? ErrorCode, Prescription? Prescription)> UpdatePrescriptionAsync(
        Guid prescriptionId,
        IEnumerable<PrescriptionMedicine> medicines,
        string? notes = null);

    Task<(bool Success, string? ErrorCode)> DispenseMedicineAsync(
        Guid prescriptionId,
        Guid medicineId,
        decimal quantityDispensed);

    Task<Prescription?> GetPrescriptionAsync(Guid prescriptionId);
    Task<IEnumerable<Prescription>> GetPatientPrescriptionsAsync(Guid patientId);
    Task<IEnumerable<Prescription>> GetActivePrescriptionsAsync();
}