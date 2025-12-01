using HypeSoft.Application.Interfaces;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Categories.Commands;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUser;

    public DeleteCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IAuditService auditService,
        ICurrentUserService currentUser)
    {
        _categoryRepository = categoryRepository;
        _auditService = auditService;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {request.Id} not found");

        var categoryName = category.Name;
        var result = await _categoryRepository.DeleteAsync(request.Id, cancellationToken);

        await _auditService.LogAsync(
            _currentUser.UserId ?? "system",
            _currentUser.Username ?? "system",
            "Delete",
            "Category",
            request.Id,
            categoryName,
            "Category deleted",
            cancellationToken
        );

        return result;
    }
}
