using AutoMapper;
using FluentAssertions;
using HypeSoft.Application.Categories.Queries;
using HypeSoft.Application.DTOs;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Categories;

public class GetCategoryByIdQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetCategoryByIdQueryHandler _handler;

    public GetCategoryByIdQueryHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetCategoryByIdQueryHandler(
            _categoryRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingCategory_ReturnsCategory()
    {
        // Arrange
        var categoryId = "cat-1";
        var category = Category.Create("Test Category", "Test Description");
        typeof(Category).GetProperty("Id")!.SetValue(category, categoryId);

        var expectedDto = new CategoryDto(categoryId, "Test Category", "Test Description", DateTime.UtcNow, DateTime.UtcNow);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _mapperMock
            .Setup(x => x.Map<CategoryDto>(category))
            .Returns(expectedDto);

        var query = new GetCategoryByIdQuery(categoryId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(categoryId);
        result.Name.Should().Be("Test Category");
    }

    [Fact]
    public async Task Handle_NonExistingCategory_ReturnsNull()
    {
        // Arrange
        var categoryId = "non-existent";

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var query = new GetCategoryByIdQuery(categoryId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
