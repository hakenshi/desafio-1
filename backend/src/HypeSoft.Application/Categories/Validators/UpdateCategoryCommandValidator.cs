using FluentValidation;
using HypeSoft.Application.Categories.Commands;

namespace HypeSoft.Application.Categories.Validators;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required");

        RuleFor(x => x.Category)
            .NotNull().WithMessage("Category data is required")
            .SetValidator(new UpdateCategoryValidator());
    }
}
