using HypeSoft.Application.Categories.Commands;
using HypeSoft.Application.Categories.Queries;
using HypeSoft.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UpdateCategoryDto = HypeSoft.Application.Categories.Commands.UpdateCategoryDto;

namespace HypeSoft.API.Controllers;

/// <summary>
/// Controller for managing product categories
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all categories with pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<CategoryDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery(page, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(string id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery(id));
        if (category == null)
            return NotFound(new { message = $"Category with ID {id} not found" });
        
        return Ok(category);
    }

    /// <summary>
    /// Create a new category (requires admin or manager role)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "admin,manager")]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
    {
        var category = await _mediator.Send(new CreateCategoryCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    /// <summary>
    /// Update an existing category (requires admin or manager role)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin,manager")]
    public async Task<ActionResult<CategoryDto>> Update(string id, [FromBody] UpdateCategoryDto dto)
    {
        try
        {
            var category = await _mediator.Send(new UpdateCategoryCommand(id, dto));
            return Ok(category);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a category (requires admin or manager role)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,manager")]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            await _mediator.Send(new DeleteCategoryCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
