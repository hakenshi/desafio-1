using MediatR;

namespace HypeSoft.Application.Auth.Commands;

public record DeleteUserCommand(string UserId) : IRequest<bool>;
