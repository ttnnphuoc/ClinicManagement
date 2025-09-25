using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IQueueService
{
    Task<(bool Success, string? ErrorCode, PatientQueue? Queue)> AddPatientToQueueAsync(
        Guid patientId,
        Guid? appointmentId = null,
        string queueType = "Appointment",
        int priority = 0,
        string? notes = null);

    Task<(bool Success, string? ErrorCode)> UpdateQueueStatusAsync(
        Guid queueId,
        string status,
        Guid? assignedStaffId = null,
        Guid? roomId = null);

    Task<(bool Success, string? ErrorCode)> CallNextPatientAsync(
        Guid? staffId = null,
        Guid? roomId = null);

    Task<PatientQueue?> GetQueueItemAsync(Guid queueId);
    
    Task<IEnumerable<PatientQueue>> GetTodayQueueAsync();
    
    Task<IEnumerable<PatientQueue>> GetWaitingPatientsAsync();
    
    Task<int> GetEstimatedWaitTimeAsync(Guid queueId);
    
    Task<string> GenerateQueueNumberAsync(DateTime queueDate, string queueType);
}