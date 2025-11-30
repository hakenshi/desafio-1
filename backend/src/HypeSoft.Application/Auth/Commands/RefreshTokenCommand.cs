using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<TokenResponseDto?>;
