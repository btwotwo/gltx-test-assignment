namespace CountryGwpApi.CountryGwp.Services.Calculator;

public interface IAverageGwpCalculator
{
    Task<Dictionary<string, decimal>> CalculateAsync(string country, string[] lineOfBusiness);
}