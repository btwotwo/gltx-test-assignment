using System.Collections.Immutable;
using CountryGwpApi.CountryGwp.Data.Csv;

namespace CountryGwpApi.CountryGwp.Data.Repository;

public class InMemoryGwpDataRepository : IGwpDataRepository
{
    private readonly IGwpByCountryCsvLoader _csvLoader;

    public InMemoryGwpDataRepository(IGwpByCountryCsvLoader csvLoader)
    {
        _csvLoader = csvLoader;
    }

    public async Task<IReadOnlyCollection<GwpByCountryDataModel>> GetByCountryAsync(string country)
    {
        var data = await _csvLoader.LoadAsync();

        return data
            .Where(x => string.Equals(x.Country, country, StringComparison.InvariantCulture)).ToImmutableArray();
    }
}