using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<TokenResponseDto?>;
