namespace CountryGwpApi.CountryGwp.Data.Csv;

public interface IGwpByCountryCsvLoader
{
    Task<IReadOnlyCollection<GwpByCountryDataModel>> LoadAsync();
}