using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Auth.Queries;

public record GetUsersQuery : IRequest<IEnumerable<KeycloakUserDto>>;
