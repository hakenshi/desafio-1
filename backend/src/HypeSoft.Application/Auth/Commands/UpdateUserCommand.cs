using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Auth.Commands;

public record UpdateUserCommand(string UserId, UpdateUserRequestDto User) : IRequest<bool>;
