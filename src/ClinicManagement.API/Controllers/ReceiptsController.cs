using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.Constants;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Controllers;

[Authorize(Policy = Policies.ManageFinance)]
[ApiController]
[Route("api/[controller]")]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptService _receiptService;
    private readonly IClinicContext _clinicContext;

    public ReceiptsController(IReceiptService receiptService, IClinicContext clinicContext)
    {
        _receiptService = receiptService;
        _clinicContext = clinicContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetReceipts()
    {
        var receipts = await _receiptService.GetReceiptsAsync();
        var response = receipts.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchReceipts([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest));
        }

        var receipts = await _receiptService.SearchReceiptsAsync(searchTerm);
        var response = receipts.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReceipt(Guid id)
    {
        var receipt = await _receiptService.GetReceiptAsync(id);
        if (receipt == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        var response = MapToResponse(receipt);
        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpPost]
    public async Task<IActionResult> CreateReceipt([FromBody] ReceiptDto receiptDto)
    {
        try
        {
            var receipt = await MapToEntity(receiptDto);
            var createdReceipt = await _receiptService.CreateReceiptAsync(receipt);
            var response = MapToResponse(createdReceipt);

            return CreatedAtAction(nameof(GetReceipt), new { id = createdReceipt.Id }, 
                ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReceipt(Guid id, [FromBody] ReceiptDto receiptDto)
    {
        try
        {
            var receipt = await MapToEntity(receiptDto);
            receipt.Id = id;

            var updatedReceipt = await _receiptService.UpdateReceiptAsync(receipt);
            var response = MapToResponse(updatedReceipt);

            return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReceipt(Guid id)
    {
        try
        {
            await _receiptService.DeleteReceiptAsync(id);
            return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadReceipt(Guid id)
    {
        try
        {
            var receipt = await _receiptService.GetReceiptAsync(id);
            if (receipt == null)
            {
                return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
            }

            // TODO: Implement PDF generation
            var pdfBytes = await _receiptService.GenerateReceiptPdfAsync(receipt);
            return File(pdfBytes, "application/pdf", $"receipt-{receipt.ReceiptNumber}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    private ReceiptDto MapToResponse(Receipt receipt)
    {
        return new ReceiptDto
        {
            Id = receipt.Id,
            ReceiptNumber = receipt.ReceiptNumber,
            PatientId = receipt.Bill?.PatientId ?? Guid.Empty,
            PatientName = receipt.Bill?.Patient?.FullName ?? string.Empty,
            Amount = receipt.TotalAmount,
            PaymentMethod = "N/A", // Receipt entity doesn't have payment method directly
            Status = receipt.Status,
            IssuedDate = receipt.ReceiptDate,
            CreatedAt = receipt.CreatedAt
        };
    }

    private async Task<Receipt> MapToEntity(ReceiptDto dto)
    {
        return new Receipt
        {
            Id = dto.Id ?? Guid.NewGuid(),
            ReceiptNumber = dto.ReceiptNumber,
            BillId = Guid.Empty, // This should be provided in the request
            TotalAmount = dto.Amount,
            Status = dto.Status,
            ReceiptDate = dto.IssuedDate,
            ClinicId = _clinicContext.CurrentClinicId ?? throw new InvalidOperationException("No clinic context available"),
            GeneratedByStaffId = GetCurrentUserId(),
            ReceiptType = "Receipt"
        };
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found"));
    }
}