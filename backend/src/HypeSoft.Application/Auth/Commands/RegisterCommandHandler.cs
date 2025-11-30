using HypeSoft.Application.Interfaces;
using MediatR;

namespace HypeSoft.Application.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
{
    private readonly IKeycloakService _keycloakService;

    public RegisterCommandHandler(IKeycloakService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _keycloakService.RegisterAsync(request.Request, cancellationToken);
    }
}
