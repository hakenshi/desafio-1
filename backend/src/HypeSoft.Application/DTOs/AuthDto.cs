namespace HypeSoft.Application.DTOs;

public record LoginRequestDto
{
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
}

public record RegisterRequestDto
{
    public string Username { get; init; } = "";
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

public record RefreshTokenRequestDto
{
    public string RefreshToken { get; init; } = "";
}

public record TokenResponseDto
{
    public string AccessToken { get; init; } = "";
    public string RefreshToken { get; init; } = "";
    public int ExpiresIn { get; init; }
    public string TokenType { get; init; } = "Bearer";
}

public record UserInfoDto
{
    public string Id { get; init; } = "";
    public string Username { get; init; } = "";
    public string Email { get; init; } = "";
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string Role { get; init; } = "user";
}

public record KeycloakUserDto
{
    public string Id { get; init; } = "";
    public string Username { get; init; } = "";
    public string Email { get; init; } = "";
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public bool Enabled { get; init; }
    public string Role { get; init; } = "user";
}
