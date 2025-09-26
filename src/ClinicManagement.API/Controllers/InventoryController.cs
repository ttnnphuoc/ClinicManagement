using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.Constants;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Controllers;

[Authorize(Policy = Policies.ManageInventory)]
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly IMedicineService _medicineService;

    public InventoryController(IInventoryService inventoryService, IMedicineService medicineService)
    {
        _inventoryService = inventoryService;
        _medicineService = medicineService;
    }

    [HttpGet]
    public async Task<IActionResult> GetInventoryItems()
    {
        var items = await _inventoryService.GetAllStockAsync();
        var response = items.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockItems()
    {
        var items = await _inventoryService.GetLowStockItemsAsync();
        var response = items.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("expiring")]
    public async Task<IActionResult> GetExpiringItems([FromQuery] int days = 30)
    {
        var items = await _inventoryService.GetExpiringItemsAsync(days);
        var response = items.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("medicine/{medicineId}")]
    public async Task<IActionResult> GetMedicineStock(Guid medicineId)
    {
        var items = await _inventoryService.GetMedicineStockAsync(medicineId);
        var response = items.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("medicine/{medicineId}/available")]
    public async Task<IActionResult> GetAvailableStock(Guid medicineId)
    {
        var availableStock = await _inventoryService.GetAvailableStockAsync(medicineId);
        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, new { availableStock }));
    }

    [HttpPost("add-stock")]
    public async Task<IActionResult> AddStock([FromBody] AddStockRequest request)
    {
        try
        {
            var result = await _inventoryService.AddStockAsync(
                request.MedicineId,
                request.BatchNumber,
                request.Quantity,
                request.CostPrice,
                request.ExpiryDate,
                request.Supplier);

            if (!result.Success)
            {
                return BadRequest(ApiResponse.ErrorResponse(result.ErrorCode ?? ResponseCodes.Common.BadRequest));
            }

            var response = MapToResponse(result.Item!);
            return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    [HttpPut("{id}/update-stock")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockRequest request)
    {
        try
        {
            var result = await _inventoryService.UpdateStockAsync(id, request.Quantity, request.ReorderLevel);
            
            if (!result.Success)
            {
                return BadRequest(ApiResponse.ErrorResponse(result.ErrorCode ?? ResponseCodes.Common.BadRequest));
            }

            return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    [HttpPost("deduct-stock")]
    public async Task<IActionResult> DeductStock([FromBody] DeductStockRequest request)
    {
        try
        {
            var result = await _inventoryService.DeductStockAsync(
                request.MedicineId,
                request.Quantity,
                request.Reason ?? "Manual deduction");

            if (!result.Success)
            {
                return BadRequest(ApiResponse.ErrorResponse(result.ErrorCode ?? ResponseCodes.Common.BadRequest));
            }

            return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    private InventoryItemDto MapToResponse(InventoryItem item)
    {
        return new InventoryItemDto
        {
            Id = item.Id,
            Name = item.Medicine?.Name ?? "Unknown Medicine",
            Category = "Medicine", // Default category
            CurrentStock = (int)item.Quantity,
            MinimumStock = (int)item.ReorderLevel,
            Unit = "units",
            Price = item.CostPrice,
            Supplier = item.Supplier ?? string.Empty,
            ExpiryDate = item.ExpiryDate,
            LastRestocked = item.ReceivedDate,
            Status = DetermineStatus(item),
            CreatedAt = item.CreatedAt
        };
    }

    private string DetermineStatus(InventoryItem item)
    {
        if (item.Quantity == 0)
            return "out_of_stock";
        else if (item.Quantity <= item.ReorderLevel)
            return "low_stock";
        else
            return "in_stock";
    }

    public class AddStockRequest
    {
        public Guid MedicineId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Supplier { get; set; }
    }

    public class UpdateStockRequest
    {
        public decimal Quantity { get; set; }
        public decimal ReorderLevel { get; set; }
    }

    public class DeductStockRequest
    {
        public Guid MedicineId { get; set; }
        public decimal Quantity { get; set; }
        public string? Reason { get; set; }
    }
}