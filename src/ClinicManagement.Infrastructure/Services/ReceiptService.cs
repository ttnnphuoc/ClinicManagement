using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class ReceiptService : IReceiptService
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly IBillRepository _billRepository;
    private readonly IClinicContext _clinicContext;

    public ReceiptService(
        IReceiptRepository receiptRepository,
        IBillRepository billRepository,
        IClinicContext clinicContext)
    {
        _receiptRepository = receiptRepository;
        _billRepository = billRepository;
        _clinicContext = clinicContext;
    }

    public async Task<(bool Success, string? ErrorCode, Receipt? Receipt)> GenerateReceiptAsync(
        Guid billId,
        string receiptType = "Receipt",
        bool sendEmail = false)
    {
        var bill = await _billRepository.GetBillWithDetailsAsync(billId);
        if (bill == null)
        {
            return (false, "BILL_NOT_FOUND", null);
        }

        if (bill.Status != "Paid" && receiptType == "Receipt")
        {
            return (false, "BILL_NOT_PAID", null);
        }

        var receipt = new Receipt
        {
            ClinicId = _clinicContext.CurrentClinicId!.Value,
            BillId = billId,
            ReceiptNumber = await _receiptRepository.GenerateReceiptNumberAsync(receiptType),
            ReceiptDate = DateTime.UtcNow,
            ReceiptType = receiptType,
            TotalAmount = bill.TotalAmount,
            Status = "Generated",
            CustomerEmail = bill.Patient.Email,
            CustomerPhone = bill.Patient.PhoneNumber,
            GeneratedByStaffId = _clinicContext.CurrentUserId ?? Guid.Empty
        };

        await _receiptRepository.AddAsync(receipt);
        await _receiptRepository.SaveChangesAsync();

        if (sendEmail && !string.IsNullOrEmpty(receipt.CustomerEmail))
        {
            await SendReceiptEmailAsync(receipt.Id, receipt.CustomerEmail);
        }

        var created = await _receiptRepository.GetReceiptWithDetailsAsync(receipt.Id);
        return (true, null, created);
    }

    public async Task<(bool Success, string? ErrorCode, byte[]? PdfData)> GenerateReceiptPdfAsync(Guid receiptId)
    {
        var receipt = await _receiptRepository.GetReceiptWithDetailsAsync(receiptId);
        if (receipt == null)
        {
            return (false, "RECEIPT_NOT_FOUND", null);
        }

        // TODO: Implement PDF generation logic
        // This would typically use a library like iTextSharp or PdfSharp
        // For now, return a placeholder
        var pdfData = System.Text.Encoding.UTF8.GetBytes($"Receipt PDF for {receipt.ReceiptNumber}");
        
        return (true, null, pdfData);
    }

    public async Task<(bool Success, string? ErrorCode)> SendReceiptEmailAsync(Guid receiptId, string email)
    {
        var receipt = await _receiptRepository.GetReceiptWithDetailsAsync(receiptId);
        if (receipt == null)
        {
            return (false, "RECEIPT_NOT_FOUND");
        }

        // TODO: Implement email sending logic
        // This would typically use an email service like SendGrid or SMTP
        
        receipt.IsEmailSent = true;
        receipt.EmailSentDate = DateTime.UtcNow;
        receipt.Status = "Sent";

        await _receiptRepository.UpdateAsync(receipt);
        await _receiptRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<Receipt?> GetReceiptAsync(Guid receiptId)
    {
        return await _receiptRepository.GetReceiptWithDetailsAsync(receiptId);
    }

    public async Task<IEnumerable<Receipt>> GetReceiptsByBillAsync(Guid billId)
    {
        return await _receiptRepository.GetReceiptsByBillAsync(billId);
    }

    public async Task<IEnumerable<Receipt>> GetReceiptsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _receiptRepository.GetReceiptsByDateRangeAsync(startDate, endDate);
    }

    // Additional methods for CRUD operations
    public async Task<IEnumerable<Receipt>> GetReceiptsAsync()
    {
        return await _receiptRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Receipt>> SearchReceiptsAsync(string searchTerm)
    {
        var receipts = await _receiptRepository.GetAllAsync();
        return receipts.Where(r => 
            r.ReceiptNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            (r.Bill?.Patient?.FullName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
    }

    public async Task<Receipt> CreateReceiptAsync(Receipt receipt)
    {
        receipt.CreatedAt = DateTime.UtcNow;
        receipt.UpdatedAt = DateTime.UtcNow;
        return await _receiptRepository.AddAsync(receipt);
    }

    public async Task<Receipt> UpdateReceiptAsync(Receipt receipt)
    {
        receipt.UpdatedAt = DateTime.UtcNow;
        await _receiptRepository.UpdateAsync(receipt);
        await _receiptRepository.SaveChangesAsync();
        return receipt;
    }

    public async Task DeleteReceiptAsync(Guid receiptId)
    {
        var receipt = await _receiptRepository.GetByIdAsync(receiptId);
        if (receipt != null)
        {
            await _receiptRepository.DeleteAsync(receipt);
            await _receiptRepository.SaveChangesAsync();
        }
    }

    public async Task<byte[]> GenerateReceiptPdfAsync(Receipt receipt)
    {
        // TODO: Implement PDF generation for receipt
        // This is a placeholder - you would use a PDF library like iTextSharp or PdfSharp
        var content = $"Receipt #{receipt.ReceiptNumber}\nAmount: {receipt.TotalAmount:C}\nDate: {receipt.ReceiptDate:yyyy-MM-dd}";
        return System.Text.Encoding.UTF8.GetBytes(content);
    }
}