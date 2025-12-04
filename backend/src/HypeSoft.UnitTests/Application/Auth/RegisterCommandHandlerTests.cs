using FluentAssertions;
using HypeSoft.Application.Auth.Commands;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IKeycloakService> _keycloakServiceMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _keycloakServiceMock = new Mock<IKeycloakService>();
        _handler = new RegisterCommandHandler(_keycloakServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsTrue()
    {
        var request = new RegisterRequestDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            FirstName = "Test",
            LastName = "User"
        };

        _keycloakServiceMock
            .Setup(x => x.RegisterAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new RegisterCommand(request);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _keycloakServiceMock.Verify(x => x.RegisterAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RegistrationFails_ReturnsFalse()
    {
        var request = new RegisterRequestDto
        {
            Username = "existinguser",
            Email = "existing@example.com",
            Password = "password123"
        };

        _keycloakServiceMock
            .Setup(x => x.RegisterAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new RegisterCommand(request);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
