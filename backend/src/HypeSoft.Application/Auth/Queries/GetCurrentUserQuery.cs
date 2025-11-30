using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Auth.Queries;

public record GetCurrentUserQuery(string AccessToken) : IRequest<UserInfoDto>;
