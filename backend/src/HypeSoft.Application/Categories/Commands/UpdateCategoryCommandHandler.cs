using AutoMapper;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Categories.Commands;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IAuditService auditService,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _auditService = auditService;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {request.Id} not found");

        category.Update(
            request.Category.Name,
            request.Category.Description
        );

        await _categoryRepository.UpdateAsync(category, cancellationToken);

        await _auditService.LogAsync(
            _currentUser.UserId ?? "system",
            _currentUser.Username ?? "system",
            "Update",
            "Category",
            category.Id,
            category.Name,
            "Category updated",
            cancellationToken
        );

        return _mapper.Map<CategoryDto>(category);
    }
}
