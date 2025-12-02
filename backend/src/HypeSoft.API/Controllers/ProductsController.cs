using HypeSoft.Application.DTOs;
using HypeSoft.Application.Products.Commands;
using HypeSoft.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HypeSoft.API.Controllers;

/// <summary>
/// Gerenciamento de produtos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista todos os produtos com paginação
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10, máximo: 100)</param>
    /// <param name="categoryId">ID da categoria para filtrar (opcional)</param>
    /// <returns>Lista paginada de produtos</returns>
    /// <response code="200">Retorna a lista de produtos</response>
    /// <response code="400">Parâmetros de paginação inválidos</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedResponse<ProductDto>>> GetAll(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] string? categoryId = null)
    {
        var result = await _mediator.Send(new GetAllProductsQuery(page, pageSize, categoryId));
        return Ok(result);
    }

    /// <summary>
    /// Busca um produto por ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>Dados do produto</returns>
    /// <response code="200">Retorna o produto encontrado</response>
    /// <response code="404">Produto não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(string id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        if (product == null)
            return NotFound();
        
        return Ok(product);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Search([FromQuery] string name)
    {
        var products = await _mediator.Send(new SearchProductsQuery(name));
        return Ok(products);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStock()
    {
        var products = await _mediator.Send(new GetLowStockProductsQuery());
        return Ok(products);
    }

    /// <summary>
    /// Cria um novo produto (requer role admin ou manager)
    /// </summary>
    /// <param name="dto">Dados do produto a ser criado</param>
    /// <returns>Produto criado</returns>
    /// <response code="201">Produto criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="403">Acesso negado - requer role admin ou manager</response>
    [HttpPost]
    [Authorize(Roles = "admin,manager")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        var product = await _mediator.Send(new CreateProductCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Atualiza um produto existente (requer role admin ou manager)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin,manager")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductDto>> Update(string id, [FromBody] UpdateProductDto dto)
    {
        var product = await _mediator.Send(new UpdateProductCommand(id, dto));
        return Ok(product);
    }

    /// <summary>
    /// Remove um produto (requer role admin ou manager)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,manager")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(string id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }
}
