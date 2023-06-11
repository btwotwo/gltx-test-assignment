using CountryGwpApi.CountryGwp.Data.Csv;
using CountryGwpApi.CountryGwp.Data.Repository;
using CountryGwpApi.CountryGwp.Services.Calculator;

const string RoutePrefix = "/server/api";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
AddAverageGwpCalculator(builder);

var app = builder.Build();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"{RoutePrefix}/swagger/v1/swagger.json", "GwpApi");
});
app.UsePathBase(RoutePrefix);
app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentname}/swagger.json";
});
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();

void AddAverageGwpCalculator(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.AddSingleton<IGwpByCountryCsvLoader, GwpByCountryCsvFileLoader>();
    webApplicationBuilder.Services.AddScoped<IGwpDataRepository, InMemoryGwpDataRepository>();
    webApplicationBuilder.Services.AddScoped<IAverageGwpCalculator, AverageGwpCalculator>();
    webApplicationBuilder.Services.Decorate<IAverageGwpCalculator, AverageGwpCalculatorCacheDecorator>();
    webApplicationBuilder.Services.AddMemoryCache();
}
