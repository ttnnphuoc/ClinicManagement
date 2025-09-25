using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.Constants;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Controllers;

[Authorize(Policy = Policies.ManageBills)]
[ApiController]
[Route("api/[controller]")]
public class BillsController : ControllerBase
{
    private readonly IBillService _billService;
    private readonly IClinicContext _clinicContext;

    public BillsController(IBillService billService, IClinicContext clinicContext)
    {
        _billService = billService;
        _clinicContext = clinicContext;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBill(Guid id)
    {
        var bill = await _billService.GetBillAsync(id);

        if (bill == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(bill)));
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientBills(Guid patientId)
    {
        var bills = await _billService.GetPatientBillsAsync(patientId);
        var response = bills.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingBills()
    {
        var bills = await _billService.GetPendingBillsAsync();
        var response = bills.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpPost]
    public async Task<IActionResult> CreateBill([FromBody] CreateBillRequest request)
    {
        var billItems = request.Items.Select(item => new BillItem
        {
            ServiceId = item.ServiceId,
            MedicineId = item.MedicineId,
            ItemName = item.ItemName,
            ItemType = item.ItemType,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.Quantity * item.UnitPrice,
            Notes = item.Notes
        });

        var (success, errorCode, bill) = await _billService.CreateBillAsync(
            request.PatientId,
            request.AppointmentId,
            billItems,
            request.DiscountAmount,
            request.DiscountPercentage,
            request.Notes);

        if (!success)
        {
            return BadRequest(ApiResponse.ErrorResponse(errorCode ?? ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(bill!)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBill(Guid id, [FromBody] UpdateBillRequest request)
    {
        var billItems = request.Items.Select(item => new BillItem
        {
            ServiceId = item.ServiceId,
            MedicineId = item.MedicineId,
            ItemName = item.ItemName,
            ItemType = item.ItemType,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.Quantity * item.UnitPrice,
            Notes = item.Notes
        });

        var (success, errorCode, bill) = await _billService.UpdateBillAsync(
            id,
            billItems,
            request.DiscountAmount,
            request.DiscountPercentage,
            request.Notes);

        if (!success)
        {
            return errorCode == "NOT_FOUND" 
                ? NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound))
                : BadRequest(ApiResponse.ErrorResponse(errorCode ?? ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(bill!)));
    }

    [HttpPost("{id}/payments")]
    public async Task<IActionResult> ProcessPayment(Guid id, [FromBody] ProcessPaymentRequest request)
    {
        var (success, errorCode) = await _billService.ProcessPaymentAsync(
            id,
            request.Amount,
            request.PaymentMethod,
            request.Reference,
            request.Notes);

        if (!success)
        {
            return errorCode == "NOT_FOUND" 
                ? NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound))
                : BadRequest(ApiResponse.ErrorResponse(errorCode ?? ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
    }

    private static BillResponse MapToResponse(Bill bill) => new()
    {
        Id = bill.Id,
        ClinicId = bill.ClinicId,
        PatientId = bill.PatientId,
        PatientName = bill.Patient?.FullName ?? string.Empty,
        AppointmentId = bill.AppointmentId,
        BillNumber = bill.BillNumber,
        BillDate = bill.BillDate,
        SubTotal = bill.SubTotal,
        DiscountAmount = bill.DiscountAmount,
        DiscountPercentage = bill.DiscountPercentage,
        TotalAmount = bill.TotalAmount,
        Status = bill.Status,
        PaymentMethod = bill.PaymentMethod,
        Notes = bill.Notes,
        Items = bill.BillItems?.Select(MapBillItemToResponse) ?? Enumerable.Empty<BillItemResponse>(),
        Payments = bill.Payments?.Select(MapPaymentToResponse) ?? Enumerable.Empty<PaymentResponse>(),
        CreatedAt = bill.CreatedAt
    };

    private static BillItemResponse MapBillItemToResponse(BillItem item) => new()
    {
        Id = item.Id,
        ServiceId = item.ServiceId,
        ServiceName = item.Service?.Name ?? string.Empty,
        MedicineId = item.MedicineId,
        MedicineName = item.Medicine?.Name ?? string.Empty,
        ItemName = item.ItemName,
        ItemType = item.ItemType,
        Quantity = item.Quantity,
        UnitPrice = item.UnitPrice,
        TotalPrice = item.TotalPrice,
        Notes = item.Notes
    };

    private static PaymentResponse MapPaymentToResponse(Payment payment) => new()
    {
        Id = payment.Id,
        PaymentNumber = payment.PaymentNumber,
        PaymentDate = payment.PaymentDate,
        Amount = payment.Amount,
        PaymentMethod = payment.PaymentMethod,
        Reference = payment.Reference,
        Status = payment.Status,
        Notes = payment.Notes,
        ReceivedByStaffName = payment.ReceivedByStaff?.FullName ?? string.Empty
    };
}