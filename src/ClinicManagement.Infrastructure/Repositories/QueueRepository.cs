using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Repositories;

public class QueueRepository : Repository<PatientQueue>, IQueueRepository
{
    public QueueRepository(ApplicationDbContext context, IClinicContext clinicContext) 
        : base(context, clinicContext)
    {
    }

    public async Task<PatientQueue?> GetQueueWithDetailsAsync(Guid id)
    {
        return await _context.PatientQueues
            .Include(pq => pq.Patient)
            .Include(pq => pq.Appointment)
            .Include(pq => pq.AssignedStaff)
            .Include(pq => pq.Room)
            .FirstOrDefaultAsync(pq => pq.Id == id);
    }

    public async Task<IEnumerable<PatientQueue>> GetTodayQueueAsync()
    {
        var today = DateTime.Today;
        return await _context.PatientQueues
            .Include(pq => pq.Patient)
            .Include(pq => pq.Appointment)
            .Include(pq => pq.AssignedStaff)
            .Where(pq => pq.QueueDate.Date == today)
            .OrderBy(pq => pq.Priority)
            .ThenBy(pq => pq.CheckInTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<PatientQueue>> GetWaitingPatientsAsync()
    {
        return await _context.PatientQueues
            .Include(pq => pq.Patient)
            .Include(pq => pq.Appointment)
            .Where(pq => pq.Status == "Waiting" || pq.Status == "Called")
            .OrderBy(pq => pq.Priority)
            .ThenBy(pq => pq.CheckInTime)
            .ToListAsync();
    }

    public async Task<string> GenerateQueueNumberAsync(DateTime queueDate, string queueType)
    {
        var dateString = queueDate.ToString("yyyyMMdd");
        var prefix = queueType switch
        {
            "Emergency" => "E",
            "WalkIn" => "W",
            _ => "A"
        };

        var lastQueue = await _context.PatientQueues
            .Where(pq => pq.QueueNumber.StartsWith($"{prefix}{dateString}"))
            .OrderByDescending(pq => pq.QueueNumber)
            .FirstOrDefaultAsync();

        if (lastQueue == null)
        {
            return $"{prefix}{dateString}001";
        }

        var lastNumber = lastQueue.QueueNumber.Substring(9);
        if (int.TryParse(lastNumber, out int number))
        {
            return $"{prefix}{dateString}{(number + 1):D3}";
        }

        return $"{prefix}{dateString}001";
    }

    public async Task<PatientQueue?> GetNextPatientAsync(Guid? staffId = null, Guid? roomId = null)
    {
        var query = _context.PatientQueues
            .Include(pq => pq.Patient)
            .Include(pq => pq.Appointment)
            .Where(pq => pq.Status == "Waiting");

        if (staffId.HasValue)
        {
            query = query.Where(pq => !pq.AssignedStaffId.HasValue || pq.AssignedStaffId == staffId);
        }

        if (roomId.HasValue)
        {
            query = query.Where(pq => !pq.RoomId.HasValue || pq.RoomId == roomId);
        }

        return await query
            .OrderByDescending(pq => pq.Priority)
            .ThenBy(pq => pq.CheckInTime)
            .FirstOrDefaultAsync();
    }
}