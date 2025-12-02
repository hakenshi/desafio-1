using FluentAssertions;
using HypeSoft.Application.Auth.Commands;
using HypeSoft.Application.Interfaces;
using Moq;

namespace HypeSoft.UnitTests.Application.Auth;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IKeycloakService> _keycloakServiceMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _keycloakServiceMock = new Mock<IKeycloakService>();
        _auditServiceMock = new Mock<IAuditService>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        
        _currentUserServiceMock.Setup(x => x.UserId).Returns("admin-id");
        _currentUserServiceMock.Setup(x => x.Username).Returns("admin");
        
        _handler = new DeleteUserCommandHandler(
            _keycloakServiceMock.Object,
            _auditServiceMock.Object,
            _currentUserServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenDeleteSucceeds_ShouldReturnTrue()
    {
        // Arrange
        var userId = "user-to-delete";
        _keycloakServiceMock
            .Setup(x => x.DeleteUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new DeleteUserCommand(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenDeleteFails_ShouldReturnFalse()
    {
        // Arrange
        var userId = "user-to-delete";
        _keycloakServiceMock
            .Setup(x => x.DeleteUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new DeleteUserCommand(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenDeleteSucceeds_ShouldLogAudit()
    {
        // Arrange
        var userId = "user-to-delete";
        _keycloakServiceMock
            .Setup(x => x.DeleteUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new DeleteUserCommand(userId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _auditServiceMock.Verify(
            x => x.LogAsync(
                "admin-id",
                "admin",
                "Delete",
                "User",
                userId,
                null,
                "User deleted from Keycloak",
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenDeleteFails_ShouldNotLogAudit()
    {
        // Arrange
        var userId = "user-to-delete";
        _keycloakServiceMock
            .Setup(x => x.DeleteUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new DeleteUserCommand(userId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _auditServiceMock.Verify(
            x => x.LogAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_ShouldCallKeycloakServiceWithCorrectUserId()
    {
        // Arrange
        var userId = "specific-user-id";
        _keycloakServiceMock
            .Setup(x => x.DeleteUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new DeleteUserCommand(userId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _keycloakServiceMock.Verify(
            x => x.DeleteUserAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenCurrentUserIsNull_ShouldUseSystemForAudit()
    {
        // Arrange
        var userId = "user-to-delete";
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        _currentUserServiceMock.Setup(x => x.Username).Returns((string?)null);
        
        _keycloakServiceMock
            .Setup(x => x.DeleteUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new DeleteUserCommand(userId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _auditServiceMock.Verify(
            x => x.LogAsync(
                "system",
                "system",
                "Delete",
                "User",
                userId,
                null,
                "User deleted from Keycloak",
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}
