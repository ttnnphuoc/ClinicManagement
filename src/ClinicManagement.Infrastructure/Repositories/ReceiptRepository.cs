using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Repositories;

public class ReceiptRepository : Repository<Receipt>, IReceiptRepository
{
    public ReceiptRepository(ApplicationDbContext context, IClinicContext clinicContext) 
        : base(context, clinicContext)
    {
    }

    public async Task<Receipt?> GetReceiptWithDetailsAsync(Guid id)
    {
        return await _context.Receipts
            .Include(r => r.Bill)
                .ThenInclude(b => b.Patient)
            .Include(r => r.Bill)
                .ThenInclude(b => b.BillItems)
                    .ThenInclude(bi => bi.Service)
            .Include(r => r.Bill)
                .ThenInclude(b => b.BillItems)
                    .ThenInclude(bi => bi.Medicine)
            .Include(r => r.GeneratedByStaff)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Receipt>> GetReceiptsByBillAsync(Guid billId)
    {
        return await _context.Receipts
            .Where(r => r.BillId == billId)
            .OrderByDescending(r => r.ReceiptDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Receipt>> GetReceiptsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Receipts
            .Include(r => r.Bill)
                .ThenInclude(b => b.Patient)
            .Where(r => r.ReceiptDate >= startDate && r.ReceiptDate <= endDate)
            .OrderByDescending(r => r.ReceiptDate)
            .ToListAsync();
    }

    public async Task<string> GenerateReceiptNumberAsync(string receiptType)
    {
        var today = DateTime.Today;
        var todayString = today.ToString("yyyyMMdd");
        var prefix = receiptType == "Invoice" ? "INV" : "REC";
        
        var lastReceipt = await _context.Receipts
            .Where(r => r.ReceiptNumber.StartsWith($"{prefix}-{todayString}"))
            .OrderByDescending(r => r.ReceiptNumber)
            .FirstOrDefaultAsync();

        if (lastReceipt == null)
        {
            return $"{prefix}-{todayString}-0001";
        }

        var lastNumber = lastReceipt.ReceiptNumber.Split('-').Last();
        if (int.TryParse(lastNumber, out int number))
        {
            return $"{prefix}-{todayString}-{(number + 1):D4}";
        }

        return $"{prefix}-{todayString}-0001";
    }
}