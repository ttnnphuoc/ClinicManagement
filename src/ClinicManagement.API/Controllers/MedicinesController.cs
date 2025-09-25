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
public class MedicinesController : ControllerBase
{
    private readonly IMedicineService _medicineService;
    private readonly IClinicContext _clinicContext;

    public MedicinesController(IMedicineService medicineService, IClinicContext clinicContext)
    {
        _medicineService = medicineService;
        _clinicContext = clinicContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetMedicines()
    {
        var medicines = await _medicineService.GetActiveMedicinesAsync();
        var response = medicines.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchMedicines([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest));
        }

        var medicines = await _medicineService.SearchMedicinesAsync(searchTerm);
        var response = medicines.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedicine(Guid id)
    {
        var medicine = await _medicineService.GetMedicineAsync(id);

        if (medicine == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(medicine)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateMedicine([FromBody] CreateMedicineRequest request)
    {
        var (success, errorCode, medicine) = await _medicineService.CreateMedicineAsync(
            request.Name,
            request.GenericName,
            request.Manufacturer,
            request.Dosage,
            request.Form,
            request.Price,
            request.Description);

        if (!success)
        {
            return BadRequest(ApiResponse.ErrorResponse(errorCode ?? ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(medicine!)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMedicine(Guid id, [FromBody] UpdateMedicineRequest request)
    {
        var (success, errorCode, medicine) = await _medicineService.UpdateMedicineAsync(
            id,
            request.Name,
            request.GenericName,
            request.Manufacturer,
            request.Dosage,
            request.Form,
            request.Price,
            request.Description,
            request.IsActive);

        if (!success)
        {
            return errorCode == "NOT_FOUND" 
                ? NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound))
                : BadRequest(ApiResponse.ErrorResponse(errorCode ?? ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(medicine!)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMedicine(Guid id)
    {
        var (success, errorCode) = await _medicineService.DeleteMedicineAsync(id);

        if (!success)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
    }

    private static MedicineResponse MapToResponse(Medicine medicine) => new()
    {
        Id = medicine.Id,
        Name = medicine.Name,
        GenericName = medicine.GenericName,
        Manufacturer = medicine.Manufacturer,
        Dosage = medicine.Dosage,
        Form = medicine.Form,
        Price = medicine.Price,
        Description = medicine.Description,
        IsActive = medicine.IsActive,
        TotalStock = medicine.InventoryItems?.Sum(i => i.Quantity) ?? 0,
        CreatedAt = medicine.CreatedAt
    };
}