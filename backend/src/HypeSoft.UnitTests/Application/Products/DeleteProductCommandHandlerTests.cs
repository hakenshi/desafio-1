using FluentAssertions;
using HypeSoft.Application.Interfaces;
using HypeSoft.Application.Products.Commands;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using MediatR;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Products;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _auditServiceMock = new Mock<IAuditService>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _handler = new DeleteProductCommandHandler(
            _productRepositoryMock.Object,
            _auditServiceMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_DeletesProduct()
    {
        // Arrange
        var productId = "prod-1";
        var existingProduct = Product.Create("Test Product", "Test Description", 10.0m, "cat-1", 100);
        typeof(Product).GetProperty("Id")!.SetValue(existingProduct, productId);

        var command = new DeleteProductCommand(productId);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _productRepositoryMock
            .Setup(x => x.DeleteAsync(productId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _currentUserMock.Setup(x => x.UserId).Returns("user-1");
        _currentUserMock.Setup(x => x.Username).Returns("testuser");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        _productRepositoryMock.Verify(x => x.DeleteAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.LogAsync(
            "user-1", "testuser", "Delete", "Product",
            productId, "Test Product", "Product deleted",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductNotFound_StillDeletesAndLogsWithNullName()
    {
        // Arrange
        var productId = "non-existent";
        var command = new DeleteProductCommand(productId);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        _productRepositoryMock
            .Setup(x => x.DeleteAsync(productId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _currentUserMock.Setup(x => x.UserId).Returns("user-1");
        _currentUserMock.Setup(x => x.Username).Returns("testuser");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        _productRepositoryMock.Verify(x => x.DeleteAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.LogAsync(
            "user-1", "testuser", "Delete", "Product",
            productId, null, "Product deleted",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NullCurrentUser_UsesSystemForAudit()
    {
        // Arrange
        var productId = "prod-1";
        var existingProduct = Product.Create("Test Product", "Test Description", 10.0m, "cat-1", 100);
        typeof(Product).GetProperty("Id")!.SetValue(existingProduct, productId);

        var command = new DeleteProductCommand(productId);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _productRepositoryMock
            .Setup(x => x.DeleteAsync(productId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _currentUserMock.Setup(x => x.UserId).Returns((string?)null);
        _currentUserMock.Setup(x => x.Username).Returns((string?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _auditServiceMock.Verify(x => x.LogAsync(
            "system", "system", "Delete", "Product",
            productId, "Test Product", "Product deleted",
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
