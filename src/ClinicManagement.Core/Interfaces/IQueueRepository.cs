using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IQueueRepository : IRepository<PatientQueue>
{
    Task<PatientQueue?> GetQueueWithDetailsAsync(Guid id);
    Task<IEnumerable<PatientQueue>> GetTodayQueueAsync();
    Task<IEnumerable<PatientQueue>> GetWaitingPatientsAsync();
    Task<string> GenerateQueueNumberAsync(DateTime queueDate, string queueType);
    Task<PatientQueue?> GetNextPatientAsync(Guid? staffId = null, Guid? roomId = null);
}