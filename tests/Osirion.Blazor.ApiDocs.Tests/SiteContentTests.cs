using Shouldly;

namespace Osirion.Blazor.ApiDocs.Tests;

/// <summary>
/// The site's parameter tables are generated from the components. This fails when a component gains, loses or
/// renames a parameter, or its documentation changes, until the tables are regenerated:
/// <c>dotnet run --project tools/Osirion.Blazor.ApiDocs -- content</c>.
/// </summary>
public class SiteContentTests
{
    private static readonly Lazy<Dictionary<string, Type>> Components = new(() => ComponentCatalog.Load(AppContext.BaseDirectory));

    public static TheoryData<string> ContentFiles()
    {
        var data = new TheoryData<string>();
        foreach (var file in Directory.EnumerateFiles(ContentRoot(), "*.md", SearchOption.AllDirectories).Order(StringComparer.Ordinal))
        {
            if (File.ReadAllText(file).Contains("osirion:parameters", StringComparison.Ordinal))
            {
                data.Add(Path.GetRelativePath(ContentRoot(), file));
            }
        }

        return data;
    }

    [Fact]
    public void Catalog_FindsTheLibraryComponents()
    {
        Components.Value.Keys.ShouldContain("OsirionFeatureCard");
        Components.Value.Keys.ShouldContain("ContentList");
        Components.Value.Count.ShouldBeGreaterThan(100);
    }

    [Theory]
    [MemberData(nameof(ContentFiles))]
    public void ParameterTables_MatchTheComponents(string file)
    {
        // Arrange
        var tables = new ParameterTable();
        var text = File.ReadAllText(Path.Combine(ContentRoot(), file));

        // Act
        var updated = ContentUpdater.Update(
            text,
            name => Components.Value.TryGetValue(name, out var type) ? ParameterTable.ToMarkdown(tables.GetRows(type)) : null,
            out var unknown);

        // Assert
        unknown.ShouldBeEmpty();
        updated.ShouldBe(text, "regenerate with: dotnet run --project tools/Osirion.Blazor.ApiDocs -- content");
    }

    private static string ContentRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return Path.Combine(directory.FullName, "content");
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    }
}
