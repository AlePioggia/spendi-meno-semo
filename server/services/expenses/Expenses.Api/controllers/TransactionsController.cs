using Expenses.Api.dto.request.transaction;
using Expenses.Api.dto.response.transaction;
using Expenses.Application.commands.transactions;
using Expenses.Application.queries.transactions;
using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Expenses.Api.controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetTransactionResponseDto>>> GetTransactionsAsync()
        {
            try
            {
                //long userId = long.Parse(User.FindFirst("sub")!.Value); 
                //long tenantId = long.Parse(User.FindFirst("tenantId")!.Value);
                var query = new GetTransactionsQuery();
                var transactions = await _mediator.Send(query);
                if (transactions is null)
                {
                    return NotFound();
                }

                var mappedTransactions = transactions.Select(
                    tx => new GetTransactionResponseDto(
                        tx.Id,
                        tx.Description ?? "",
                        tx.Amount != null ? tx.Amount.Amount : 0,
                        tx.Amount != null ? tx.Amount.Currency.ToString() : "",
                        tx.ExpenseType.ToString(),
                        tx.CategoryId,
                        tx.Date,
                        tx.CreatedAt
                    )).ToList();

                return Ok(mappedTransactions);
            } catch (ValidationException vex)
            {
                return BadRequest(new { Errors = vex.Errors.Select(e => e.ErrorMessage) });
            } catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetTransactionResponseDto>> GetTransactionAsync(int id)
        {
            try
            {
                //long userId = long.Parse(User.FindFirst("sub")!.Value);
                //long tenantId = long.Parse(User.FindFirst("tenantId")!.Value);
                var query = new GetTransactionByIdQuery(
                    id
                );

                var transaction = await _mediator.Send(query);
                if (transaction is null)
                {
                    return NotFound();
                }

                var mappedTransaction = new GetTransactionResponseDto(
                    transaction.Id,
                    transaction.Description ?? "",
                    transaction.Amount != null ? transaction.Amount.Amount : 0,
                    transaction.Amount != null ? transaction.Amount.Currency.ToString() : "",
                    transaction.ExpenseType.ToString(),
                    transaction.CategoryId,
                    transaction.Date,
                    transaction.CreatedAt
                );
                return Ok(mappedTransaction);
            } catch (ValidationException ex)
            {
                return BadRequest(new { Errors = ex.Errors.Select(e => e.ErrorMessage) });
            } catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransactionAsync([FromBody] CreateTransactionRequestDto request)
        {
            try
            {
                //long userId = long.Parse(User.FindFirst("sub")!.Value);
                //long tenantId = long.Parse(User.FindFirst("tenantId")!.Value);

                if (!Enum.TryParse<TransactionType>(request.TransactionType, true, out var expenseType))
                {
                    return BadRequest($"ExpenseType '{request.TransactionType}' non valido.");
                }

                if (!Enum.TryParse<Currency>(request.Currency, true, out var currency))
                {
                    return BadRequest($"Currency '{request.Currency}' non valida.");
                }

                var command = new CreateTransactionCommand(
                    request.Description,
                    request.Amount,
                    currency,
                    expenseType,
                    request.CategoryId,
                    request.Date
                );
                
                await _mediator.Send(command);

                return Ok();
            } catch (ValidationException ex)
            {
                return BadRequest(new { Errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTransactionAsync(int id, [FromBody] UpdateTransactionRequestDto request)
        {
            try
            {
                //long userId = long.Parse(User.FindFirst("sub")!.Value);
                //long tenantId = long.Parse(User.FindFirst("tenantId")!.Value);

                if (!Enum.TryParse<TransactionType>(request.TransactionType, true, out var transactionType))
                {
                    return BadRequest($"TransactionType '{request.TransactionType}' non valido.");
                }

                if (!Enum.TryParse<Currency>(request.Currency, true, out var currency))
                {
                    return BadRequest($"Currency '{request.Currency}' non valida.");
                }

                var command = new UpdateTransactionCommand(
                    id,
                    request.Description,
                    request.Amount,
                    transactionType,
                    currency,
                    request.CategoryId,
                    request.Date
                );

                await _mediator.Send(command);
                return Ok(id);
            } catch (ValidationException ex)
            {
                return BadRequest(new { Errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransactionAsync(int id)
        {
            try
            {
                //long userId = long.Parse(User.FindFirst("sub")!.Value);
                //long tenantId = long.Parse(User.FindFirst("tenantId")!.Value);

                await _mediator.Send(new DeleteTransactionCommand(id));
                return Ok(id);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { Errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
