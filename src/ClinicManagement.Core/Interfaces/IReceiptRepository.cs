using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IReceiptRepository : IRepository<Receipt>
{
    Task<Receipt?> GetReceiptWithDetailsAsync(Guid id);
    Task<IEnumerable<Receipt>> GetReceiptsByBillAsync(Guid billId);
    Task<IEnumerable<Receipt>> GetReceiptsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<string> GenerateReceiptNumberAsync(string receiptType);
}