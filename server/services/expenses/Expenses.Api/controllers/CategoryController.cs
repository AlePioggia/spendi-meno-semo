using Expenses.Api.dto.request.category;
using Expenses.Api.dto.request.transaction;
using Expenses.Api.dto.response.transaction;
using Expenses.Application.commands.categories;
using Expenses.Application.queries.categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Expenses.Api.controllers
{
    [Authorize]
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetCategoryResponseDto>> GetCategoryByIdAsync(long id)
        {
            try
            {
                var query = new GetCategoryByIdQuery(id);
                var category = await _mediator.Send(query);
                if (category is null)
                {
                    return NotFound();
                }
                var responseDto = new GetCategoryResponseDto(
                    category.Id,
                    category.Name ?? "",
                    category.Description ?? "",
                    category.UserId,
                    category.TenantId,
                    category.CreatedAt
                );
                return Ok(responseDto);
            }
            catch (ValidationException vex)
            {
                return BadRequest("Validation errore" + vex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<GetCategoryResponseDto>>> GetCategoriesAsync()
        {
            try
            {
                var query = new GetCategoriesQuery();
                var categories = await _mediator.Send(query);
                if (categories is null)
                {
                    return NotFound();
                }
                var responseDtos = categories.Select(category => new GetCategoryResponseDto(
                    category.Id,
                    category.Name ?? "",
                    category.Description ?? "",
                    category.UserId ?? "",
                    category.TenantId,
                    category.CreatedAt
                )).ToList();
                return Ok(responseDtos);
            }
            catch (ValidationException vex)
            {
                return BadRequest("Validation error" + vex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequestDto request)
        {
            try
            {
                var command = new CreateCategoryCommand(
                    request.Name,
                    request.Description,
                    DateTime.Now
                );

                await _mediator.Send(command);

                return Ok(command);
            }
            catch (ValidationException vex)
            {
                return BadRequest("Validation error" + vex);
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
                await _mediator.Send(new DeleteCategoryCommand(id));
                return Ok(id);
            }
            catch (ValidationException ex)
            {
                return BadRequest("Validation error" + ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}
