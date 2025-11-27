using FluentValidation;
using HypeSoft.Application.Products.Commands;

namespace HypeSoft.Application.Products.Validators;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(CreateProductValidator productValidator)
    {
        RuleFor(x => x.Product)
            .NotNull().WithMessage("Dados do produto são obrigatórios")
            .SetValidator(productValidator);
    }
}
