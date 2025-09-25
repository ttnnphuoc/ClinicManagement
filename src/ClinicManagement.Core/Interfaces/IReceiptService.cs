using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IReceiptService
{
    Task<(bool Success, string? ErrorCode, Receipt? Receipt)> GenerateReceiptAsync(
        Guid billId, 
        string receiptType = "Receipt",
        bool sendEmail = false);

    Task<(bool Success, string? ErrorCode, byte[]? PdfData)> GenerateReceiptPdfAsync(Guid receiptId);
    
    Task<(bool Success, string? ErrorCode)> SendReceiptEmailAsync(Guid receiptId, string email);
    
    Task<Receipt?> GetReceiptAsync(Guid receiptId);
    
    Task<IEnumerable<Receipt>> GetReceiptsByBillAsync(Guid billId);
    
    Task<IEnumerable<Receipt>> GetReceiptsByDateRangeAsync(DateTime startDate, DateTime endDate);
}