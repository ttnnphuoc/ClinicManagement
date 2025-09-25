using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface ITreatmentHistoryService
{
    Task<(bool Success, string? ErrorCode, TreatmentHistory? TreatmentHistory)> CreateTreatmentAsync(
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
        string? notes);

    Task<(bool Success, string? ErrorCode, TreatmentHistory? TreatmentHistory)> UpdateTreatmentAsync(
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
        string? notes);

    Task<(bool Success, string? ErrorCode)> DeleteTreatmentAsync(Guid id);
    Task<TreatmentHistory?> GetTreatmentByIdAsync(Guid id);
    Task<TreatmentHistory?> GetTreatmentByAppointmentIdAsync(Guid appointmentId);
    Task<IEnumerable<TreatmentHistory>> GetPatientTreatmentHistoryAsync(Guid patientId);
    Task<(IEnumerable<TreatmentHistory> Items, int Total)> SearchTreatmentHistoryAsync(Guid? patientId, string? search, int page, int pageSize);
}