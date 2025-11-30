using HypeSoft.Application.Interfaces;
using MediatR;

namespace HypeSoft.Application.Auth.Commands;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IKeycloakService _keycloakService;

    public LogoutCommandHandler(IKeycloakService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        return await _keycloakService.LogoutAsync(request.RefreshToken, cancellationToken);
    }
}
