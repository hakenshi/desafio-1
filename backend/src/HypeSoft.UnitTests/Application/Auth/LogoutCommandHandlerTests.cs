using FluentAssertions;
using HypeSoft.Application.Auth.Commands;
using HypeSoft.Application.Interfaces;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Auth;

public class LogoutCommandHandlerTests
{
    private readonly Mock<IKeycloakService> _keycloakServiceMock;
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _keycloakServiceMock = new Mock<IKeycloakService>();
        _handler = new LogoutCommandHandler(_keycloakServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRefreshToken_ReturnsTrue()
    {
        var refreshToken = "valid-refresh-token";

        _keycloakServiceMock
            .Setup(x => x.LogoutAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new LogoutCommand(refreshToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _keycloakServiceMock.Verify(x => x.LogoutAsync(refreshToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRefreshToken_ReturnsFalse()
    {
        var refreshToken = "invalid-refresh-token";

        _keycloakServiceMock
            .Setup(x => x.LogoutAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new LogoutCommand(refreshToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
