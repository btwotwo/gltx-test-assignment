using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace CountryGwpApi.CountryGwp.Data.Csv;

public class GwpByCountryCsvFileLoader : IGwpByCountryCsvLoader
{
    private const string
        CsvFilePath = "gwpByCountry.csv"; // path hardcoded for the simplicity, but better to move it to config
    private IReadOnlyCollection<GwpByCountryDataModel>? _cachedModel = null;
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

    public async Task<IReadOnlyCollection<GwpByCountryDataModel>> LoadAsync()
    {
        try
        {
            await _lock.WaitAsync();
            if (_cachedModel is not null)
            {
                return _cachedModel;
            }

            _cachedModel = await LoadDataFromCsv();
            return _cachedModel;
        }
        finally
        {
            _lock.Release();
        }
    }

    private static async Task<IReadOnlyCollection<GwpByCountryDataModel>> LoadDataFromCsv()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture);
        using var stream = new StreamReader(CsvFilePath);
        using var reader = new CsvReader(stream, new CsvConfiguration(CultureInfo.InvariantCulture));
        reader.Context.TypeConverterOptionsCache.GetOptions<decimal?>().NumberStyles = NumberStyles.Number |
            NumberStyles.AllowDecimalPoint |
            NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign;

        var records = reader.GetRecordsAsync<GwpByCountryDataModel>();
        return await records.ToListAsync();
    }
}