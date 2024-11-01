using Microsoft.Extensions.Caching.Memory;

namespace Grid.Loaders;

public class CurrencyCacheLoader : ICurrencyLoader
{
    private readonly CnbLoader _cnbLoader;
    private readonly IMemoryCache _cache;
    private const string EUR_TO_CZK_CACHE_KEY = "EurToCzkRate";

    public CurrencyCacheLoader(CnbLoader cnbLoader, IMemoryCache cache)
    {
        _cnbLoader = cnbLoader;
        _cache = cache;
    }
    public async Task<decimal> LoadEurToCzkCurrencyAsync()
    {
        if (_cache.TryGetValue(EUR_TO_CZK_CACHE_KEY, out decimal currency))
        {
            return currency;
        }
        else
        {
            var currencyToCache = await _cnbLoader.LoadEurToCzkCurrencyAsync();

            var now = DateTime.UtcNow; //teď 
            var nextMidnight = DateTime.Today.AddDays(1).ToUniversalTime(); //zítra v 00:00
            var durationUntilMidnight = nextMidnight - now; // zítra 00:00 - teď 
            
            _cache.Set(EUR_TO_CZK_CACHE_KEY, currencyToCache, durationUntilMidnight);
            return currencyToCache;
        }
    }
}
