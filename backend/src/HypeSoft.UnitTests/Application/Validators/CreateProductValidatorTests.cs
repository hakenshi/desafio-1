using FluentAssertions;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Products.Validators;

namespace HypeSoft.UnitTests.Application.Validators;

public class CreateProductValidatorTests
{
    private readonly CreateProductValidator _validator;

    public CreateProductValidatorTests()
    {
        _validator = new CreateProductValidator();
    }

    [Fact]
    public void Validate_ValidProduct_ShouldNotHaveErrors()
    {
        var dto = new CreateProductDto(
            "Valid Product Name",
            "Valid Description with more than 10 characters",
            99.99m,
            "category-1",
            10
        );

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyName_ShouldHaveError()
    {
        var dto = new CreateProductDto("", "Valid Description Here", 10m, "cat1", 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NullName_ShouldHaveError()
    {
        var dto = new CreateProductDto(null!, "Valid Description Here", 10m, "cat1", 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameTooShort_ShouldHaveError()
    {
        var dto = new CreateProductDto("AB", "Valid Description Here", 10m, "cat1", 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("mínimo"));
    }

    [Fact]
    public void Validate_NameTooLong_ShouldHaveError()
    {
        var longName = new string('a', 201);
        var dto = new CreateProductDto(longName, "Valid Description Here", 10m, "cat1", 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("200"));
    }

    [Fact]
    public void Validate_EmptyDescription_ShouldHaveError()
    {
        var dto = new CreateProductDto("Valid Name", "", 10m, "cat1", 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_NullDescription_ShouldHaveError()
    {
        var dto = new CreateProductDto("Valid Name", null!, 10m, "cat1", 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_DescriptionTooShort_ShouldHaveError()
    {
        var dto = new CreateProductDto("Valid Name", "Short", 10m, "cat1", 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description" && e.ErrorMessage.Contains("mínimo"));
    }

    [Fact]
    public void Validate_DescriptionTooLong_ShouldHaveError()
    {
        var longDescription = new string('a', 1001);
        var dto = new CreateProductDto("Valid Name", longDescription, 10m, "cat1", 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description" && e.ErrorMessage.Contains("1000"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_InvalidPrice_ShouldHaveError(decimal price)
    {
        var dto = new CreateProductDto("Valid Name", "Valid Description Here", price, "cat1", 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_PriceTooHigh_ShouldHaveError()
    {
        var dto = new CreateProductDto("Valid Name", "Valid Description Here", 1000001m, "cat1", 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price" && e.ErrorMessage.Contains("1.000.000"));
    }

    [Fact]
    public void Validate_EmptyCategoryId_ShouldHaveError()
    {
        var dto = new CreateProductDto("Valid Name", "Valid Description Here", 10m, "", 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryId");
    }

    [Fact]
    public void Validate_NullCategoryId_ShouldHaveError()
    {
        var dto = new CreateProductDto("Valid Name", "Valid Description Here", 10m, null!, 5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryId");
    }

    [Fact]
    public void Validate_NegativeStockQuantity_ShouldHaveError()
    {
        var dto = new CreateProductDto("Valid Name", "Valid Description Here", 10m, "cat1", -5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StockQuantity");
    }

    [Fact]
    public void Validate_StockQuantityTooHigh_ShouldHaveError()
    {
        var dto = new CreateProductDto("Valid Name", "Valid Description Here", 10m, "cat1", 100001);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StockQuantity" && e.ErrorMessage.Contains("100.000"));
    }

    [Fact]
    public void Validate_ZeroStockQuantity_ShouldBeValid()
    {
        var dto = new CreateProductDto("Valid Name", "Valid Description Here", 10m, "cat1", 0);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_MultipleErrors_ShouldReturnAllErrors()
    {
        var dto = new CreateProductDto("AB", "Short", -10m, "", -5);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(3);
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryId");
        result.Errors.Should().Contain(e => e.PropertyName == "StockQuantity");
    }
}
