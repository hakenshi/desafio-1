using HypeSoft.Application.Interfaces;
using MediatR;

namespace HypeSoft.Application.Auth.Commands;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IKeycloakService _keycloakService;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserCommandHandler(
        IKeycloakService keycloakService,
        IAuditService auditService,
        ICurrentUserService currentUser)
    {
        _keycloakService = keycloakService;
        _auditService = auditService;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _keycloakService.UpdateUserAsync(request.UserId, request.User, cancellationToken);

        if (result)
        {
            await _auditService.LogAsync(
                _currentUser.UserId ?? "system",
                _currentUser.Username ?? "system",
                "Update",
                "User",
                request.UserId,
                request.User.Email,
                $"Updated user role to {request.User.Role}",
                cancellationToken
            );
        }

        return result;
    }
}
