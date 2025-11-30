using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Auth.Commands;

public record RegisterCommand(RegisterRequestDto Request) : IRequest<bool>;
