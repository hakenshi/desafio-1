using HypeSoft.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HypeSoft.Application.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
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
        if (!IsQuery(request))
        {
            return await next();
        }

        var cacheKey = GenerateCacheKey(request);
        var cachedResponse = await _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
        if (cachedResponse != null)
        {
            _logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
            return cachedResponse;
        }

        _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);
        var response = await next();
        if (response != null)
        {
            var expiration = GetCacheExpiration(request);
            await _cacheService.SetAsync(cacheKey, response, expiration, cancellationToken);
            _logger.LogDebug("Cached response for {CacheKey} with expiration {Expiration}", cacheKey, expiration);
        }

        return response!;
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
        if (request.GetType().Name.Contains("Dashboard"))
            return TimeSpan.FromMinutes(1);
        if (request.GetType().Name.Contains("GetAll"))
            return TimeSpan.FromMinutes(2);
        if (request.GetType().Name.Contains("GetById"))
            return TimeSpan.FromMinutes(5);
        return TimeSpan.FromMinutes(3);
    }
}
