using HypeSoft.Application.Auth.Commands;
using HypeSoft.Application.Auth.Queries;
using HypeSoft.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HypeSoft.API.Controllers;

/// <summary>
/// Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all users (requires admin role)
    /// </summary>
    [HttpGet("users")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(IEnumerable<KeycloakUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<KeycloakUserDto>>> GetUsers()
    {
        var users = await _mediator.Send(new GetUsersQuery());
        return Ok(users);
    }

    /// <summary>
    /// Update a user (requires admin role)
    /// </summary>
    [HttpPut("users/{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequestDto request)
    {
        var success = await _mediator.Send(new UpdateUserCommand(id, request));
        
        if (!success)
        {
            return BadRequest(new { message = "Failed to update user" });
        }

        return Ok(new { message = "User updated successfully" });
    }

    /// <summary>
    /// Delete a user (requires admin role)
    /// </summary>
    [HttpDelete("users/{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var success = await _mediator.Send(new DeleteUserCommand(id));
        
        if (!success)
        {
            return BadRequest(new { message = "Failed to delete user" });
        }

        return Ok(new { message = "User deleted successfully" });
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password));
        
        if (result is null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var success = await _mediator.Send(new RegisterCommand(request));
        
        if (!success)
        {
            return BadRequest(new { message = "Failed to create user" });
        }

        return Created("", new { message = "User registered successfully" });
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken));
        
        if (result is null)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get current user info
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserInfoDto>> GetCurrentUser()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        var token = authHeader.StartsWith("Bearer ") ? authHeader[7..] : authHeader;
        
        var result = await _mediator.Send(new GetCurrentUserQuery(token));
        return Ok(result);
    }

    /// <summary>
    /// Logout and invalidate refresh token
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
    {
        var success = await _mediator.Send(new LogoutCommand(request.RefreshToken));
        
        if (!success)
        {
            return BadRequest(new { message = "Failed to logout" });
        }

        return Ok(new { message = "Logged out successfully" });
    }
}
