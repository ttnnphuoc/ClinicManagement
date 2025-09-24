using Microsoft.EntityFrameworkCore;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;

namespace ClinicManagement.Infrastructure.Repositories;

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Set<Appointment>()
            .Include(a => a.Patient)
            .Include(a => a.Staff)
            .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate && !a.IsDeleted)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId)
    {
        return await _context.Set<Appointment>()
            .Include(a => a.Patient)
            .Include(a => a.Staff)
            .Where(a => a.PatientId == patientId && !a.IsDeleted)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByStaffIdAsync(Guid staffId)
    {
        return await _context.Set<Appointment>()
            .Include(a => a.Patient)
            .Include(a => a.Staff)
            .Where(a => a.StaffId == staffId && !a.IsDeleted)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Set<Appointment>()
            .Include(a => a.Patient)
            .Include(a => a.Staff)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }

    public async Task<bool> IsTimeSlotAvailableAsync(Guid staffId, DateTime appointmentDate, Guid? excludeAppointmentId = null)
    {
        var query = _context.Set<Appointment>()
            .Where(a => a.StaffId == staffId && !a.IsDeleted && a.Status != "Cancelled");

        if (excludeAppointmentId.HasValue)
        {
            query = query.Where(a => a.Id != excludeAppointmentId.Value);
        }

        var existingAppointment = await query
            .FirstOrDefaultAsync(a => a.AppointmentDate == appointmentDate);

        return existingAppointment == null;
    }
}