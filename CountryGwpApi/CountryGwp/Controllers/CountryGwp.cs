using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CountryGwpApi.CountryGwp.Services.Calculator;
using Microsoft.AspNetCore.Mvc;

namespace CountryGwpApi.CountryGwp.Controllers;

public record AverageGwpRequestParams(
    [StringLength(999), Required, MinLength(2)]
    string Country, 
    [property: JsonPropertyName("lob")]
    [Required, MinLength(1)]
    string[] LineOfBusiness);
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
    public async Task<ActionResult<Dictionary<string, decimal>>> GetAverage([FromBody]AverageGwpRequestParams parameters)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
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