using AutoMapper;
using FluentAssertions;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using HypeSoft.Application.Products.Commands;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Products;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _auditServiceMock = new Mock<IAuditService>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _auditServiceMock.Object,
            _currentUserMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesProduct()
    {
        var categoryId = "cat-1";
        var createDto = new CreateProductDto("Test Product", "Test Description", 99.99m, categoryId, 100);
        var command = new CreateProductCommand(createDto);

        var category = Category.Create("Test Category", "Test Description");
        typeof(Category).GetProperty("Id")!.SetValue(category, categoryId);

        _productRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _currentUserMock.Setup(x => x.UserId).Returns("user-1");
        _currentUserMock.Setup(x => x.Username).Returns("testuser");
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Product");
        result.Price.Should().Be(99.99m);
        result.CategoryName.Should().Be("Test Category");
        _productRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.LogAsync(
            "user-1", "testuser", "Create", "Product",
            It.IsAny<string>(), "Test Product", It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_UsesUnknownCategoryName()
    {
        var categoryId = "cat-1";
        var createDto = new CreateProductDto("Test Product", "Test Description", 99.99m, categoryId, 100);
        var command = new CreateProductCommand(createDto);

        _productRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        _currentUserMock.Setup(x => x.UserId).Returns("user-1");
        _currentUserMock.Setup(x => x.Username).Returns("testuser");
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.CategoryName.Should().Be("Unknown");
    }

    [Fact]
    public async Task Handle_NullCurrentUser_UsesSystemForAudit()
    {
        var categoryId = "cat-1";
        var createDto = new CreateProductDto("Test Product", "Test Description", 99.99m, categoryId, 100);
        var command = new CreateProductCommand(createDto);

        _productRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        _currentUserMock.Setup(x => x.UserId).Returns((string?)null);
        _currentUserMock.Setup(x => x.Username).Returns((string?)null);
        await _handler.Handle(command, CancellationToken.None);
        _auditServiceMock.Verify(x => x.LogAsync(
            "system", "system", "Create", "Product",
            It.IsAny<string>(), "Test Product", It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_LowStockProduct_SetsIsLowStockTrue()
    {
        var categoryId = "cat-1";
        var createDto = new CreateProductDto("Test Product", "Test Description", 99.99m, categoryId, 5);
        var command = new CreateProductCommand(createDto);

        _productRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        _currentUserMock.Setup(x => x.UserId).Returns("user-1");
        _currentUserMock.Setup(x => x.Username).Returns("testuser");
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.IsLowStock.Should().BeTrue();
    }
}
