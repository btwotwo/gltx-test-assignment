using System.Text.Json.Serialization;
using CountryGwpApi.CountryGwp.Services.Calculator;
using Microsoft.AspNetCore.Mvc;

namespace CountryGwpApi.CountryGwp.Controllers;

public record AverageGwpRequestParams(string Country, [property: JsonPropertyName("lob")] string[] LineOfBusiness);
public record AverageRequestResponse(decimal Transport, decimal Liability);

[Route("gwp")]
public class CountryGwp : Controller
{
    private readonly IAverageGwpCalculator _calculator;

    public CountryGwp(IAverageGwpCalculator calculator)
    {
        _calculator = calculator;
    }
    [Route("avg")]
    [HttpPost]
    public async Task<ActionResult<Dictionary<string, decimal>>> GetAverage(AverageGwpRequestParams parameters)
    {
        try
        {
            var calculationResult = await _calculator.CalculateAsync(parameters.Country, parameters.LineOfBusiness);
            return calculationResult;
        }
        catch (LobDoesNotExistException e)
        {
            return BadRequest($"Line Of Business '{e.LineOfBusiness}' does not exist.");
        }
    }
}