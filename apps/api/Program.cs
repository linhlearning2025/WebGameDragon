using System.Text.Json;
using System.Text.Json.Serialization;
using DragonEnglishKids.Api.Configuration;
using DragonEnglishKids.Api.Data;
using DragonEnglishKids.Api.Health;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = DatabaseConfiguration.GetRequiredConnectionString(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IDatabaseReadinessProbe, PostgresReadinessProbe>();
builder.Services.AddOpenApi("v1");

var app = builder.Build();
var healthJsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

app.MapGet("/health/live", () =>
{
    var result = HealthResponses.Liveness();
    return Results.Json(result.Body, healthJsonOptions, statusCode: result.StatusCode);
});

app.MapGet("/health/ready", async (
    IDatabaseReadinessProbe databaseProbe,
    CancellationToken cancellationToken) =>
{
    var result = await HealthResponses.ReadinessAsync(databaseProbe, cancellationToken);
    return Results.Json(result.Body, healthJsonOptions, statusCode: result.StatusCode);
});

app.MapOpenApi("/openapi/{documentName}.json");

app.Run();

public partial class Program;