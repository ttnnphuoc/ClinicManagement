using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Repositories;

public class BillRepository : Repository<Bill>, IBillRepository
{
    public BillRepository(ApplicationDbContext context, IClinicContext clinicContext) 
        : base(context, clinicContext)
    {
    }

    public async Task<Bill?> GetBillWithDetailsAsync(Guid id)
    {
        return await _context.Bills
            .Include(b => b.Patient)
            .Include(b => b.Appointment)
            .Include(b => b.BillItems)
                .ThenInclude(bi => bi.Service)
            .Include(b => b.BillItems)
                .ThenInclude(bi => bi.Medicine)
            .Include(b => b.Payments)
            .Include(b => b.CreatedByStaff)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Bill>> GetBillsByPatientAsync(Guid patientId)
    {
        return await _context.Bills
            .Include(b => b.BillItems)
            .Include(b => b.Payments)
            .Where(b => b.PatientId == patientId)
            .OrderByDescending(b => b.BillDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Bill>> GetPendingBillsAsync()
    {
        return await _context.Bills
            .Include(b => b.Patient)
            .Include(b => b.BillItems)
            .Where(b => b.Status == "Pending" || b.Status == "Partial")
            .OrderBy(b => b.BillDate)
            .ToListAsync();
    }

    public async Task<string> GenerateBillNumberAsync()
    {
        var today = DateTime.Today;
        var todayString = today.ToString("yyyyMMdd");
        
        var lastBill = await _context.Bills
            .Where(b => b.BillNumber.StartsWith($"BILL-{todayString}"))
            .OrderByDescending(b => b.BillNumber)
            .FirstOrDefaultAsync();

        if (lastBill == null)
        {
            return $"BILL-{todayString}-0001";
        }

        var lastNumber = lastBill.BillNumber.Split('-').Last();
        if (int.TryParse(lastNumber, out int number))
        {
            return $"BILL-{todayString}-{(number + 1):D4}";
        }

        return $"BILL-{todayString}-0001";
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Bills.Where(b => b.Status == "Paid");

        if (startDate.HasValue)
            query = query.Where(b => b.BillDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(b => b.BillDate <= endDate.Value);

        return await query.SumAsync(b => b.TotalAmount);
    }
}