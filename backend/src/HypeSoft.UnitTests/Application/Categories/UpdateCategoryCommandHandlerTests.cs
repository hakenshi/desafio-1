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

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _auditServiceMock = new Mock<IAuditService>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _auditServiceMock.Object,
            _currentUserMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesCategory()
    {
        // Arrange
        var categoryId = "cat-1";
        var existingCategory = Category.Create("Old Name", "Old Description");
        typeof(Category).GetProperty("Id")!.SetValue(existingCategory, categoryId);

        var updateDto = new UpdateCategoryDto("New Name", "New Description");
        var command = new UpdateCategoryCommand(categoryId, updateDto);
        var expectedDto = new CategoryDto(categoryId, "New Name", "New Description", DateTime.UtcNow, DateTime.UtcNow);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _currentUserMock.Setup(x => x.UserId).Returns("user-1");
        _currentUserMock.Setup(x => x.Username).Returns("testuser");

        _mapperMock
            .Setup(x => x.Map<CategoryDto>(It.IsAny<Category>()))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Name");
        _categoryRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.LogAsync(
            "user-1", "testuser", "Update", "Category",
            categoryId, It.IsAny<string>(), "Category updated",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var categoryId = "non-existent";
        var updateDto = new UpdateCategoryDto("New Name", "New Description");
        var command = new UpdateCategoryCommand(categoryId, updateDto);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NullCurrentUser_UsesSystemForAudit()
    {
        // Arrange
        var categoryId = "cat-1";
        var existingCategory = Category.Create("Old Name", "Old Description");
        typeof(Category).GetProperty("Id")!.SetValue(existingCategory, categoryId);

        var updateDto = new UpdateCategoryDto("New Name", "New Description");
        var command = new UpdateCategoryCommand(categoryId, updateDto);
        var expectedDto = new CategoryDto(categoryId, "New Name", "New Description", DateTime.UtcNow, DateTime.UtcNow);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _currentUserMock.Setup(x => x.UserId).Returns((string?)null);
        _currentUserMock.Setup(x => x.Username).Returns((string?)null);

        _mapperMock
            .Setup(x => x.Map<CategoryDto>(It.IsAny<Category>()))
            .Returns(expectedDto);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _auditServiceMock.Verify(x => x.LogAsync(
            "system", "system", "Update", "Category",
            categoryId, It.IsAny<string>(), "Category updated",
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
