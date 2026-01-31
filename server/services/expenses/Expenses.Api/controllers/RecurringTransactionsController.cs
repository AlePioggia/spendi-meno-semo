using Expenses.Api.dto.request.recurringTransaction;
using Expenses.Api.dto.response.recurringTransaction;
using Expenses.Application.commands.recurringTransactions;
using Expenses.Application.queries.recurringTransactions;
using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Expenses.Api.controllers
{
    [Route("api/recurringTransaction")]
    [ApiController]
    public class RecurringTransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecurringTransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetRecurringTransactionResponseDto>>> GetRecurringTransactionsAsync()
        {
            try
            {
                var recurringOperations = await _mediator.Send(new GetRecurringTransactionsQuery());
                if (recurringOperations is null)
                {
                    return NotFound();
                }

                var mapped = recurringOperations.Select(ro => new GetRecurringTransactionResponseDto(
                    ro.Id,
                    ro.Description ?? "",
                    ro.Frequency.ToString(),
                    ro.StartDate,
                    ro.EndDate,
                    ro.CategoryId,
                    ro.Template?.Description ?? "",
                    ro.Template?.Amount?.Amount ?? 0,
                    ro.Template?.Amount?.Currency.ToString() ?? "",
                    ro.Template?.TransactionType.ToString() ?? "",
                    ro.Template?.Date ?? default,
                    ro.CreatedAt
                )).ToList();

                return Ok(mapped);
            }
            catch (ValidationException vex)
            {
                return BadRequest(new { Errors = vex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetRecurringTransactionResponseDto>> GetRecurringTransactionByIdAsync(long id)
        {
            try
            {
                var ro = await _mediator.Send(new GetRecurringTransactionByIdQuery(id));
                if (ro is null)
                {
                    return NotFound();
                }

                var mapped = new GetRecurringTransactionResponseDto(
                    ro.Id,
                    ro.Description ?? "",
                    ro.Frequency.ToString(),
                    ro.StartDate,
                    ro.EndDate,
                    ro.CategoryId,
                    ro.Template?.Description ?? "",
                    ro.Template?.Amount?.Amount ?? 0,
                    ro.Template?.Amount?.Currency.ToString() ?? "",
                    ro.Template?.TransactionType.ToString() ?? "",
                    ro.Template?.Date ?? default,
                    ro.CreatedAt
                );

                return Ok(mapped);
            }
            catch (ValidationException vex)
            {
                return BadRequest(new { Errors = vex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecurringTransactionAsync([FromBody] RecurringOperationUpsertRequestDto request)
        {
            try
            {
                if (!Enum.TryParse<TransactionType>(request.Template.TransactionType, true, out var transactionType))
                {
                    return BadRequest($"TransactionType '{request.Template.TransactionType}' non valido.");
                }

                if (!Enum.TryParse<Currency>(request.Template.Currency, true, out var currency))
                {
                    return BadRequest($"Currency '{request.Template.Currency}' non valida.");
                }

                if (!Enum.TryParse<RecurringOperationFrequency>(request.Frequency, true, out var frequency))
                {
                    return BadRequest($"RecurringOperationFrequency '{request.Frequency}' non valida.");
                }

                var command = new CreateRecurringTransactionCommand(
                    request.Description,
                    request.Template.Description,
                    currency,
                    request.Template.Amount,
                    transactionType,
                    request.CategoryId,
                    frequency,
                    request.StartDate,
                    request.EndDate,
                    request.Template.Date
                );

                await _mediator.Send(command);
                return Ok();
            }
            catch (ValidationException vex)
            {
                return BadRequest(new { Errors = vex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecurringTransactionAsync(long id, [FromBody] RecurringOperationUpsertRequestDto request)
        {
            try
            {
                if (request.Id.HasValue && id != request.Id.Value)
                {
                    return BadRequest("Id mismatch.");
                }

                var requestId = request.Id ?? id;

                if (!Enum.TryParse<TransactionType>(request.Template.TransactionType, true, out var transactionType))
                {
                    return BadRequest($"TransactionType '{request.Template.TransactionType}' non valido.");
                }

                if (!Enum.TryParse<Currency>(request.Template.Currency, true, out var currency))
                {
                    return BadRequest($"Currency '{request.Template.Currency}' non valida.");
                }

                if (!Enum.TryParse<RecurringOperationFrequency>(request.Frequency, true, out var frequency))
                {
                    return BadRequest($"RecurringOperationFrequency '{request.Frequency}' non valida.");
                }

                var command = new UpdateRecurringTransactionCommand(
                    requestId,
                    request.Description,
                    request.Template.Description,
                    currency,
                    request.Template.Amount,
                    transactionType,
                    request.CategoryId,
                    frequency,
                    request.StartDate,
                    request.EndDate,
                    request.Template.Date
                );

                await _mediator.Send(command);
                return Ok(id);
            }
            catch (ValidationException vex)
            {
                return BadRequest(new { Errors = vex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecurringTransactionAsync(long id)
        {
            try
            {
                await _mediator.Send(new DeleteRecurringTransactionCommand(id));
                return Ok(id);
            }
            catch (ValidationException vex)
            {
                return BadRequest(new { Errors = vex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
