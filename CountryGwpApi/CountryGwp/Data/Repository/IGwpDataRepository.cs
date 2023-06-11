using CountryGwpApi.CountryGwp.Data.Csv;

namespace CountryGwpApi.CountryGwp.Data.Repository;

public interface IGwpDataRepository
{
    public Task<IReadOnlyCollection<GwpByCountryDataModel>> GetByCountryAsync(string country);
    
    // Add, GetAll can be implemented as well if needed.
}