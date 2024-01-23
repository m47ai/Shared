namespace M47.Shared.Application.MediatR.Behaviors;

using EasyCaching.Core;
using global::MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
    private readonly IEasyCachingProvider _provider;
    private readonly ICacheRequest _cacheRequest;
    private readonly int defaultCacheExpirationInHours = 1;

    public CachingBehavior(IEasyCachingProvider provider,
                           ILogger<CachingBehavior<TRequest, TResponse>> logger, ICacheRequest cacheRequest)
    {
        _logger = logger;
        _provider = provider;
        _cacheRequest = cacheRequest;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
                                        CancellationToken cancellationToken)
    {
        if (_cacheRequest is null)
        {
            return await next();
        }

        var cacheKey = _cacheRequest.CacheKey;
        var cachedResponse = await _provider.GetAsync<TResponse>(cacheKey, cancellationToken);
        if (cachedResponse.Value is not null)
        {
            _logger.LogDebug("Fetch data from cache with cachKey: {CacheKey}", cacheKey);
            return cachedResponse.Value;
        }

        var response = await next();

        var expirationTime = _cacheRequest.AbsoluteExpirationRelativeToNow ??
                             DateTime.Now.AddHours(defaultCacheExpirationInHours);

        await _provider.SetAsync(cacheKey, response, expirationTime.TimeOfDay, cancellationToken);

        _logger.LogDebug("Set data to cache with cachKey: {CacheKey}", cacheKey);

        return response;
    }
}