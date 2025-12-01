using HypeSoft.Application.Dashboard.Queries;
using HypeSoft.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HypeSoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get()
    {
        var dashboard = await _mediator.Send(new GetDashboardQuery());
        return Ok(dashboard);
    }

    [HttpGet("audit-logs")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogs([FromQuery] int count = 10)
    {
        var logs = await _mediator.Send(new GetRecentAuditLogsQuery(count));
        return Ok(logs);
    }

    [HttpGet("recent-products")]
    public async Task<ActionResult<IEnumerable<RecentProductDto>>> GetRecentProducts([FromQuery] int count = 10)
    {
        var products = await _mediator.Send(new GetRecentProductsQuery(count));
        return Ok(products);
    }
}
