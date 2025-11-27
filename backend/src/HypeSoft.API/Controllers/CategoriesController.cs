using HypeSoft.Application.Categories.Commands;
using HypeSoft.Application.Categories.Queries;
using HypeSoft.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HypeSoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
    {
        var category = await _mediator.Send(new CreateCategoryCommand(dto));
        return CreatedAtAction(nameof(GetAll), new { id = category.Id }, category);
    }
}
