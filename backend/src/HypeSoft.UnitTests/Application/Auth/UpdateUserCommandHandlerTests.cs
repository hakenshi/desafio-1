using FluentAssertions;
using HypeSoft.Application.Auth.Commands;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Auth;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IKeycloakService> _keycloakServiceMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _keycloakServiceMock = new Mock<IKeycloakService>();
        _auditServiceMock = new Mock<IAuditService>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _handler = new UpdateUserCommandHandler(
            _keycloakServiceMock.Object,
            _auditServiceMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesUserAndLogsAudit()
    {
        var userId = "user-1";
        var updateRequest = new UpdateUserRequestDto
        {
            Email = "updated@example.com",
            FirstName = "Updated",
            LastName = "User",
            Role = "manager"
        };

        _keycloakServiceMock
            .Setup(x => x.UpdateUserAsync(userId, updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _currentUserMock.Setup(x => x.UserId).Returns("admin-1");
        _currentUserMock.Setup(x => x.Username).Returns("admin");

        var command = new UpdateUserCommand(userId, updateRequest);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _keycloakServiceMock.Verify(x => x.UpdateUserAsync(userId, updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.LogAsync(
            "admin-1", "admin", "Update", "User",
            userId, updateRequest.Email, It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateFails_DoesNotLogAudit()
    {
        var userId = "user-1";
        var updateRequest = new UpdateUserRequestDto
        {
            Email = "updated@example.com",
            Role = "manager"
        };

        _keycloakServiceMock
            .Setup(x => x.UpdateUserAsync(userId, updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new UpdateUserCommand(userId, updateRequest);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _auditServiceMock.Verify(x => x.LogAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NullCurrentUser_UsesSystemForAudit()
    {
        var userId = "user-1";
        var updateRequest = new UpdateUserRequestDto
        {
            Email = "updated@example.com",
            Role = "user"
        };

        _keycloakServiceMock
            .Setup(x => x.UpdateUserAsync(userId, updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _currentUserMock.Setup(x => x.UserId).Returns((string?)null);
        _currentUserMock.Setup(x => x.Username).Returns((string?)null);

        var command = new UpdateUserCommand(userId, updateRequest);

        await _handler.Handle(command, CancellationToken.None);

        _auditServiceMock.Verify(x => x.LogAsync(
            "system", "system", "Update", "User",
            userId, updateRequest.Email, It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
