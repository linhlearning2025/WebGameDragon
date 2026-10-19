using System.Reflection;
using System.Text.RegularExpressions;
using DragonEnglishKids.Api.Privacy.Consent;
using DragonEnglishKids.Api.Privacy.Requests;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class PrivacyContractTests
{
    private static readonly string[] HttpMethods = ["get", "put", "post", "delete", "patch"];

    private static string[] ReadContractLines()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        const string relative = "specs/002-child-data-privacy/contracts/privacy.openapi.yaml";
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, relative)))
        {
            directory = directory.Parent;
        }

        Assert.IsNotNull(directory, $"{relative} was not found.");
        return File.ReadAllLines(Path.Combine(directory.FullName, relative));
    }

    private static Dictionary<string, HashSet<string>> ContractOperations(string[] lines)
    {
        var operations = new Dictionary<string, HashSet<string>>();
        string? path = null;
        string? operation = null;
        var inPaths = false;

        foreach (var line in lines)
        {
            if (line.StartsWith("paths:", StringComparison.Ordinal))
            {
                inPaths = true;
                continue;
            }

            if (inPaths && line.Length > 0 && line[0] != ' ')
            {
                inPaths = false;
            }

            if (!inPaths)
            {
                continue;
            }

            if (line.StartsWith("  /", StringComparison.Ordinal))
            {
                path = line.Trim().TrimEnd(':');
                operation = null;
            }
            else if (path is not null && line.StartsWith("    ", StringComparison.Ordinal) && !line.StartsWith("     ", StringComparison.Ordinal)
                     && HttpMethods.Contains(line.Trim().TrimEnd(':')))
            {
                operation = $"{line.Trim().TrimEnd(':').ToUpperInvariant()} {path}";
                operations[operation] = [];
            }
            else if (operation is not null && Regex.IsMatch(line, "^        \"\\d{3}\":"))
            {
                operations[operation].Add(line.Trim().Split('"')[1]);
            }
        }

        return operations;
    }

    private static Dictionary<string, HashSet<string>> ContractSchemas(string[] lines)
    {
        var schemas = new Dictionary<string, HashSet<string>>();
        string? schema = null;
        var inSchemas = false;
        var inProperties = false;

        foreach (var line in lines)
        {
            if (line.StartsWith("  schemas:", StringComparison.Ordinal))
            {
                inSchemas = true;
                continue;
            }

            if (!inSchemas)
            {
                continue;
            }

            if (line.StartsWith("    ", StringComparison.Ordinal) && !line.StartsWith("     ", StringComparison.Ordinal))
            {
                schema = line.Trim().TrimEnd(':');
                schemas[schema] = [];
                inProperties = false;
            }
            else if (line.Trim() == "properties:")
            {
                inProperties = true;
            }
            else if (schema is not null && inProperties && line.StartsWith("        ", StringComparison.Ordinal)
                     && !line.StartsWith("         ", StringComparison.Ordinal))
            {
                schemas[schema].Add(line.Trim().Split(':')[0]);
            }
        }

        return schemas;
    }

    private static string CamelCase(string name) => char.ToLowerInvariant(name[0]) + name[1..];

    [TestMethod]
    public void ImplementedEndpointsMatchTheContractExactly()
    {
        using var app = new PrivacyTestApp();
        var implemented = app.Services.GetRequiredService<EndpointDataSource>().Endpoints
            .OfType<RouteEndpoint>()
            .Where(endpoint => endpoint.RoutePattern.RawText?.StartsWith("/api/v1/", StringComparison.Ordinal) == true)
            .SelectMany(endpoint => endpoint.Metadata.GetMetadata<HttpMethodMetadata>()!.HttpMethods
                .Select(method => $"{method} {Regex.Replace(endpoint.RoutePattern.RawText!, @"\{(\w+):[^}]+\}", "{$1}")}"))
            .Order()
            .ToList();

        var contract = ContractOperations(ReadContractLines()).Keys.Order().ToList();

        CollectionAssert.AreEqual(contract, implemented);
    }

    [TestMethod]
    public void ContractDocumentsTheStatusCodesTheApiReturns()
    {
        var operations = ContractOperations(ReadContractLines());

        CollectionAssert.IsSubsetOf(
            new[] { "200", "403", "409" },
            operations["PUT /api/v1/children/{childId}/consents/{purpose}"].ToList());
        CollectionAssert.IsSubsetOf(
            new[] { "200", "403", "503" },
            operations["POST /api/v1/children/{childId}/speech-sessions"].ToList());
        CollectionAssert.IsSubsetOf(
            new[] { "200", "403" },
            operations["POST /api/v1/children/{childId}/speech-sessions/{speechSessionId}/heartbeat"].ToList());
        CollectionAssert.IsSubsetOf(
            new[] { "202", "403" },
            operations["POST /api/v1/children/{childId}/privacy-requests"].ToList());
        CollectionAssert.IsSubsetOf(
            new[] { "200", "403" },
            operations["GET /api/v1/privacy-requests/{requestId}"].ToList());
    }

    [TestMethod]
    public void ContractSchemasMatchTheResponseAndRequestModels()
    {
        var schemas = ContractSchemas(ReadContractLines());
        var models = new Dictionary<string, Type>
        {
            ["ConsentDecisionRequest"] = typeof(ConsentDecisionRequest),
            ["ConsentState"] = typeof(ConsentStateDto),
            ["ConsentHistory"] = typeof(ConsentHistoryDto),
            ["SpeechSessionGrant"] = typeof(SpeechSessionGrantDto),
            ["NoticeDocument"] = typeof(NoticeDocument),
            ["PrivacyRequestCreate"] = typeof(PrivacyRequestCreateDto),
            ["PrivacyRequestStatus"] = typeof(PrivacyRequestStatusDto)
        };

        foreach (var (schema, model) in models)
        {
            var expected = model.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(property => CamelCase(property.Name))
                .Order()
                .ToList();

            CollectionAssert.AreEqual(expected, schemas[schema].Order().ToList(), schema);
        }
    }
}
