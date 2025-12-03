using FluentAssertions;
using HypeSoft.Application.Auth.Commands;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Auth;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IKeycloakService> _keycloakServiceMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _keycloakServiceMock = new Mock<IKeycloakService>();
        _handler = new RefreshTokenCommandHandler(_keycloakServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRefreshToken_ReturnsNewTokenResponse()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";
        var expectedToken = new TokenResponseDto
        {
            AccessToken = "new-access-token",
            RefreshToken = "new-refresh-token",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        _keycloakServiceMock
            .Setup(x => x.RefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedToken);

        var command = new RefreshTokenCommand(refreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("new-access-token");
        _keycloakServiceMock.Verify(x => x.RefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRefreshToken_ReturnsNull()
    {
        // Arrange
        var refreshToken = "invalid-refresh-token";

        _keycloakServiceMock
            .Setup(x => x.RefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TokenResponseDto?)null);

        var command = new RefreshTokenCommand(refreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
