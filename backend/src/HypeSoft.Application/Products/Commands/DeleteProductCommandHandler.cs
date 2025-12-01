using HypeSoft.Application.Interfaces;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Products.Commands;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUser;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        IAuditService auditService,
        ICurrentUserService currentUser)
    {
        _productRepository = productRepository;
        _auditService = auditService;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        var productName = product?.Name;

        await _productRepository.DeleteAsync(request.Id, cancellationToken);

        await _auditService.LogAsync(
            _currentUser.UserId ?? "system",
            _currentUser.Username ?? "system",
            "Delete",
            "Product",
            request.Id,
            productName,
            "Product deleted",
            cancellationToken
        );

        return Unit.Value;
    }
}
