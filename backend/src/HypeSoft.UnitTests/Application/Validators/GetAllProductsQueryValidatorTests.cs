using FluentAssertions;
using HypeSoft.Application.Products.Queries;
using HypeSoft.Application.Products.Validators;

namespace HypeSoft.UnitTests.Application.Validators;

public class GetAllProductsQueryValidatorTests
{
    private readonly GetAllProductsQueryValidator _validator;

    public GetAllProductsQueryValidatorTests()
    {
        _validator = new GetAllProductsQueryValidator();
    }

    [Fact]
    public void Validate_ValidQuery_ShouldNotHaveErrors()
    {
        // Arrange
        var query = new GetAllProductsQuery(1, 10);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Validate_InvalidPage_ShouldHaveError(int page)
    {
        // Arrange
        var query = new GetAllProductsQuery(page, 10);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Page");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidPageSize_ShouldHaveError(int pageSize)
    {
        // Arrange
        var query = new GetAllProductsQuery(1, pageSize);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_PageSizeTooLarge_ShouldHaveError()
    {
        // Arrange
        var query = new GetAllProductsQuery(1, 101);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageSize" && e.ErrorMessage.Contains("100"));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 50)]
    [InlineData(1, 100)]
    [InlineData(5, 20)]
    public void Validate_ValidPaginationValues_ShouldBeValid(int page, int pageSize)
    {
        // Arrange
        var query = new GetAllProductsQuery(page, pageSize);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
