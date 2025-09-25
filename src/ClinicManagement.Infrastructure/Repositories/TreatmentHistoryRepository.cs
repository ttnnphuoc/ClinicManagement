using Microsoft.EntityFrameworkCore;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;

namespace ClinicManagement.Infrastructure.Repositories;

public class TreatmentHistoryRepository : Repository<TreatmentHistory>, ITreatmentHistoryRepository
{
    public TreatmentHistoryRepository(ApplicationDbContext context, IClinicContext clinicContext) : base(context, clinicContext)
    {
    }

    public async Task<IEnumerable<TreatmentHistory>> GetPatientTreatmentHistoryAsync(Guid patientId)
    {
        var query = _context.Set<TreatmentHistory>()
            .Include(th => th.Staff)
            .Include(th => th.Appointment)
            .Where(th => th.PatientId == patientId && !th.IsDeleted);

        // Filter by clinic if user is not SuperAdmin
        if (_clinicContext.CurrentClinicId.HasValue)
        {
            query = query.Where(th => th.ClinicId == _clinicContext.CurrentClinicId.Value);
        }

        return await query
            .OrderByDescending(th => th.TreatmentDate)
            .ToListAsync();
    }

    public async Task<TreatmentHistory?> GetByAppointmentIdAsync(Guid appointmentId)
    {
        var query = _context.Set<TreatmentHistory>()
            .Include(th => th.Patient)
            .Include(th => th.Staff)
            .Include(th => th.Appointment)
            .Where(th => th.AppointmentId == appointmentId && !th.IsDeleted);

        // Filter by clinic if user is not SuperAdmin
        if (_clinicContext.CurrentClinicId.HasValue)
        {
            query = query.Where(th => th.ClinicId == _clinicContext.CurrentClinicId.Value);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<int> GetTotalCountAsync(Guid? patientId = null, string? search = null)
    {
        var query = _context.Set<TreatmentHistory>().Where(th => !th.IsDeleted);

        // Filter by clinic if user is not SuperAdmin
        if (_clinicContext.CurrentClinicId.HasValue)
        {
            query = query.Where(th => th.ClinicId == _clinicContext.CurrentClinicId.Value);
        }

        if (patientId.HasValue)
        {
            query = query.Where(th => th.PatientId == patientId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(th => 
                th.ChiefComplaint!.Contains(search) || 
                th.Diagnosis!.Contains(search) || 
                th.Treatment.Contains(search) ||
                th.Notes!.Contains(search));
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<TreatmentHistory>> SearchTreatmentHistoryAsync(Guid? patientId, string? search, int page, int pageSize)
    {
        var query = _context.Set<TreatmentHistory>()
            .Include(th => th.Patient)
            .Include(th => th.Staff)
            .Include(th => th.Appointment)
            .Where(th => !th.IsDeleted);

        // Filter by clinic if user is not SuperAdmin
        if (_clinicContext.CurrentClinicId.HasValue)
        {
            query = query.Where(th => th.ClinicId == _clinicContext.CurrentClinicId.Value);
        }

        if (patientId.HasValue)
        {
            query = query.Where(th => th.PatientId == patientId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(th => 
                th.ChiefComplaint!.Contains(search) || 
                th.Diagnosis!.Contains(search) || 
                th.Treatment.Contains(search) ||
                th.Notes!.Contains(search));
        }

        return await query
            .OrderByDescending(th => th.TreatmentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}