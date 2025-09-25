using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IBillRepository : IRepository<Bill>
{
    Task<Bill?> GetBillWithDetailsAsync(Guid id);
    Task<IEnumerable<Bill>> GetBillsByPatientAsync(Guid patientId);
    Task<IEnumerable<Bill>> GetPendingBillsAsync();
    Task<string> GenerateBillNumberAsync();
    Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
}