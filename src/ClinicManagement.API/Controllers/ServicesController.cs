using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.Constants;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IClinicContext _clinicContext;

    public ServicesController(IServiceRepository serviceRepository, IClinicContext clinicContext)
    {
        _serviceRepository = serviceRepository;
        _clinicContext = clinicContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetServices([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var services = await _serviceRepository.SearchServicesAsync(search, page, pageSize);
        var total = await _serviceRepository.GetTotalCountAsync(search);

        var response = new
        {
            items = services.Select(s => MapToResponse(s)),
            total,
            page,
            pageSize
        };

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveServices()
    {
        var services = await _serviceRepository.GetActiveServicesAsync();
        var response = services.Select(s => MapToResponse(s));

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetService(Guid id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);

        if (service == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(service)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request)
    {
        var clinicId = _clinicContext.CurrentClinicId!.Value;

        var service = new Service
        {
            ClinicId = clinicId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            DurationMinutes = request.DurationMinutes,
            IsActive = request.IsActive
        };

        await _serviceRepository.AddAsync(service);
        await _serviceRepository.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(service)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(Guid id, [FromBody] UpdateServiceRequest request)
    {
        var service = await _serviceRepository.GetByIdAsync(id);

        if (service == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        service.Name = request.Name;
        service.Description = request.Description;
        service.Price = request.Price;
        service.DurationMinutes = request.DurationMinutes;
        service.IsActive = request.IsActive;

        await _serviceRepository.UpdateAsync(service);
        await _serviceRepository.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(service)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);

        if (service == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        await _serviceRepository.SoftDeleteAsync(service);
        await _serviceRepository.SaveChangesAsync();

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