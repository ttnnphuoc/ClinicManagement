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
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IClinicContext _clinicContext;

    public TransactionsController(ITransactionService transactionService, IClinicContext clinicContext)
    {
        _transactionService = transactionService;
        _clinicContext = clinicContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var transactions = await _transactionService.GetTransactionsAsync(startDate, endDate);
        var response = transactions.Select(MapToResponse);

        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetTransactionSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var summary = await _transactionService.GetTransactionSummaryAsync(startDate, endDate);
        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, summary));
    }

    [HttpGet("by-category")]
    public async Task<IActionResult> GetTransactionsByCategory([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var categoryData = await _transactionService.GetTransactionsByCategoryAsync(startDate, endDate);
        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, categoryData));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaction(Guid id)
    {
        var transaction = await _transactionService.GetTransactionAsync(id);
        if (transaction == null)
        {
            return NotFound(ApiResponse.ErrorResponse(ResponseCodes.Common.NotFound));
        }

        var response = MapToResponse(transaction);
        return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto transactionDto)
    {
        try
        {
            var transaction = await MapToEntity(transactionDto);
            var createdTransaction = await _transactionService.CreateTransactionAsync(transaction);
            var response = MapToResponse(createdTransaction);

            return CreatedAtAction(nameof(GetTransaction), new { id = createdTransaction.Id }, 
                ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] TransactionDto transactionDto)
    {
        try
        {
            var transaction = await MapToEntity(transactionDto);
            transaction.Id = id;

            var updatedTransaction = await _transactionService.UpdateTransactionAsync(transaction);
            var response = MapToResponse(updatedTransaction);

            return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success, response));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        try
        {
            await _transactionService.DeleteTransactionAsync(id);
            return Ok(ApiResponse.SuccessResponse(ResponseCodes.Common.Success));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ResponseCodes.Common.BadRequest, ex.Message));
        }
    }

    private TransactionDto MapToResponse(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            Description = transaction.Description,
            Type = transaction.Type,
            Category = transaction.Category,
            Amount = transaction.Amount,
            Date = transaction.Date,
            PaymentMethod = transaction.PaymentMethod,
            Reference = transaction.Reference,
            CreatedAt = transaction.CreatedAt
        };
    }

    private async Task<Transaction> MapToEntity(TransactionDto dto)
    {
        return new Transaction
        {
            Id = dto.Id ?? Guid.NewGuid(),
            Description = dto.Description,
            Type = dto.Type,
            Category = dto.Category,
            Amount = dto.Amount,
            Date = dto.Date,
            PaymentMethod = dto.PaymentMethod,
            Reference = dto.Reference,
            ClinicId = _clinicContext.CurrentClinicId ?? throw new InvalidOperationException("No clinic context available")
        };
    }
}