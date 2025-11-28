using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Categories.Commands;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {request.Id} not found");

        return await _categoryRepository.DeleteAsync(request.Id, cancellationToken);
    }
}
