using HypeSoft.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
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
        var user = request.User;
        
        var roles = new List<string>();
        var realmAccessClaim = user.FindFirst("realm_access")?.Value;
        
        if (!string.IsNullOrEmpty(realmAccessClaim))
        {
            try
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
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse realm_access claim");
            }
        }
        
        var userInfo = new UserInfoDto
        {
            Id = user.FindFirst("sub")?.Value ?? "",
            Username = user.FindFirst("preferred_username")?.Value ?? "",
            Email = user.FindFirst("email")?.Value ?? "",
            FirstName = user.FindFirst("given_name")?.Value,
            LastName = user.FindFirst("family_name")?.Value,
            Roles = roles
        };

        return Task.FromResult(userInfo);
    }
}
