using CountryGwpApi.CountryGwp.Controllers;
using CountryGwpApi.CountryGwp.Data.Csv;
using CountryGwpApi.CountryGwp.Data.Repository;

namespace CountryGwpApi.CountryGwp.Services.Calculator;

class LobDoesNotExistException : Exception
{
    public string LineOfBusiness { get; }
    public LobDoesNotExistException(string lineOfBusiness) : base(
        $"Line Of Business does not exist. [LoB = {lineOfBusiness}]")
    {
        LineOfBusiness = lineOfBusiness;
    }
}


public class AverageGwpCalculator : IAverageGwpCalculator
{
    private readonly IGwpDataRepository _gwpDataRepository;

    public AverageGwpCalculator(IGwpDataRepository gwpDataRepository)
    {
        _gwpDataRepository = gwpDataRepository;
    }

    public async Task<Dictionary<string, decimal>> CalculateAsync(string country, string[] lineOfBusiness)
    {
        var data = await _gwpDataRepository.GetByCountryAsync(country);
        var result = new Dictionary<string, decimal>();

        foreach (var lob in lineOfBusiness)
        {
            var lobEntry =
                data.SingleOrDefault(m => string.Equals(lob, m.LineOfBusiness, StringComparison.InvariantCulture));
            
            if (lobEntry is null)
            {
                throw new LobDoesNotExistException(lob);
            }

            var avg = CalculateAverage(lobEntry, lob);
            result.Add(lob, avg);
        }

        return result;
    }

    private static decimal CalculateAverage(GwpByCountryDataModel lobEntry, string lob)
    {
        var existingCount = 0;

        decimal Val(decimal? value)
        {
            if (!value.HasValue)
            {
                return 0;
            }

            existingCount += 1;
            return value.Value;
        }

        var sum = Val(lobEntry.Y2008) + Val(lobEntry.Y2009) + Val(lobEntry.Y2010) + Val(lobEntry.Y2011) +
                  Val(lobEntry.Y2012) + Val(lobEntry.Y2013) + Val(lobEntry.Y2014) + Val(lobEntry.Y2015);

        if (existingCount != 0)
        {
            return sum / existingCount;
        }
        else
        {
            return 0;
        }
    }
}