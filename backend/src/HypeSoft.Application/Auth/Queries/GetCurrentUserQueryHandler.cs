using HypeSoft.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace HypeSoft.Application.Auth.Queries;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserInfoDto>
{
    private readonly ILogger<GetCurrentUserQueryHandler> _logger;

    public GetCurrentUserQueryHandler(ILogger<GetCurrentUserQueryHandler> logger)
    {
        _logger = logger;
    }

    public Task<UserInfoDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var token = request.AccessToken;
        
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Access token is empty");
            return Task.FromResult(new UserInfoDto());
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            
            // Get the highest priority role (admin > manager > user)
            var role = "user";
            var realmAccessClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
            
            if (!string.IsNullOrEmpty(realmAccessClaim))
            {
                var realmAccess = JsonSerializer.Deserialize<JsonElement>(realmAccessClaim);
                if (realmAccess.TryGetProperty("roles", out var rolesElement))
                {
                    var roles = rolesElement.EnumerateArray()
                        .Select(r => r.GetString() ?? "")
                        .Where(r => !string.IsNullOrEmpty(r))
                        .ToList();

                    // Priority: admin > manager > user
                    if (roles.Contains("admin"))
                        role = "admin";
                    else if (roles.Contains("manager"))
                        role = "manager";
                    else if (roles.Contains("user"))
                        role = "user";
                }
            }

            var userInfo = new UserInfoDto
            {
                Id = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "",
                Username = jwtToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? "",
                Email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "",
                FirstName = jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value,
                LastName = jwtToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value,
                Role = role
            };

            return Task.FromResult(userInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decode JWT token");
            return Task.FromResult(new UserInfoDto());
        }
    }
}
