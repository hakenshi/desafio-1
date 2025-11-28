using HypeSoft.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HypeSoft.Application.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(ICacheService cacheService, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only cache queries (requests that don't modify data)
        if (!IsQuery(request))
        {
            return await next();
        }

        var cacheKey = GenerateCacheKey(request);
        
        // Execute the request
        var response = await next();

        return response;
    }

    private static bool IsQuery(TRequest request)
    {
        var requestType = request.GetType();
        return requestType.Name.Contains("Query") || requestType.Name.Contains("Get");
    }

    private static string GenerateCacheKey(TRequest request)
    {
        var requestType = request.GetType().Name;
        var properties = request.GetType().GetProperties()
            .Select(p => $"{p.Name}:{p.GetValue(request)}")
            .ToArray();
        
        return $"{requestType}:{string.Join(":", properties)}";
    }

    private static TimeSpan GetCacheExpiration(TRequest request)
    {
        // Dashboard data: 1 minute
        if (request.GetType().Name.Contains("Dashboard"))
            return TimeSpan.FromMinutes(1);

        // Product lists: 2 minutes
        if (request.GetType().Name.Contains("GetAll"))
            return TimeSpan.FromMinutes(2);

        // Single product: 5 minutes
        if (request.GetType().Name.Contains("GetById"))
            return TimeSpan.FromMinutes(5);

        // Default: 3 minutes
        return TimeSpan.FromMinutes(3);
    }
}
