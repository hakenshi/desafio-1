using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HypeSoft.Infraestructure.Services;

public class KeycloakService : IKeycloakService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KeycloakService> _logger;

    public KeycloakService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<KeycloakService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<TokenResponseDto?> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenEndpoint = $"{_configuration["Keycloak:Authority"]}/protocol/openid-connect/token";
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = _configuration["Keycloak:ClientId"] ?? "hypesoft-api",
                ["client_secret"] = _configuration["Keycloak:ClientSecret"] ?? "",
                ["username"] = email,
                ["password"] = password
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Login failed for user {Email}", email);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(json);

            return new TokenResponseDto
            {
                AccessToken = tokenResponse?.AccessToken ?? "",
                RefreshToken = tokenResponse?.RefreshToken ?? "",
                ExpiresIn = tokenResponse?.ExpiresIn ?? 0,
                TokenType = tokenResponse?.TokenType ?? "Bearer"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Email}", email);
            return null;
        }
    }

    public async Task<bool> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var adminToken = await GetAdminTokenAsync(cancellationToken);
            if (string.IsNullOrEmpty(adminToken))
            {
                _logger.LogError("Unable to get admin token for user registration");
                return false;
            }

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

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to create user: {Error}", error);
                return false;
            }

            _logger.LogInformation("User {Email} registered successfully", request.Email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user {Email}", request.Email);
            return false;
        }
    }

    public async Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenEndpoint = $"{_configuration["Keycloak:Authority"]}/protocol/openid-connect/token";
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = _configuration["Keycloak:ClientId"] ?? "hypesoft-api",
                ["client_secret"] = _configuration["Keycloak:ClientSecret"] ?? "",
                ["refresh_token"] = refreshToken
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Token refresh failed");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(json);

            return new TokenResponseDto
            {
                AccessToken = tokenResponse?.AccessToken ?? "",
                RefreshToken = tokenResponse?.RefreshToken ?? "",
                ExpiresIn = tokenResponse?.ExpiresIn ?? 0,
                TokenType = tokenResponse?.TokenType ?? "Bearer"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return null;
        }
    }

    public async Task<bool> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var logoutEndpoint = $"{_configuration["Keycloak:Authority"]}/protocol/openid-connect/logout";
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _configuration["Keycloak:ClientId"] ?? "hypesoft-api",
                ["client_secret"] = _configuration["Keycloak:ClientSecret"] ?? "",
                ["refresh_token"] = refreshToken
            });

            var response = await _httpClient.PostAsync(logoutEndpoint, content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Logout failed");
                return false;
            }

            _logger.LogInformation("User logged out successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return false;
        }
    }

    private async Task<string?> GetAdminTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var keycloakUrl = _configuration["Keycloak:Authority"]?.Replace("/realms/hypesoft", "") ?? "";
            var tokenEndpoint = $"{keycloakUrl}/realms/master/protocol/openid-connect/token";
            
            var adminUsername = _configuration["Keycloak:AdminUsername"] ?? "admin";
            var adminPassword = _configuration["Keycloak:AdminPassword"] ?? "";
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = "admin-cli",
                ["username"] = adminUsername,
                ["password"] = adminPassword
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(json);
            return tokenResponse?.AccessToken;
        }
        catch
        {
            return null;
        }
    }
}

internal class KeycloakTokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }
}
