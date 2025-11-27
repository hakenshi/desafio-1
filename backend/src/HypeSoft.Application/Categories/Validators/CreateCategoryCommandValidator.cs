using FluentValidation;
using HypeSoft.Application.Categories.Commands;

namespace HypeSoft.Application.Categories.Validators;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator(CreateCategoryValidator categoryValidator)
    {
        RuleFor(x => x.Category)
            .NotNull().WithMessage("Dados da categoria são obrigatórios")
            .SetValidator(categoryValidator);
    }
}
