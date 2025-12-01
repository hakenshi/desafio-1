using AutoMapper;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Categories.Commands;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(
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

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(
            request.Category.Name,
            request.Category.Description
        );

        var created = await _categoryRepository.CreateAsync(category, cancellationToken);

        await _auditService.LogAsync(
            _currentUser.UserId ?? "system",
            _currentUser.Username ?? "system",
            "Create",
            "Category",
            created.Id,
            created.Name,
            $"Created category: {created.Description}",
            cancellationToken
        );

        return _mapper.Map<CategoryDto>(created);
    }
}
