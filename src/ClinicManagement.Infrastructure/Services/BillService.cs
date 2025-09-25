using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class BillService : IBillService
{
    private readonly IBillRepository _billRepository;
    private readonly IClinicContext _clinicContext;

    public BillService(IBillRepository billRepository, IClinicContext clinicContext)
    {
        _billRepository = billRepository;
        _clinicContext = clinicContext;
    }

    public async Task<(bool Success, string? ErrorCode, Bill? Bill)> CreateBillAsync(
        Guid patientId,
        Guid? appointmentId,
        IEnumerable<BillItem> items,
        decimal discountAmount = 0,
        decimal discountPercentage = 0,
        string? notes = null)
    {
        if (!items.Any())
        {
            return (false, "NO_ITEMS", null);
        }

        var billItems = items.ToList();
        var subTotal = billItems.Sum(i => i.TotalPrice);
        var totalAmount = await CalculateTotalAmount(billItems, discountAmount, discountPercentage);

        var bill = new Bill
        {
            ClinicId = _clinicContext.CurrentClinicId!.Value,
            PatientId = patientId,
            AppointmentId = appointmentId,
            BillNumber = await _billRepository.GenerateBillNumberAsync(),
            BillDate = DateTime.UtcNow,
            SubTotal = subTotal,
            DiscountAmount = discountAmount,
            DiscountPercentage = discountPercentage,
            TotalAmount = totalAmount,
            Status = "Pending",
            Notes = notes,
            CreatedByStaffId = _clinicContext.CurrentUserId ?? Guid.Empty
        };

        // Set bill items
        foreach (var item in billItems)
        {
            item.BillId = bill.Id;
            bill.BillItems.Add(item);
        }

        await _billRepository.AddAsync(bill);
        await _billRepository.SaveChangesAsync();

        var created = await _billRepository.GetBillWithDetailsAsync(bill.Id);
        return (true, null, created);
    }

    public async Task<(bool Success, string? ErrorCode, Bill? Bill)> UpdateBillAsync(
        Guid billId,
        IEnumerable<BillItem> items,
        decimal discountAmount = 0,
        decimal discountPercentage = 0,
        string? notes = null)
    {
        var bill = await _billRepository.GetBillWithDetailsAsync(billId);
        if (bill == null)
        {
            return (false, "NOT_FOUND", null);
        }

        if (bill.Status == "Paid")
        {
            return (false, "BILL_ALREADY_PAID", null);
        }

        var billItems = items.ToList();
        var subTotal = billItems.Sum(i => i.TotalPrice);
        var totalAmount = await CalculateTotalAmount(billItems, discountAmount, discountPercentage);

        // Clear existing items
        bill.BillItems.Clear();

        // Add new items
        foreach (var item in billItems)
        {
            item.BillId = bill.Id;
            bill.BillItems.Add(item);
        }

        bill.SubTotal = subTotal;
        bill.DiscountAmount = discountAmount;
        bill.DiscountPercentage = discountPercentage;
        bill.TotalAmount = totalAmount;
        bill.Notes = notes;

        await _billRepository.UpdateAsync(bill);
        await _billRepository.SaveChangesAsync();

        var updated = await _billRepository.GetBillWithDetailsAsync(billId);
        return (true, null, updated);
    }

    public async Task<(bool Success, string? ErrorCode)> ProcessPaymentAsync(
        Guid billId,
        decimal amount,
        string paymentMethod,
        string? reference = null,
        string? notes = null)
    {
        var bill = await _billRepository.GetBillWithDetailsAsync(billId);
        if (bill == null)
        {
            return (false, "NOT_FOUND");
        }

        if (amount <= 0)
        {
            return (false, "INVALID_AMOUNT");
        }

        var totalPaid = bill.Payments.Where(p => p.Status == "Completed").Sum(p => p.Amount);
        var remainingAmount = bill.TotalAmount - totalPaid;

        if (amount > remainingAmount)
        {
            return (false, "AMOUNT_EXCEEDS_REMAINING");
        }

        var payment = new Payment
        {
            ClinicId = _clinicContext.CurrentClinicId!.Value,
            BillId = billId,
            PaymentNumber = await GeneratePaymentNumberAsync(),
            PaymentDate = DateTime.UtcNow,
            Amount = amount,
            PaymentMethod = paymentMethod,
            Reference = reference,
            Status = "Completed",
            Notes = notes,
            ReceivedByStaffId = _clinicContext.CurrentUserId ?? Guid.Empty
        };

        bill.Payments.Add(payment);

        // Update bill status
        var newTotalPaid = totalPaid + amount;
        if (newTotalPaid >= bill.TotalAmount)
        {
            bill.Status = "Paid";
            bill.PaymentMethod = paymentMethod;
        }
        else if (newTotalPaid > 0)
        {
            bill.Status = "Partial";
        }

        await _billRepository.UpdateAsync(bill);
        await _billRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<Bill?> GetBillAsync(Guid billId)
    {
        return await _billRepository.GetBillWithDetailsAsync(billId);
    }

    public async Task<IEnumerable<Bill>> GetPatientBillsAsync(Guid patientId)
    {
        return await _billRepository.GetBillsByPatientAsync(patientId);
    }

    public async Task<IEnumerable<Bill>> GetPendingBillsAsync()
    {
        return await _billRepository.GetPendingBillsAsync();
    }

    public Task<decimal> CalculateTotalAmount(IEnumerable<BillItem> items, decimal discountAmount, decimal discountPercentage)
    {
        var subTotal = items.Sum(i => i.TotalPrice);
        var percentageDiscount = subTotal * (discountPercentage / 100);
        var totalDiscount = discountAmount + percentageDiscount;
        
        return Task.FromResult(Math.Max(0, subTotal - totalDiscount));
    }

    private Task<string> GeneratePaymentNumberAsync()
    {
        var today = DateTime.Today;
        var todayString = today.ToString("yyyyMMdd");
        return Task.FromResult($"PAY-{todayString}-{DateTime.Now.Ticks.ToString().Substring(8)}");
    }
}