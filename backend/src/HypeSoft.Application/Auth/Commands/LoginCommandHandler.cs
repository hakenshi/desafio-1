using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using MediatR;

namespace HypeSoft.Application.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponseDto?>
{
    private readonly IKeycloakService _keycloakService;

    public LoginCommandHandler(IKeycloakService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    public async Task<TokenResponseDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _keycloakService.LoginAsync(request.Email, request.Password, cancellationToken);
    }
}
