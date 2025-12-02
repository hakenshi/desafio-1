using HypeSoft.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HypeSoft.Application.Behaviors;

public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheInvalidationBehavior<TRequest, TResponse>> _logger;

    public CacheInvalidationBehavior(ICacheService cacheService, ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        // Invalidate cache for commands (operations that modify data)
        if (IsCommand(request))
        {
            try
            {
                await InvalidateRelatedCaches(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error invalidating cache for {RequestType}", typeof(TRequest).Name);
            }
        }

        return response;
    }

    private static bool IsCommand(TRequest request)
    {
        var requestType = request.GetType();
        return requestType.Name.Contains("Command") || 
               requestType.Name.Contains("Create") || 
               requestType.Name.Contains("Update") || 
               requestType.Name.Contains("Delete");
    }

    private async Task InvalidateRelatedCaches(TRequest request, CancellationToken cancellationToken)
    {
        var requestType = request.GetType().Name;

        // Invalidate product-related caches
        if (requestType.Contains("Product"))
        {
            await _cacheService.RemoveByPrefixAsync("GetAllProductsQuery:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("GetProductByIdQuery:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("GetLowStockProductsQuery:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("SearchProductsQuery:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("GetDashboardQuery:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("GetRecentProductsQuery:", cancellationToken);
            
            _logger.LogInformation("Invalidated product-related caches for {RequestType}", requestType);
        }

        // Invalidate category-related caches
        if (requestType.Contains("Category"))
        {
            await _cacheService.RemoveByPrefixAsync("GetAllCategoriesQuery:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("GetCategoryByIdQuery:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("GetDashboardQuery:", cancellationToken);
            
            _logger.LogInformation("Invalidated category-related caches for {RequestType}", requestType);
        }
    }
}
