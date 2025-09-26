using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetTransactionsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<Transaction?> GetTransactionAsync(Guid id);
    Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task<Transaction> UpdateTransactionAsync(Transaction transaction);
    Task DeleteTransactionAsync(Guid id);
    Task<TransactionSummary> GetTransactionSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<CategorySummary>> GetTransactionsByCategoryAsync(DateTime? startDate = null, DateTime? endDate = null);
}

public class TransactionSummary
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetIncome { get; set; }
    public int TotalTransactions { get; set; }
}

public class CategorySummary
{
    public string Category { get; set; } = string.Empty;
    public decimal RevenueAmount { get; set; }
    public decimal ExpenseAmount { get; set; }
    public int TransactionCount { get; set; }
}