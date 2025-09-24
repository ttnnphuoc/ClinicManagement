using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.Constants;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Controllers;

[Authorize(Policy = Policies.ManageServices)]
[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IMedicalServiceService _medicalServiceService;
    private readonly IClinicContext _clinicContext;

    public ServicesController(IMedicalServiceService medicalServiceService, IClinicContext clinicContext)
    {
        _medicalServiceService = medicalServiceService;
        _clinicContext = clinicContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetServices([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (items, total) = await _medicalServiceService.SearchServicesAsync(search, page, pageSize);

        var response = new
        {
            items = items.Select(s => MapToResponse(s)),
            total,
            page,
            pageSize
        };

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveServices()
    {
        var services = await _medicalServiceService.GetActiveServicesAsync();
        var response = services.Select(s => MapToResponse(s));

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetService(Guid id)
    {
        var service = await _medicalServiceService.GetServiceByIdAsync(id);

        if (service == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(service)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request)
    {
        var (success, errorCode, service) = await _medicalServiceService.CreateServiceAsync(
            _clinicContext.CurrentClinicId,
            request.Name,
            request.Description,
            request.Price,
            request.DurationMinutes,
            request.IsActive);

        if (!success)
        {
            return errorCode == "AUTH_UNAUTHORIZED"
                ? BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Auth.Unauthorized))
                : BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(service!)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(Guid id, [FromBody] UpdateServiceRequest request)
    {
        var (success, errorCode, service) = await _medicalServiceService.UpdateServiceAsync(
            id,
            request.Name,
            request.Description,
            request.Price,
            request.DurationMinutes,
            request.IsActive);

        if (!success)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(service!)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        var (success, errorCode) = await _medicalServiceService.DeleteServiceAsync(id);

        if (!success)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
    }

    private static ServiceResponse MapToResponse(Service s) => new()
    {
        Id = s.Id,
        ClinicId = s.ClinicId,
        Name = s.Name,
        Description = s.Description,
        Price = s.Price,
        DurationMinutes = s.DurationMinutes,
        IsActive = s.IsActive,
        CreatedAt = s.CreatedAt
    };
}