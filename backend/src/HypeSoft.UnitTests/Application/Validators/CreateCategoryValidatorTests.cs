using FluentAssertions;
using HypeSoft.Application.Categories.Validators;
using HypeSoft.Application.DTOs;

namespace HypeSoft.UnitTests.Application.Validators;

public class CreateCategoryValidatorTests
{
    private readonly CreateCategoryValidator _validator;

    public CreateCategoryValidatorTests()
    {
        _validator = new CreateCategoryValidator();
    }

    [Fact]
    public void Validate_ValidCategory_ShouldNotHaveErrors()
    {
        var dto = new CreateCategoryDto("Valid Category", "Valid description for category");

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyName_ShouldHaveError()
    {
        var dto = new CreateCategoryDto("", "Valid description");

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NullName_ShouldHaveError()
    {
        var dto = new CreateCategoryDto(null!, "Valid description");

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameTooShort_ShouldHaveError()
    {
        var dto = new CreateCategoryDto("AB", "Valid description");

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("mínimo"));
    }

    [Fact]
    public void Validate_NameTooLong_ShouldHaveError()
    {
        var longName = new string('a', 101);
        var dto = new CreateCategoryDto(longName, "Valid description");

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("100"));
    }

    [Fact]
    public void Validate_EmptyDescription_ShouldHaveError()
    {
        var dto = new CreateCategoryDto("Valid Name", "");

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_NullDescription_ShouldHaveError()
    {
        var dto = new CreateCategoryDto("Valid Name", null!);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_DescriptionTooLong_ShouldHaveError()
    {
        var longDescription = new string('a', 501);
        var dto = new CreateCategoryDto("Valid Name", longDescription);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description" && e.ErrorMessage.Contains("500"));
    }
}
