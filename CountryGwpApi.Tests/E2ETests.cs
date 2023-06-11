using System.Net;
using System.Net.Http.Json;
using System.Text;
using CountryGwpApi.CountryGwp.Controllers;
using CountryGwpApi.CountryGwp.Data.Csv;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.Core;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CountryGwpApi.Tests;

public class E2ETests: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IGwpByCountryCsvLoader _csvLoaderSubstitute;
    private static readonly string Endpoint = "/server/api/gwp/avg/";

    public E2ETests(WebApplicationFactory<Program> factory)
    {
        _csvLoaderSubstitute = Substitute.For<IGwpByCountryCsvLoader>();
        _factory = factory;
    }
    
    [Fact]
    public async Task GivenBasicRequest_WhenProvidedData_CanCalculateAverage()
    {
        // Arrange
        var factory = ReplaceCsvLoader();
        SetupFakeCsv();
        var client = factory.CreateClient();
        
        // Act
        var req = new AverageGwpRequestParams("Country1", new[] { "Lob1" });
        var response = await CallApi(client, req);

        response.EnsureSuccessStatusCode();
        
        var res = await response.Content.ReadAsStringAsync();
        var averages = JsonSerializer.Deserialize<Dictionary<string, decimal>>(res);

        averages.Should().ContainKey("Lob1");
        averages.Should().ContainValue(2M);
    }

    [Fact]
    public async Task GivenRequest_WhenInvalidParameters_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var req = new AverageGwpRequestParams("e", new[] { "" });
        
        // Act
        var res = await CallApi(client, req);
        
        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GivenRequest_WhenLoadsRealData_ShouldCalculateCorrectly()
    {
        // Arrange
        var client = _factory.CreateClient();
        var req = new AverageGwpRequestParams("ae", new[] { "transport", "property" });
        
        // Act
        var res = await CallApi(client, req);
        
        // Assert
        res.EnsureSuccessStatusCode();
        var avg = JsonSerializer.Deserialize<Dictionary<string, decimal>>(await res.Content.ReadAsStringAsync());

        avg.Should().HaveCount(2);
        avg.Should().ContainKey("transport");
        avg.Should().ContainKey("property");

        avg["transport"].Should().BeApproximately(285137382.471M, 0.001M);
        avg["property"].Should().BeApproximately(599026844.914M, 0.001M);
    }

    private static async Task<HttpResponseMessage> CallApi(HttpClient client, AverageGwpRequestParams req)
    {
        var res = await client.PostAsync(Endpoint, JsonContent.Create(req));
        return res;
    }

    [Fact]
    public async Task GivenRequest_WhenLobDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        var client = ReplaceCsvLoader().CreateClient();
        SetupFakeCsv();
        var req = new AverageGwpRequestParams("Country1", new[] { "NotExists" });
        
        // Act
        var res = await CallApi(client, req);
        
        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await res.Content.ReadAsStringAsync();

        body.Should().Be("Line Of Business 'NotExists' does not exist.");
    }
    
    private WebApplicationFactory<Program> ReplaceCsvLoader()
    {
        var factory = _factory.WithWebHostBuilder((builder =>
        {
            builder.ConfigureServices(s => { s.AddSingleton<IGwpByCountryCsvLoader>(_csvLoaderSubstitute); });
        }));

        return factory;
    }

    private ConfiguredCall SetupFakeCsv()
    {
        return _csvLoaderSubstitute.LoadAsync().Returns(new List<GwpByCountryDataModel>()
        {
            new GwpByCountryDataModel()
            {
                Country = "Country1",
                LineOfBusiness = "Lob1",
                Y2008 = 1,
                Y2009 = 2,
                Y2010 = 3
            }
        });
    }
}