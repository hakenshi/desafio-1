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
            .NotNull().WithMessage("Category data is required");
        
        When(x => x.Category != null, () =>
        {
            RuleFor(x => x.Category!.Name)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres");

            RuleFor(x => x.Category!.Description)
                .NotEmpty().WithMessage("Descrição é obrigatória")
                .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");
        });
    }
}
