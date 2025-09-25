using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.Constants;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.API.Controllers;

[Authorize(Policy = Policies.ManageAppointments)]
[ApiController]
[Route("api/[controller]")]
public class QueueController : ControllerBase
{
    private readonly IQueueService _queueService;
    private readonly IClinicContext _clinicContext;

    public QueueController(IQueueService queueService, IClinicContext clinicContext)
    {
        _queueService = queueService;
        _clinicContext = clinicContext;
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetTodayQueue()
    {
        var queue = await _queueService.GetTodayQueueAsync();
        var response = new List<PatientQueueResponse>();

        foreach (var item in queue)
        {
            var estimatedWait = await _queueService.GetEstimatedWaitTimeAsync(item.Id);
            response.Add(MapToResponse(item, estimatedWait));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("waiting")]
    public async Task<IActionResult> GetWaitingPatients()
    {
        var queue = await _queueService.GetWaitingPatientsAsync();
        var response = new List<PatientQueueResponse>();

        foreach (var item in queue)
        {
            var estimatedWait = await _queueService.GetEstimatedWaitTimeAsync(item.Id);
            response.Add(MapToResponse(item, estimatedWait));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddPatientToQueue([FromBody] AddPatientToQueueRequest request)
    {
        var (success, errorCode, queue) = await _queueService.AddPatientToQueueAsync(
            request.PatientId,
            request.AppointmentId,
            request.QueueType,
            request.Priority,
            request.Notes);

        if (!success)
        {
            return BadRequest(ApiResponse.ErrorResponse(errorCode ?? ResponseCodes.Common.BadRequest));
        }

        var estimatedWait = await _queueService.GetEstimatedWaitTimeAsync(queue!.Id);
        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(queue, estimatedWait)));
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateQueueStatus(Guid id, [FromBody] UpdateQueueStatusRequest request)
    {
        var (success, errorCode) = await _queueService.UpdateQueueStatusAsync(
            id,
            request.Status,
            request.AssignedStaffId,
            request.RoomId);

        if (!success)
        {
            return errorCode == "QUEUE_NOT_FOUND"
                ? NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound))
                : BadRequest(ApiResponse.ErrorResponse(errorCode ?? ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
    }

    [HttpPost("call-next")]
    public async Task<IActionResult> CallNextPatient([FromQuery] Guid? staffId = null, [FromQuery] Guid? roomId = null)
    {
        var (success, errorCode) = await _queueService.CallNextPatientAsync(staffId, roomId);

        if (!success)
        {
            return BadRequest(ApiResponse.ErrorResponse(errorCode ?? ResponseCodes.Common.BadRequest));
        }

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQueueItem(Guid id)
    {
        var queue = await _queueService.GetQueueItemAsync(id);

        if (queue == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        var estimatedWait = await _queueService.GetEstimatedWaitTimeAsync(id);
        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, MapToResponse(queue, estimatedWait)));
    }

    private static PatientQueueResponse MapToResponse(PatientQueue queue, int estimatedWaitTime) => new()
    {
        Id = queue.Id,
        PatientId = queue.PatientId,
        PatientName = queue.Patient?.FullName ?? string.Empty,
        AppointmentId = queue.AppointmentId,
        QueueNumber = queue.QueueNumber,
        QueueDate = queue.QueueDate,
        CheckInTime = queue.CheckInTime,
        CalledTime = queue.CalledTime,
        StartTime = queue.StartTime,
        CompletionTime = queue.CompletionTime,
        Status = queue.Status,
        QueueType = queue.QueueType,
        Priority = queue.Priority,
        AssignedStaffName = queue.AssignedStaff?.FullName,
        RoomName = queue.Room?.RoomName,
        Notes = queue.Notes,
        EstimatedWaitTime = estimatedWaitTime
    };
}