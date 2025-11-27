using FluentAssertions;
using HypeSoft.Application.Products.Queries;
using HypeSoft.Application.Products.Validators;

namespace HypeSoft.UnitTests.Application.Validators;

public class SearchProductsQueryValidatorTests
{
    private readonly SearchProductsQueryValidator _validator;

    public SearchProductsQueryValidatorTests()
    {
        _validator = new SearchProductsQueryValidator();
    }

    [Fact]
    public void Validate_ValidSearchQuery_ShouldNotHaveErrors()
    {
        // Arrange
        var query = new SearchProductsQuery("Notebook");

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyName_ShouldHaveError(string name)
    {
        // Arrange
        var query = new SearchProductsQuery(name);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameTooShort_ShouldHaveError()
    {
        // Arrange
        var query = new SearchProductsQuery("A");

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("mínimo"));
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("Not")]
    [InlineData("Notebook Dell")]
    public void Validate_ValidSearchTerms_ShouldBeValid(string name)
    {
        // Arrange
        var query = new SearchProductsQuery(name);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
