using System.Text.Json;
using System.Text.Json.Serialization;
using DragonEnglishKids.Api.Configuration;
using DragonEnglishKids.Api.Data;
using DragonEnglishKids.Api.Health;
using DragonEnglishKids.Api.Privacy;
using DragonEnglishKids.Api.Privacy.Redaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateBuilder(args);
var connectionString = DatabaseConfiguration.GetRequiredConnectionString(builder.Configuration);
var privacyOptions = PrivacyConfiguration.Bind(builder.Configuration, builder.Environment);

builder.Logging.AddConsole(options => options.FormatterName = RedactingConsoleFormatter.FormatterName);
builder.Logging.AddConsoleFormatter<RedactingConsoleFormatter, ConsoleFormatterOptions>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IDatabaseReadinessProbe, PostgresReadinessProbe>();
builder.Services.AddPrivacy(privacyOptions);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);
builder.Services.AddOpenApi("v1");

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
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

app.MapPrivacyEndpoints();
app.MapOpenApi("/openapi/{documentName}.json");

app.Run();

public partial class Program;