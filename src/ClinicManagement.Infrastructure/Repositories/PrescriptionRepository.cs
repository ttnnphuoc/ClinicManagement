using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Repositories;

public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
{
    public PrescriptionRepository(ApplicationDbContext context, IClinicContext clinicContext) 
        : base(context, clinicContext)
    {
    }

    public async Task<Prescription?> GetPrescriptionWithDetailsAsync(Guid id)
    {
        return await _context.Prescriptions
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .Include(p => p.TreatmentHistory)
            .Include(p => p.PrescriptionMedicines)
                .ThenInclude(pm => pm.Medicine)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Prescription>> GetPatientPrescriptionsAsync(Guid patientId)
    {
        return await _context.Prescriptions
            .Include(p => p.Doctor)
            .Include(p => p.PrescriptionMedicines)
                .ThenInclude(pm => pm.Medicine)
            .Where(p => p.PatientId == patientId)
            .OrderByDescending(p => p.PrescriptionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Prescription>> GetActivePrescriptionsAsync()
    {
        return await _context.Prescriptions
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .Include(p => p.PrescriptionMedicines)
                .ThenInclude(pm => pm.Medicine)
            .Where(p => p.Status == "Active" && p.PrescriptionMedicines.Any(pm => !pm.IsDispensed))
            .OrderBy(p => p.PrescriptionDate)
            .ToListAsync();
    }

    public async Task<string> GeneratePrescriptionNumberAsync()
    {
        var today = DateTime.Today;
        var todayString = today.ToString("yyyyMMdd");
        
        var lastPrescription = await _context.Prescriptions
            .Where(p => p.PrescriptionNumber.StartsWith($"RX-{todayString}"))
            .OrderByDescending(p => p.PrescriptionNumber)
            .FirstOrDefaultAsync();

        if (lastPrescription == null)
        {
            return $"RX-{todayString}-0001";
        }

        var lastNumber = lastPrescription.PrescriptionNumber.Split('-').Last();
        if (int.TryParse(lastNumber, out int number))
        {
            return $"RX-{todayString}-{(number + 1):D4}";
        }

        return $"RX-{todayString}-0001";
    }
}