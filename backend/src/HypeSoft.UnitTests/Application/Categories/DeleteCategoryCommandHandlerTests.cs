using FluentAssertions;
using HypeSoft.Application.Categories.Commands;
using HypeSoft.Application.Interfaces;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Categories;

public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _auditServiceMock = new Mock<IAuditService>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _handler = new DeleteCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _auditServiceMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_DeletesCategory()
    {
        var categoryId = "cat-1";
        var existingCategory = Category.Create("Test Category", "Test Description");
        typeof(Category).GetProperty("Id")!.SetValue(existingCategory, categoryId);

        var command = new DeleteCategoryCommand(categoryId);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock
            .Setup(x => x.DeleteAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _currentUserMock.Setup(x => x.UserId).Returns("user-1");
        _currentUserMock.Setup(x => x.Username).Returns("testuser");
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().BeTrue();
        _categoryRepositoryMock.Verify(x => x.DeleteAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.LogAsync(
            "user-1", "testuser", "Delete", "Category",
            categoryId, "Test Category", "Category deleted",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsKeyNotFoundException()
    {
        var categoryId = "non-existent";
        var command = new DeleteCategoryCommand(categoryId);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DeleteFails_ReturnsFalse()
    {
        var categoryId = "cat-1";
        var existingCategory = Category.Create("Test Category", "Test Description");
        typeof(Category).GetProperty("Id")!.SetValue(existingCategory, categoryId);

        var command = new DeleteCategoryCommand(categoryId);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock
            .Setup(x => x.DeleteAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _currentUserMock.Setup(x => x.UserId).Returns("user-1");
        _currentUserMock.Setup(x => x.Username).Returns("testuser");
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NullCurrentUser_UsesSystemForAudit()
    {
        var categoryId = "cat-1";
        var existingCategory = Category.Create("Test Category", "Test Description");
        typeof(Category).GetProperty("Id")!.SetValue(existingCategory, categoryId);

        var command = new DeleteCategoryCommand(categoryId);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock
            .Setup(x => x.DeleteAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _currentUserMock.Setup(x => x.UserId).Returns((string?)null);
        _currentUserMock.Setup(x => x.Username).Returns((string?)null);
        await _handler.Handle(command, CancellationToken.None);
        _auditServiceMock.Verify(x => x.LogAsync(
            "system", "system", "Delete", "Category",
            categoryId, "Test Category", "Category deleted",
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
