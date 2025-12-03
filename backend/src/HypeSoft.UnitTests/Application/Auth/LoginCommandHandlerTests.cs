using FluentAssertions;
using HypeSoft.Application.Auth.Commands;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IKeycloakService> _keycloakServiceMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _keycloakServiceMock = new Mock<IKeycloakService>();
        _handler = new LoginCommandHandler(_keycloakServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsTokenResponse()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var expectedToken = new TokenResponseDto
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        _keycloakServiceMock
            .Setup(x => x.LoginAsync(email, password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedToken);

        var command = new LoginCommand(email, password);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("access-token");
        _keycloakServiceMock.Verify(x => x.LoginAsync(email, password, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ReturnsNull()
    {
        // Arrange
        var email = "invalid@example.com";
        var password = "wrongpassword";

        _keycloakServiceMock
            .Setup(x => x.LoginAsync(email, password, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TokenResponseDto?)null);

        var command = new LoginCommand(email, password);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
