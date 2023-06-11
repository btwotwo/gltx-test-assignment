using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace CountryGwpApi.CountryGwp.Services.Calculator;

public class AverageGwpCalculatorCacheDecorator: IAverageGwpCalculator
{
    private readonly IAverageGwpCalculator _calculator;
    private readonly IMemoryCache _memoryCache;

    // This might be improved, for example we can cache individual "country-lob" pairs
    public AverageGwpCalculatorCacheDecorator(IAverageGwpCalculator calculator, IMemoryCache memoryCache)
    {
        _calculator = calculator;
        _memoryCache = memoryCache;
    }
    public Task<Dictionary<string, decimal>> CalculateAsync(string country, string[] lineOfBusiness)
    {
        var joinedLobs = string.Join("", lineOfBusiness);
        var key = $"{country}_{joinedLobs}";
        return _memoryCache.GetOrCreateAsync<Dictionary<string, decimal>>(key, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(15);
            return _calculator.CalculateAsync(country, lineOfBusiness);
        })!;
    }
}