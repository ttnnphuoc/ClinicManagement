using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class QueueService : IQueueService
{
    private readonly IQueueRepository _queueRepository;
    private readonly IClinicContext _clinicContext;

    public QueueService(IQueueRepository queueRepository, IClinicContext clinicContext)
    {
        _queueRepository = queueRepository;
        _clinicContext = clinicContext;
    }

    public async Task<(bool Success, string? ErrorCode, PatientQueue? Queue)> AddPatientToQueueAsync(
        Guid patientId,
        Guid? appointmentId = null,
        string queueType = "Appointment",
        int priority = 0,
        string? notes = null)
    {
        var queueDate = DateTime.Today;
        var queueNumber = await _queueRepository.GenerateQueueNumberAsync(queueDate, queueType);

        var patientQueue = new PatientQueue
        {
            ClinicId = _clinicContext.CurrentClinicId!.Value,
            PatientId = patientId,
            AppointmentId = appointmentId,
            QueueNumber = queueNumber,
            QueueDate = queueDate,
            CheckInTime = DateTime.UtcNow,
            Status = "Waiting",
            QueueType = queueType,
            Priority = priority,
            Notes = notes
        };

        await _queueRepository.AddAsync(patientQueue);
        await _queueRepository.SaveChangesAsync();

        var created = await _queueRepository.GetQueueWithDetailsAsync(patientQueue.Id);
        return (true, null, created);
    }

    public async Task<(bool Success, string? ErrorCode)> UpdateQueueStatusAsync(
        Guid queueId,
        string status,
        Guid? assignedStaffId = null,
        Guid? roomId = null)
    {
        var queue = await _queueRepository.GetByIdAsync(queueId);
        if (queue == null)
        {
            return (false, "QUEUE_NOT_FOUND");
        }

        queue.Status = status;
        if (assignedStaffId.HasValue)
            queue.AssignedStaffId = assignedStaffId;
        if (roomId.HasValue)
            queue.RoomId = roomId;

        switch (status)
        {
            case "Called":
                queue.CalledTime = DateTime.UtcNow;
                break;
            case "InProgress":
                queue.StartTime = DateTime.UtcNow;
                break;
            case "Completed":
            case "NoShow":
            case "Cancelled":
                queue.CompletionTime = DateTime.UtcNow;
                break;
        }

        await _queueRepository.UpdateAsync(queue);
        await _queueRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorCode)> CallNextPatientAsync(
        Guid? staffId = null,
        Guid? roomId = null)
    {
        var nextPatient = await _queueRepository.GetNextPatientAsync(staffId, roomId);
        if (nextPatient == null)
        {
            return (false, "NO_PATIENTS_WAITING");
        }

        return await UpdateQueueStatusAsync(nextPatient.Id, "Called", staffId, roomId);
    }

    public async Task<PatientQueue?> GetQueueItemAsync(Guid queueId)
    {
        return await _queueRepository.GetQueueWithDetailsAsync(queueId);
    }

    public async Task<IEnumerable<PatientQueue>> GetTodayQueueAsync()
    {
        return await _queueRepository.GetTodayQueueAsync();
    }

    public async Task<IEnumerable<PatientQueue>> GetWaitingPatientsAsync()
    {
        return await _queueRepository.GetWaitingPatientsAsync();
    }

    public async Task<int> GetEstimatedWaitTimeAsync(Guid queueId)
    {
        var queue = await _queueRepository.GetByIdAsync(queueId);
        if (queue == null)
            return 0;

        // Simple estimation: count patients ahead * average consultation time (15 minutes)
        var waitingPatients = await GetWaitingPatientsAsync();
        var patientsAhead = waitingPatients.Count(
            pq => pq.CheckInTime < queue.CheckInTime && pq.Priority >= queue.Priority);

        return patientsAhead * 15; // 15 minutes per patient
    }

    public async Task<string> GenerateQueueNumberAsync(DateTime queueDate, string queueType)
    {
        return await _queueRepository.GenerateQueueNumberAsync(queueDate, queueType);
    }
}