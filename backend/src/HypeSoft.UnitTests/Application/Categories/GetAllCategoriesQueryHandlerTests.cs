using AutoMapper;
using FluentAssertions;
using HypeSoft.Application.Categories.Queries;
using HypeSoft.Application.DTOs;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Categories;

public class GetAllCategoriesQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllCategoriesQueryHandler _handler;

    public GetAllCategoriesQueryHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllCategoriesQueryHandler(
            _categoryRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllCategories()
    {
        var categories = new List<Category>
        {
            Category.Create("Category 1", "Description 1"),
            Category.Create("Category 2", "Description 2"),
        };

        var categoryDtos = new List<CategoryDto>
        {
            new("cat-1", "Category 1", "Description 1", DateTime.UtcNow, DateTime.UtcNow),
            new("cat-2", "Category 2", "Description 2", DateTime.UtcNow, DateTime.UtcNow),
        };

        _categoryRepositoryMock
            .Setup(x => x.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);
        _categoryRepositoryMock
            .Setup(x => x.GetTotalCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        _mapperMock
            .Setup(x => x.Map<IEnumerable<CategoryDto>>(categories))
            .Returns(categoryDtos);

        var query = new GetAllCategoriesQuery(1, 10);
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        var categories = new List<Category>
        {
            Category.Create("Category 1", "Description 1"),
        };

        var categoryDtos = new List<CategoryDto>
        {
            new("cat-1", "Category 1", "Description 1", DateTime.UtcNow, DateTime.UtcNow),
        };

        _categoryRepositoryMock
            .Setup(x => x.GetAllAsync(2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);
        _categoryRepositoryMock
            .Setup(x => x.GetTotalCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);
        _mapperMock
            .Setup(x => x.Map<IEnumerable<CategoryDto>>(categories))
            .Returns(categoryDtos);

        var query = new GetAllCategoriesQuery(2, 5);
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.TotalCount.Should().Be(10);
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyList()
    {
        _categoryRepositoryMock
            .Setup(x => x.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());
        _categoryRepositoryMock
            .Setup(x => x.GetTotalCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _mapperMock
            .Setup(x => x.Map<IEnumerable<CategoryDto>>(It.IsAny<IEnumerable<Category>>()))
            .Returns(new List<CategoryDto>());

        var query = new GetAllCategoriesQuery(1, 10);
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }
}
