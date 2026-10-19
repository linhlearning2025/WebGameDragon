using System.Text.RegularExpressions;
using DragonEnglishKids.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class DataInventoryTests
{
    private static readonly Regex ForbiddenFieldNames = new(
        "birth|dob|address|realname|fullname|legalname|email|phone|audio|transcript",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static Dictionary<string, HashSet<string>> ReadInventory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "docs", "privacy", "data-inventory.yaml")))
        {
            directory = directory.Parent;
        }

        Assert.IsNotNull(directory, "docs/privacy/data-inventory.yaml was not found.");
        var tables = new Dictionary<string, HashSet<string>>();
        var inTables = false;
        var inFields = false;
        HashSet<string>? current = null;

        foreach (var line in File.ReadLines(Path.Combine(directory.FullName, "docs", "privacy", "data-inventory.yaml")))
        {
            if (line.StartsWith("tables:", StringComparison.Ordinal))
            {
                inTables = true;
                continue;
            }

            if (inTables && line.Length > 0 && line[0] != ' ' && line[0] != '#')
            {
                inTables = false;
            }

            if (!inTables)
            {
                continue;
            }

            if (line.StartsWith("  ", StringComparison.Ordinal) && !line.StartsWith("   ", StringComparison.Ordinal))
            {
                current = [];
                tables[line.Trim().TrimEnd(':')] = current;
                inFields = false;
            }
            else if (line.Trim() == "fields:")
            {
                inFields = true;
            }
            else if (inFields && line.StartsWith("      ", StringComparison.Ordinal) && !line.StartsWith("       ", StringComparison.Ordinal))
            {
                current?.Add(line.Trim().Split(':')[0]);
            }
        }

        return tables;
    }

    private static ApplicationDbContext CreateModelOnlyContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=model-only;Username=model;Password=model")
            .Options);

    [TestMethod]
    public void EveryPersistedTableAndFieldIsInTheDataInventory()
    {
        var inventory = ReadInventory();
        using var context = CreateModelOnlyContext();

        foreach (var entity in context.Model.GetEntityTypes())
        {
            var table = entity.GetTableName()!;
            Assert.IsTrue(inventory.ContainsKey(table), $"Table {table} is missing from the inventory.");
            foreach (var property in entity.GetProperties())
            {
                Assert.IsTrue(
                    inventory[table].Contains(property.Name),
                    $"Field {table}.{property.Name} is missing from the inventory.");
            }
        }
    }

    [TestMethod]
    public void InventoryListsNoFieldThatIsNotPersisted()
    {
        var inventory = ReadInventory();
        using var context = CreateModelOnlyContext();
        var persisted = context.Model.GetEntityTypes()
            .ToDictionary(x => x.GetTableName()!, x => x.GetProperties().Select(p => p.Name).ToHashSet());

        foreach (var (table, fields) in inventory)
        {
            Assert.IsTrue(persisted.ContainsKey(table), $"Inventory table {table} is not persisted.");
            Assert.IsTrue(fields.SetEquals(persisted[table]), $"Inventory fields for {table} do not match the model.");
        }
    }

    [TestMethod]
    public void NoPreciseBirthDateAddressRealNameAudioOrTranscriptFieldIsPersisted()
    {
        using var context = CreateModelOnlyContext();

        var names = context.Model.GetEntityTypes()
            .SelectMany(entity => entity.GetProperties().Select(property => $"{entity.GetTableName()}.{property.Name}"))
            .Where(name => ForbiddenFieldNames.IsMatch(name.Split('.')[1]))
            .ToList();

        Assert.AreEqual(0, names.Count, string.Join(", ", names));
    }
}
