using FluentValidation;
using HypeSoft.Application.DTOs;

namespace HypeSoft.Application.Products.Validators;

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Descrição é obrigatória")
            .MinimumLength(10).WithMessage("Descrição deve ter no mínimo 10 caracteres")
            .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero")
            .LessThanOrEqualTo(1000000).WithMessage("Preço deve ser menor ou igual a 1.000.000");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Categoria é obrigatória");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantidade em estoque não pode ser negativa")
            .LessThanOrEqualTo(100000).WithMessage("Quantidade em estoque deve ser menor ou igual a 100.000");
    }
}
