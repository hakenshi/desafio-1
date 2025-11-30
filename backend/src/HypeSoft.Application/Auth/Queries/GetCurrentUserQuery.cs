using HypeSoft.Application.DTOs;
using MediatR;
using System.Security.Claims;

namespace HypeSoft.Application.Auth.Queries;

public record GetCurrentUserQuery(ClaimsPrincipal User) : IRequest<UserInfoDto>;
