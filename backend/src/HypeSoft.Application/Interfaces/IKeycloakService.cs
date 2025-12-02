using HypeSoft.Application.DTOs;

namespace HypeSoft.Application.Interfaces;

public interface IKeycloakService
{
    Task<TokenResponseDto?> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<bool> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserAsync(string userId, UpdateUserRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<IEnumerable<KeycloakUserDto>> GetUsersAsync(CancellationToken cancellationToken = default);
}
