using FluentValidation;
using HypeSoft.Application.Products.Queries;

namespace HypeSoft.Application.Products.Validators;

public class SearchProductsQueryValidator : AbstractValidator<SearchProductsQuery>
{
    public SearchProductsQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome para busca é obrigatório")
            .MinimumLength(2).WithMessage("Nome para busca deve ter no mínimo 2 caracteres");
    }
}
