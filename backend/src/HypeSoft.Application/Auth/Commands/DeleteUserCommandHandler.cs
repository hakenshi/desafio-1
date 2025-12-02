using HypeSoft.Application.Interfaces;
using MediatR;

namespace HypeSoft.Application.Auth.Commands;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IKeycloakService _keycloakService;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUser;

    public DeleteUserCommandHandler(
        IKeycloakService keycloakService,
        IAuditService auditService,
        ICurrentUserService currentUser)
    {
        _keycloakService = keycloakService;
        _auditService = auditService;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _keycloakService.DeleteUserAsync(request.UserId, cancellationToken);

        if (result)
        {
            await _auditService.LogAsync(
                _currentUser.UserId ?? "system",
                _currentUser.Username ?? "system",
                "Delete",
                "User",
                request.UserId,
                null,
                "User deleted from Keycloak",
                cancellationToken
            );
        }

        return result;
    }
}
