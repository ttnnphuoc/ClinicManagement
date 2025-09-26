using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Repositories;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext context, IClinicContext clinicContext) : base(context, clinicContext)
    {
    }

    public async Task<IEnumerable<Transaction>> GetByClinicAsync(Guid clinicId)
    {
        return await _context.Transactions
            .Where(t => t.ClinicId == clinicId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(Guid clinicId, DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions
            .Where(t => t.ClinicId == clinicId && t.Date >= startDate && t.Date <= endDate)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByCategoryAsync(Guid clinicId, string category)
    {
        return await _context.Transactions
            .Where(t => t.ClinicId == clinicId && t.Category == category)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByTypeAsync(Guid clinicId, string type)
    {
        return await _context.Transactions
            .Where(t => t.ClinicId == clinicId && t.Type == type)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }
}