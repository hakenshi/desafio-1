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
            
            var roles = new List<string>();
            var realmAccessClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
            
            if (!string.IsNullOrEmpty(realmAccessClaim))
            {
                var realmAccess = JsonSerializer.Deserialize<JsonElement>(realmAccessClaim);
                if (realmAccess.TryGetProperty("roles", out var rolesElement))
                {
                    roles = rolesElement.EnumerateArray()
                        .Select(r => r.GetString() ?? "")
                        .Where(r => !string.IsNullOrEmpty(r))
                        .ToList();
                }
            }

            var userInfo = new UserInfoDto
            {
                Id = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "",
                Username = jwtToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? "",
                Email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "",
                FirstName = jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value,
                LastName = jwtToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value,
                Roles = roles
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
