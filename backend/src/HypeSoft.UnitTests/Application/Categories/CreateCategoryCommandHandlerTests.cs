using AutoMapper;
using FluentAssertions;
using HypeSoft.Application.Categories.Commands;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Categories;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _auditServiceMock = new Mock<IAuditService>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _auditServiceMock.Object,
            _currentUserMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesCategory()
    {
        // Arrange
        var createDto = new CreateCategoryDto("Test Category", "Test Description");
        var command = new CreateCategoryCommand(createDto);
        var expectedDto = new CategoryDto("cat-1", "Test Category", "Test Description", DateTime.UtcNow, DateTime.UtcNow);

        _categoryRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken _) => c);

        _currentUserMock.Setup(x => x.UserId).Returns("user-1");
        _currentUserMock.Setup(x => x.Username).Returns("testuser");

        _mapperMock
            .Setup(x => x.Map<CategoryDto>(It.IsAny<Category>()))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Category");
        _categoryRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.LogAsync(
            "user-1", "testuser", "Create", "Category",
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NullCurrentUser_UsesSystemForAudit()
    {
        // Arrange
        var createDto = new CreateCategoryDto("Test Category", "Test Description");
        var command = new CreateCategoryCommand(createDto);
        var expectedDto = new CategoryDto("cat-1", "Test Category", "Test Description", DateTime.UtcNow, DateTime.UtcNow);

        _categoryRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken _) => c);

        _currentUserMock.Setup(x => x.UserId).Returns((string?)null);
        _currentUserMock.Setup(x => x.Username).Returns((string?)null);

        _mapperMock
            .Setup(x => x.Map<CategoryDto>(It.IsAny<Category>()))
            .Returns(expectedDto);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _auditServiceMock.Verify(x => x.LogAsync(
            "system", "system", "Create", "Category",
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
