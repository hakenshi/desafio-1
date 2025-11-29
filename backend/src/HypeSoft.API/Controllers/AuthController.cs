using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HypeSoft.API.Controllers;

/// <summary>
/// Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var tokenEndpoint = $"{_configuration["Keycloak:Authority"]}/protocol/openid-connect/token";
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = _configuration["Keycloak:ClientId"] ?? "hypesoft-api",
                ["client_secret"] = _configuration["Keycloak:ClientSecret"] ?? "hypesoft-api-secret",
                ["username"] = request.Email,
                ["password"] = request.Password
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Login failed for user {Email}", request.Email);
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(json);

            return Ok(new TokenResponse
            {
                AccessToken = tokenResponse?.AccessToken ?? "",
                RefreshToken = tokenResponse?.RefreshToken ?? "",
                ExpiresIn = tokenResponse?.ExpiresIn ?? 0,
                TokenType = tokenResponse?.TokenType ?? "Bearer"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Email}", request.Email);
            return StatusCode(500, new { message = "Authentication service unavailable" });
        }
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Get admin token
            var adminToken = await GetAdminTokenAsync();
            if (string.IsNullOrEmpty(adminToken))
            {
                return StatusCode(500, new { message = "Unable to connect to authentication service" });
            }

            // Create user in Keycloak
            var keycloakUrl = _configuration["Keycloak:Authority"]?.Replace("/realms/hypesoft", "") ?? "";
            var usersEndpoint = $"{keycloakUrl}/admin/realms/hypesoft/users";

            var userPayload = new
            {
                username = request.Username,
                email = request.Email,
                firstName = request.FirstName,
                lastName = request.LastName,
                enabled = true,
                emailVerified = true,
                credentials = new[]
                {
                    new
                    {
                        type = "password",
                        value = request.Password,
                        temporary = false
                    }
                },
                realmRoles = new[] { "user" }
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, usersEndpoint)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(userPayload),
                    System.Text.Encoding.UTF8,
                    "application/json"
                )
            };
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _httpClient.SendAsync(httpRequest);

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return BadRequest(new { message = "Email already exists" });
            }

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to create user: {Error}", error);
                return BadRequest(new { message = "Failed to create user" });
            }

            _logger.LogInformation("User {Email} registered successfully", request.Email);
            return Created("", new { message = "User registered successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user {Email}", request.Email);
            return StatusCode(500, new { message = "Registration service unavailable" });
        }
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var tokenEndpoint = $"{_configuration["Keycloak:Authority"]}/protocol/openid-connect/token";
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = _configuration["Keycloak:ClientId"] ?? "hypesoft-api",
                ["client_secret"] = _configuration["Keycloak:ClientSecret"] ?? "hypesoft-api-secret",
                ["refresh_token"] = request.RefreshToken
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            
            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(json);

            return Ok(new TokenResponse
            {
                AccessToken = tokenResponse?.AccessToken ?? "",
                RefreshToken = tokenResponse?.RefreshToken ?? "",
                ExpiresIn = tokenResponse?.ExpiresIn ?? 0,
                TokenType = tokenResponse?.TokenType ?? "Bearer"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new { message = "Authentication service unavailable" });
        }
    }

    /// <summary>
    /// Get current user info
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<UserInfo> GetCurrentUser()
    {
        var user = HttpContext.User;
        
        return Ok(new UserInfo
        {
            Id = user.FindFirst("sub")?.Value ?? "",
            Username = user.FindFirst("preferred_username")?.Value ?? "",
            Email = user.FindFirst("email")?.Value ?? "",
            FirstName = user.FindFirst("given_name")?.Value ?? "",
            LastName = user.FindFirst("family_name")?.Value ?? "",
            Roles = user.FindAll("realm_access")?.Select(c => c.Value).ToList() ?? new List<string>()
        });
    }

    private async Task<string?> GetAdminTokenAsync()
    {
        try
        {
            var keycloakUrl = _configuration["Keycloak:Authority"]?.Replace("/realms/hypesoft", "") ?? "";
            var tokenEndpoint = $"{keycloakUrl}/realms/master/protocol/openid-connect/token";
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = "admin-cli",
                ["username"] = "admin",
                ["password"] = "admin123"
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(json);
            return tokenResponse?.AccessToken;
        }
        catch
        {
            return null;
        }
    }
}

// DTOs
public record LoginRequest
{
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
}

public record RegisterRequest
{
    public string Username { get; init; } = "";
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

public record RefreshTokenRequest
{
    public string RefreshToken { get; init; } = "";
}

public record TokenResponse
{
    public string AccessToken { get; init; } = "";
    public string RefreshToken { get; init; } = "";
    public int ExpiresIn { get; init; }
    public string TokenType { get; init; } = "Bearer";
}

public record UserInfo
{
    public string Id { get; init; } = "";
    public string Username { get; init; } = "";
    public string Email { get; init; } = "";
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public List<string> Roles { get; init; } = new();
}

internal class KeycloakTokenResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("token_type")]
    public string? TokenType { get; set; }
}
