using FluentValidation;
using HypeSoft.Application.Products.Queries;

namespace HypeSoft.Application.Products.Validators;

public class GetAllProductsQueryValidator : AbstractValidator<GetAllProductsQuery>
{
    public GetAllProductsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Página deve ser maior que zero");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Tamanho da página deve ser maior que zero")
            .LessThanOrEqualTo(100).WithMessage("Tamanho da página deve ser menor ou igual a 100");
    }
}
