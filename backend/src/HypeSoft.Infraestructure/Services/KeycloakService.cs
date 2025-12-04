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

    public async Task<bool> UpdateUserAsync(string userId, UpdateUserRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var adminToken = await GetAdminTokenAsync(cancellationToken);
            if (string.IsNullOrEmpty(adminToken))
            {
                _logger.LogError("Unable to get admin token for user update");
                return false;
            }

            var keycloakUrl = _configuration["Keycloak:Authority"]?.Replace("/realms/hypesoft", "") ?? "";
            var userEndpoint = $"{keycloakUrl}/admin/realms/hypesoft/users/{userId}";

            var userPayload = new
            {
                email = request.Email,
                firstName = request.FirstName,
                lastName = request.LastName
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Put, userEndpoint)
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
                _logger.LogError("Failed to update user: {Error}", error);
                return false;
            }

            await UpdateUserRoleAsync(userId, request.Role, adminToken, cancellationToken);

            _logger.LogInformation("User {UserId} updated successfully", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during update for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adminToken = await GetAdminTokenAsync(cancellationToken);
            if (string.IsNullOrEmpty(adminToken))
            {
                _logger.LogError("Unable to get admin token for user deletion");
                return false;
            }

            var keycloakUrl = _configuration["Keycloak:Authority"]?.Replace("/realms/hypesoft", "") ?? "";
            var userEndpoint = $"{keycloakUrl}/admin/realms/hypesoft/users/{userId}";

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, userEndpoint);
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to delete user: {Error}", error);
                return false;
            }

            _logger.LogInformation("User {UserId} deleted successfully", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during deletion for user {UserId}", userId);
            return false;
        }
    }

    private async Task UpdateUserRoleAsync(string userId, string newRole, string adminToken, CancellationToken cancellationToken)
    {
        try
        {
            var keycloakUrl = _configuration["Keycloak:Authority"]?.Replace("/realms/hypesoft", "") ?? "";
            
            var rolesEndpoint = $"{keycloakUrl}/admin/realms/hypesoft/roles";
            var rolesRequest = new HttpRequestMessage(HttpMethod.Get, rolesEndpoint);
            rolesRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
            
            var rolesResponse = await _httpClient.SendAsync(rolesRequest, cancellationToken);
            if (!rolesResponse.IsSuccessStatusCode) return;

            var rolesJson = await rolesResponse.Content.ReadAsStringAsync(cancellationToken);
            var availableRoles = JsonSerializer.Deserialize<List<KeycloakRoleResponse>>(rolesJson) ?? new List<KeycloakRoleResponse>();

            var userRolesEndpoint = $"{keycloakUrl}/admin/realms/hypesoft/users/{userId}/role-mappings/realm";
            var currentRolesRequest = new HttpRequestMessage(HttpMethod.Get, userRolesEndpoint);
            currentRolesRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
            
            var currentRolesResponse = await _httpClient.SendAsync(currentRolesRequest, cancellationToken);
            var currentRolesJson = await currentRolesResponse.Content.ReadAsStringAsync(cancellationToken);
            var currentRoles = JsonSerializer.Deserialize<List<KeycloakRoleWithIdResponse>>(currentRolesJson) ?? new List<KeycloakRoleWithIdResponse>();

            var rolesToRemove = currentRoles.Where(r => r.Name == "admin" || r.Name == "manager" || r.Name == "user").ToList();
            if (rolesToRemove.Any())
            {
                var removeRequest = new HttpRequestMessage(HttpMethod.Delete, userRolesEndpoint)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(rolesToRemove.Select(r => new { id = r.Id, name = r.Name })),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    )
                };
                removeRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
                await _httpClient.SendAsync(removeRequest, cancellationToken);
            }

            var roleToAdd = availableRoles.FirstOrDefault(r => r.Name == newRole);
            if (roleToAdd != null)
            {
                var addRequest = new HttpRequestMessage(HttpMethod.Post, userRolesEndpoint)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new[] { new { id = roleToAdd.Id, name = roleToAdd.Name } }),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    )
                };
                addRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
                await _httpClient.SendAsync(addRequest, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role for user {UserId}", userId);
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

    public async Task<IEnumerable<KeycloakUserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adminToken = await GetAdminTokenAsync(cancellationToken);
            if (string.IsNullOrEmpty(adminToken))
            {
                _logger.LogError("Unable to get admin token for listing users");
                return Enumerable.Empty<KeycloakUserDto>();
            }

            var keycloakUrl = _configuration["Keycloak:Authority"]?.Replace("/realms/hypesoft", "") ?? "";
            var usersEndpoint = $"{keycloakUrl}/admin/realms/hypesoft/users";

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, usersEndpoint);
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to get users: {Error}", error);
                return Enumerable.Empty<KeycloakUserDto>();
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var users = JsonSerializer.Deserialize<List<KeycloakUserResponse>>(json) ?? new List<KeycloakUserResponse>();

            var result = new List<KeycloakUserDto>();
            foreach (var user in users)
            {
                var role = await GetUserRoleAsync(user.Id ?? "", adminToken, cancellationToken);
                result.Add(new KeycloakUserDto
                {
                    Id = user.Id ?? "",
                    Username = user.Username ?? "",
                    Email = user.Email ?? "",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Enabled = user.Enabled,
                    Role = role
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users from Keycloak");
            return Enumerable.Empty<KeycloakUserDto>();
        }
    }

    private async Task<string> GetUserRoleAsync(string userId, string adminToken, CancellationToken cancellationToken)
    {
        try
        {
            var keycloakUrl = _configuration["Keycloak:Authority"]?.Replace("/realms/hypesoft", "") ?? "";
            var rolesEndpoint = $"{keycloakUrl}/admin/realms/hypesoft/users/{userId}/role-mappings/realm";

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, rolesEndpoint);
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return "user";
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var roles = JsonSerializer.Deserialize<List<KeycloakRoleResponse>>(json) ?? new List<KeycloakRoleResponse>();

            if (roles.Any(r => r.Name == "admin"))
                return "admin";
            if (roles.Any(r => r.Name == "manager"))
                return "manager";
            
            return "user";
        }
        catch
        {
            return "user";
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

internal class KeycloakUserResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }
    
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
    
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
}

internal class KeycloakRoleResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

internal class KeycloakRoleWithIdResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
