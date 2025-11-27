using FluentValidation;
using HypeSoft.Application.Products.Commands;

namespace HypeSoft.Application.Products.Validators;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator(UpdateProductValidator productValidator)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID do produto é obrigatório");

        RuleFor(x => x.Product)
            .NotNull().WithMessage("Dados do produto são obrigatórios")
            .SetValidator(productValidator);
    }
}
