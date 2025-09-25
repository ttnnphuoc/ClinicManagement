using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IBillService
{
    Task<(bool Success, string? ErrorCode, Bill? Bill)> CreateBillAsync(
        Guid patientId,
        Guid? appointmentId,
        IEnumerable<BillItem> items,
        decimal discountAmount = 0,
        decimal discountPercentage = 0,
        string? notes = null);

    Task<(bool Success, string? ErrorCode, Bill? Bill)> UpdateBillAsync(
        Guid billId,
        IEnumerable<BillItem> items,
        decimal discountAmount = 0,
        decimal discountPercentage = 0,
        string? notes = null);

    Task<(bool Success, string? ErrorCode)> ProcessPaymentAsync(
        Guid billId,
        decimal amount,
        string paymentMethod,
        string? reference = null,
        string? notes = null);

    Task<Bill?> GetBillAsync(Guid billId);
    Task<IEnumerable<Bill>> GetPatientBillsAsync(Guid patientId);
    Task<IEnumerable<Bill>> GetPendingBillsAsync();
    Task<decimal> CalculateTotalAmount(IEnumerable<BillItem> items, decimal discountAmount, decimal discountPercentage);
}