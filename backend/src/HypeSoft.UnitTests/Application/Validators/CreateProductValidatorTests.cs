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
        // Arrange
        var dto = new CreateProductDto(
            "Valid Product",
            "Valid Description",
            99.99m,
            "category-1",
            10
        );

        // Act
        var result = _validator.Validate(dto);

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
        var dto = new CreateProductDto(name, "Description", 10m, "cat1", 5);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameTooLong_ShouldHaveError()
    {
        // Arrange
        var longName = new string('a', 201);
        var dto = new CreateProductDto(longName, "Description", 10m, "cat1", 5);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("200"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyDescription_ShouldHaveError(string description)
    {
        // Arrange
        var dto = new CreateProductDto("Name", description, 10m, "cat1", 5);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_InvalidPrice_ShouldHaveError(decimal price)
    {
        // Arrange
        var dto = new CreateProductDto("Name", "Description", price, "cat1", 5);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyCategoryId_ShouldHaveError(string categoryId)
    {
        // Arrange
        var dto = new CreateProductDto("Name", "Description", 10m, categoryId, 5);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryId");
    }

    [Fact]
    public void Validate_NegativeStockQuantity_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateProductDto("Name", "Description", 10m, "cat1", -5);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StockQuantity");
    }

    [Fact]
    public void Validate_ZeroStockQuantity_ShouldBeValid()
    {
        // Arrange
        var dto = new CreateProductDto("Name", "Description", 10m, "cat1", 0);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
