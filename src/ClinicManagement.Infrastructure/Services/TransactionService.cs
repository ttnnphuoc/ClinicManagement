using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IClinicContext _clinicContext;

    public TransactionService(ITransactionRepository transactionRepository, IClinicContext clinicContext)
    {
        _transactionRepository = transactionRepository;
        _clinicContext = clinicContext;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var clinicId = _clinicContext.CurrentClinicId ?? throw new InvalidOperationException("No clinic context available");
        var transactions = await _transactionRepository.GetByClinicAsync(clinicId);
        
        if (startDate.HasValue)
            transactions = transactions.Where(t => t.Date >= startDate.Value);
        
        if (endDate.HasValue)
            transactions = transactions.Where(t => t.Date <= endDate.Value);
        
        return transactions.OrderByDescending(t => t.Date);
    }

    public async Task<Transaction?> GetTransactionAsync(Guid id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        var clinicId = _clinicContext.CurrentClinicId ?? throw new InvalidOperationException("No clinic context available");
        
        // Ensure transaction belongs to current clinic
        if (transaction?.ClinicId != clinicId)
            return null;
        
        return transaction;
    }

    public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
    {
        var clinicId = _clinicContext.CurrentClinicId ?? throw new InvalidOperationException("No clinic context available");
        transaction.ClinicId = clinicId;
        transaction.CreatedAt = DateTime.UtcNow;
        transaction.UpdatedAt = DateTime.UtcNow;
        
        return await _transactionRepository.AddAsync(transaction);
    }

    public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
    {
        var existing = await GetTransactionAsync(transaction.Id);
        if (existing == null)
            throw new InvalidOperationException("Transaction not found");
        
        var clinicId = _clinicContext.CurrentClinicId ?? throw new InvalidOperationException("No clinic context available");
        transaction.ClinicId = clinicId;
        transaction.UpdatedAt = DateTime.UtcNow;
        transaction.CreatedAt = existing.CreatedAt; // Preserve original creation date
        
        await _transactionRepository.UpdateAsync(transaction);
        await _transactionRepository.SaveChangesAsync();
        return transaction;
    }

    public async Task DeleteTransactionAsync(Guid id)
    {
        var transaction = await GetTransactionAsync(id);
        if (transaction == null)
            throw new InvalidOperationException("Transaction not found");
        
        await _transactionRepository.DeleteAsync(transaction);
    }

    public async Task<TransactionSummary> GetTransactionSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var transactions = await GetTransactionsAsync(startDate, endDate);
        
        var revenue = transactions.Where(t => t.Type == "revenue").Sum(t => t.Amount);
        var expenses = transactions.Where(t => t.Type == "expense").Sum(t => t.Amount);
        
        return new TransactionSummary
        {
            TotalRevenue = revenue,
            TotalExpenses = expenses,
            NetIncome = revenue - expenses,
            TotalTransactions = transactions.Count()
        };
    }

    public async Task<IEnumerable<CategorySummary>> GetTransactionsByCategoryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var transactions = await GetTransactionsAsync(startDate, endDate);
        
        return transactions
            .GroupBy(t => t.Category)
            .Select(g => new CategorySummary
            {
                Category = g.Key,
                RevenueAmount = g.Where(t => t.Type == "revenue").Sum(t => t.Amount),
                ExpenseAmount = g.Where(t => t.Type == "expense").Sum(t => t.Amount),
                TransactionCount = g.Count()
            })
            .OrderByDescending(c => c.RevenueAmount + c.ExpenseAmount);
    }
}