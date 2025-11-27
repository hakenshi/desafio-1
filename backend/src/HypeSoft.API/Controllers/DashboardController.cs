using HypeSoft.Application.Dashboard.Queries;
using HypeSoft.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HypeSoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
}
