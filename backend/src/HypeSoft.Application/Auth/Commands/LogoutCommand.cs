using MediatR;

namespace HypeSoft.Application.Auth.Commands;

public record LogoutCommand(string RefreshToken) : IRequest<bool>;
