using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetByClinicAsync(Guid clinicId);
    Task<IEnumerable<Transaction>> GetByDateRangeAsync(Guid clinicId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Transaction>> GetByCategoryAsync(Guid clinicId, string category);
    Task<IEnumerable<Transaction>> GetByTypeAsync(Guid clinicId, string type);
}