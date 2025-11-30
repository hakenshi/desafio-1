using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using MediatR;

namespace HypeSoft.Application.Auth.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponseDto?>
{
    private readonly IKeycloakService _keycloakService;

    public RefreshTokenCommandHandler(IKeycloakService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    public async Task<TokenResponseDto?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _keycloakService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
    }
}
