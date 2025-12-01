using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using MediatR;

namespace HypeSoft.Application.Auth.Queries;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<KeycloakUserDto>>
{
    private readonly IKeycloakService _keycloakService;

    public GetUsersQueryHandler(IKeycloakService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    public async Task<IEnumerable<KeycloakUserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _keycloakService.GetUsersAsync(cancellationToken);
    }
}
