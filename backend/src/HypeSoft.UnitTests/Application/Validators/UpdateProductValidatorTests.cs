using FluentAssertions;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Products.Validators;

namespace HypeSoft.UnitTests.Application.Validators;

public class UpdateProductValidatorTests
{
    private readonly UpdateProductValidator _validator;

    public UpdateProductValidatorTests()
    {
        _validator = new UpdateProductValidator();
    }

    [Fact]
    public void Validate_ValidProduct_ShouldNotHaveErrors()
    {
        var dto = new UpdateProductDto(
            "Valid Product",
            "Valid Description with more than 10 chars",
            99.99m,
            "category-1",
            10
        );

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_NameTooShort_ShouldHaveError()
    {
        var dto = new UpdateProductDto("AB", "Valid Description", 10m, "cat1", 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("mínimo"));
    }

    [Fact]
    public void Validate_PriceTooHigh_ShouldHaveError()
    {
        var dto = new UpdateProductDto("Product", "Description here", 1000001m, "cat1", 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_StockQuantityTooHigh_ShouldHaveError()
    {
        var dto = new UpdateProductDto("Product", "Description here", 100m, "cat1", 100001);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StockQuantity");
    }
}
