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
        var query = new SearchProductsQuery("Notebook");

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyName_ShouldHaveError()
    {
        var query = new SearchProductsQuery("");

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NullName_ShouldHaveError()
    {
        var query = new SearchProductsQuery(null!);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameTooShort_ShouldHaveError()
    {
        var query = new SearchProductsQuery("A");

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("mínimo"));
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("Not")]
    [InlineData("Notebook Dell")]
    public void Validate_ValidSearchTerms_ShouldBeValid(string name)
    {
        var query = new SearchProductsQuery(name);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }
}
