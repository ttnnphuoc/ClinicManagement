using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class TreatmentHistoryService : ITreatmentHistoryService
{
    private readonly ITreatmentHistoryRepository _treatmentHistoryRepository;
    private readonly IClinicContext _clinicContext;

    public TreatmentHistoryService(ITreatmentHistoryRepository treatmentHistoryRepository, IClinicContext clinicContext)
    {
        _treatmentHistoryRepository = treatmentHistoryRepository;
        _clinicContext = clinicContext;
    }

    public async Task<(bool Success, string? ErrorCode, TreatmentHistory? TreatmentHistory)> CreateTreatmentAsync(
        Guid patientId,
        Guid? appointmentId,
        Guid staffId,
        DateTime treatmentDate,
        string? chiefComplaint,
        string? symptoms,
        string? bloodPressure,
        decimal? temperature,
        int? heartRate,
        int? respiratoryRate,
        decimal? weight,
        decimal? height,
        string? physicalExamination,
        string? diagnosis,
        string? differentialDiagnosis,
        string treatment,
        string? prescription,
        string? treatmentPlan,
        string? followUpInstructions,
        DateTime? nextAppointmentDate,
        string? notes)
    {
        if (!_clinicContext.CurrentClinicId.HasValue)
        {
            return (false, "AUTH_UNAUTHORIZED", null);
        }

        var treatmentHistory = new TreatmentHistory
        {
            ClinicId = _clinicContext.CurrentClinicId.Value,
            PatientId = patientId,
            AppointmentId = appointmentId,
            StaffId = staffId,
            TreatmentDate = treatmentDate,
            ChiefComplaint = chiefComplaint,
            Symptoms = symptoms,
            BloodPressure = bloodPressure,
            Temperature = temperature,
            HeartRate = heartRate,
            RespiratoryRate = respiratoryRate,
            Weight = weight,
            Height = height,
            PhysicalExamination = physicalExamination,
            Diagnosis = diagnosis,
            DifferentialDiagnosis = differentialDiagnosis,
            Treatment = treatment,
            PrescriptionNotes = prescription,
            TreatmentPlan = treatmentPlan,
            FollowUpInstructions = followUpInstructions,
            NextAppointmentDate = nextAppointmentDate,
            Notes = notes
        };

        await _treatmentHistoryRepository.AddAsync(treatmentHistory);
        await _treatmentHistoryRepository.SaveChangesAsync();

        return (true, null, treatmentHistory);
    }

    public async Task<(bool Success, string? ErrorCode, TreatmentHistory? TreatmentHistory)> UpdateTreatmentAsync(
        Guid id,
        string? chiefComplaint,
        string? symptoms,
        string? bloodPressure,
        decimal? temperature,
        int? heartRate,
        int? respiratoryRate,
        decimal? weight,
        decimal? height,
        string? physicalExamination,
        string? diagnosis,
        string? differentialDiagnosis,
        string treatment,
        string? prescription,
        string? treatmentPlan,
        string? followUpInstructions,
        DateTime? nextAppointmentDate,
        string? notes)
    {
        var treatmentHistory = await _treatmentHistoryRepository.GetByIdAsync(id);
        if (treatmentHistory == null)
        {
            return (false, "NOT_FOUND", null);
        }

        treatmentHistory.ChiefComplaint = chiefComplaint;
        treatmentHistory.Symptoms = symptoms;
        treatmentHistory.BloodPressure = bloodPressure;
        treatmentHistory.Temperature = temperature;
        treatmentHistory.HeartRate = heartRate;
        treatmentHistory.RespiratoryRate = respiratoryRate;
        treatmentHistory.Weight = weight;
        treatmentHistory.Height = height;
        treatmentHistory.PhysicalExamination = physicalExamination;
        treatmentHistory.Diagnosis = diagnosis;
        treatmentHistory.DifferentialDiagnosis = differentialDiagnosis;
        treatmentHistory.Treatment = treatment;
        treatmentHistory.PrescriptionNotes = prescription;
        treatmentHistory.TreatmentPlan = treatmentPlan;
        treatmentHistory.FollowUpInstructions = followUpInstructions;
        treatmentHistory.NextAppointmentDate = nextAppointmentDate;
        treatmentHistory.Notes = notes;

        await _treatmentHistoryRepository.UpdateAsync(treatmentHistory);
        await _treatmentHistoryRepository.SaveChangesAsync();

        return (true, null, treatmentHistory);
    }

    public async Task<(bool Success, string? ErrorCode)> DeleteTreatmentAsync(Guid id)
    {
        var treatmentHistory = await _treatmentHistoryRepository.GetByIdAsync(id);
        if (treatmentHistory == null)
        {
            return (false, "NOT_FOUND");
        }

        await _treatmentHistoryRepository.SoftDeleteAsync(treatmentHistory);
        await _treatmentHistoryRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<TreatmentHistory?> GetTreatmentByIdAsync(Guid id)
    {
        return await _treatmentHistoryRepository.GetByIdAsync(id);
    }

    public async Task<TreatmentHistory?> GetTreatmentByAppointmentIdAsync(Guid appointmentId)
    {
        return await _treatmentHistoryRepository.GetByAppointmentIdAsync(appointmentId);
    }

    public async Task<IEnumerable<TreatmentHistory>> GetPatientTreatmentHistoryAsync(Guid patientId)
    {
        return await _treatmentHistoryRepository.GetPatientTreatmentHistoryAsync(patientId);
    }

    public async Task<(IEnumerable<TreatmentHistory> Items, int Total)> SearchTreatmentHistoryAsync(Guid? patientId, string? search, int page, int pageSize)
    {
        var items = await _treatmentHistoryRepository.SearchTreatmentHistoryAsync(patientId, search, page, pageSize);
        var total = await _treatmentHistoryRepository.GetTotalCountAsync(patientId, search);
        return (items, total);
    }
}