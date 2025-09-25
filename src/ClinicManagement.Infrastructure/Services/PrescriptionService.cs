using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly ITreatmentHistoryRepository _treatmentHistoryRepository;
    private readonly IClinicContext _clinicContext;

    public PrescriptionService(
        IPrescriptionRepository prescriptionRepository,
        ITreatmentHistoryRepository treatmentHistoryRepository,
        IClinicContext clinicContext)
    {
        _prescriptionRepository = prescriptionRepository;
        _treatmentHistoryRepository = treatmentHistoryRepository;
        _clinicContext = clinicContext;
    }

    public async Task<(bool Success, string? ErrorCode, Prescription? Prescription)> CreatePrescriptionAsync(
        Guid treatmentHistoryId,
        IEnumerable<PrescriptionMedicine> medicines,
        string? notes = null)
    {
        var treatmentHistory = await _treatmentHistoryRepository.GetByIdAsync(treatmentHistoryId);
        if (treatmentHistory == null)
        {
            return (false, "TREATMENT_HISTORY_NOT_FOUND", null);
        }

        var medicineList = medicines.ToList();
        if (!medicineList.Any())
        {
            return (false, "NO_MEDICINES", null);
        }

        var prescription = new Prescription
        {
            ClinicId = _clinicContext.CurrentClinicId!.Value,
            TreatmentHistoryId = treatmentHistoryId,
            PatientId = treatmentHistory.PatientId,
            DoctorId = treatmentHistory.StaffId,
            PrescriptionNumber = await _prescriptionRepository.GeneratePrescriptionNumberAsync(),
            PrescriptionDate = DateTime.UtcNow,
            Status = "Active",
            Notes = notes
        };

        // Add prescription medicines
        foreach (var medicine in medicineList)
        {
            medicine.PrescriptionId = prescription.Id;
            prescription.PrescriptionMedicines.Add(medicine);
        }

        await _prescriptionRepository.AddAsync(prescription);
        await _prescriptionRepository.SaveChangesAsync();

        var created = await _prescriptionRepository.GetPrescriptionWithDetailsAsync(prescription.Id);
        return (true, null, created);
    }

    public async Task<(bool Success, string? ErrorCode, Prescription? Prescription)> UpdatePrescriptionAsync(
        Guid prescriptionId,
        IEnumerable<PrescriptionMedicine> medicines,
        string? notes = null)
    {
        var prescription = await _prescriptionRepository.GetPrescriptionWithDetailsAsync(prescriptionId);
        if (prescription == null)
        {
            return (false, "NOT_FOUND", null);
        }

        if (prescription.Status == "Dispensed" || prescription.Status == "Cancelled")
        {
            return (false, "PRESCRIPTION_NOT_EDITABLE", null);
        }

        var medicineList = medicines.ToList();
        if (!medicineList.Any())
        {
            return (false, "NO_MEDICINES", null);
        }

        // Clear existing medicines
        prescription.PrescriptionMedicines.Clear();

        // Add new medicines
        foreach (var medicine in medicineList)
        {
            medicine.PrescriptionId = prescriptionId;
            prescription.PrescriptionMedicines.Add(medicine);
        }

        prescription.Notes = notes;

        await _prescriptionRepository.UpdateAsync(prescription);
        await _prescriptionRepository.SaveChangesAsync();

        var updated = await _prescriptionRepository.GetPrescriptionWithDetailsAsync(prescriptionId);
        return (true, null, updated);
    }

    public async Task<(bool Success, string? ErrorCode)> DispenseMedicineAsync(
        Guid prescriptionId,
        Guid medicineId,
        decimal quantityDispensed)
    {
        var prescription = await _prescriptionRepository.GetPrescriptionWithDetailsAsync(prescriptionId);
        if (prescription == null)
        {
            return (false, "NOT_FOUND");
        }

        var prescriptionMedicine = prescription.PrescriptionMedicines
            .FirstOrDefault(pm => pm.MedicineId == medicineId);

        if (prescriptionMedicine == null)
        {
            return (false, "MEDICINE_NOT_FOUND");
        }

        if (prescriptionMedicine.QuantityDispensed + quantityDispensed > prescriptionMedicine.Quantity)
        {
            return (false, "QUANTITY_EXCEEDS_PRESCRIBED");
        }

        prescriptionMedicine.QuantityDispensed += quantityDispensed;
        
        if (prescriptionMedicine.QuantityDispensed >= prescriptionMedicine.Quantity)
        {
            prescriptionMedicine.IsDispensed = true;
        }

        // Update prescription status if all medicines are dispensed
        if (prescription.PrescriptionMedicines.All(pm => pm.IsDispensed))
        {
            prescription.Status = "Dispensed";
        }

        await _prescriptionRepository.UpdateAsync(prescription);
        await _prescriptionRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<Prescription?> GetPrescriptionAsync(Guid prescriptionId)
    {
        return await _prescriptionRepository.GetPrescriptionWithDetailsAsync(prescriptionId);
    }

    public async Task<IEnumerable<Prescription>> GetPatientPrescriptionsAsync(Guid patientId)
    {
        return await _prescriptionRepository.GetPatientPrescriptionsAsync(patientId);
    }

    public async Task<IEnumerable<Prescription>> GetActivePrescriptionsAsync()
    {
        return await _prescriptionRepository.GetActivePrescriptionsAsync();
    }
}