using System.Text.Json;
using System.Xml.Linq;
using Shouldly;

namespace Osirion.Blazor.Templates.Tests;

public class TemplatePackageTests
{
    private static readonly string Root = FindRoot();
    private static readonly string TemplatesFolder = Path.Combine(Root, "src", "Osirion.Blazor.Templates", "templates");

    public static TheoryData<string> Templates()
    {
        var data = new TheoryData<string>();
        foreach (var folder in Directory.GetDirectories(TemplatesFolder))
        {
            data.Add(Path.GetFileName(folder));
        }

        return data;
    }

    [Fact]
    public void Package_HasTheSameVersionAsTheLibrary()
    {
        // Act
        var package = Version(Path.Combine(Root, "src", "Osirion.Blazor.Templates", "Osirion.Blazor.Templates.csproj"));

        // Assert
        package.ShouldBe(LibraryVersion());
    }

    // A template that referenced an older library would create sites without the fixes released with it; a newer one
    // would not restore until that version is published.
    [Theory]
    [MemberData(nameof(Templates))]
    public void Template_ReferencesTheLibraryVersionItIsReleasedWith(string template)
    {
        // Arrange
        var project = XDocument.Load(ProjectFile(template));

        // Act
        var reference = project.Descendants("PackageReference").Single(e => (string?)e.Attribute("Include") == "Osirion.Blazor");

        // Assert
        ((string?)reference.Attribute("Version")).ShouldBe(LibraryVersion());
    }

    [Theory]
    [MemberData(nameof(Templates))]
    public void Template_ShortNameIsTheFolderName_AndItsProjectIsTheSourceName(string template)
    {
        // Act
        var config = TemplateConfig(template);

        // Assert
        config.GetProperty("shortName").GetString().ShouldBe(template);
        config.GetProperty("identity").GetString().ShouldStartWith("Osirion.Blazor.Templates.");
        var sourceName = config.GetProperty("sourceName").GetString();
        Path.GetFileNameWithoutExtension(ProjectFile(template)).ShouldBe(sourceName);
        config.GetProperty("primaryOutputs")[0].GetProperty("path").GetString().ShouldBe(sourceName + ".csproj");
    }

    // Fails before the fix: with sourceName "OsirionBlog" every form of the name was the same, so dotnet new wrote the
    // namespace form into the stylesheet link. A project called My.Blog-Site linked My.Blog_Site.styles.css while the
    // bundle is My.Blog-Site.styles.css, and the site rendered without the components' styles.
    [Theory]
    [MemberData(nameof(Templates))]
    public void Template_SourceNameHasAHyphen_SoTheStylesheetLinkKeepsTheProjectName(string template)
    {
        // Arrange
        var sourceName = TemplateConfig(template).GetProperty("sourceName").GetString()!;
        var app = File.ReadAllText(Path.Combine(TemplatesFolder, template, "Components", "App.razor"));

        // Assert: the raw form differs from the namespace form, and the link uses the raw form
        sourceName.ShouldContain('-');
        app.ShouldContain($"@Assets[\"{sourceName}.styles.css\"]");
    }

    [Fact]
    public void Templates_HaveDistinctIdentities()
    {
        // Act
        var identities = Directory.GetDirectories(TemplatesFolder)
            .Select(folder => TemplateConfig(Path.GetFileName(folder)).GetProperty("identity").GetString())
            .ToList();

        // Assert
        identities.ShouldBeUnique();
    }

    [Theory]
    [MemberData(nameof(Templates))]
    public void Template_HasNoBuildOutputOrLocalState(string template)
    {
        // Act: only what dotnet new should copy belongs in the package
        var stray = Directory.EnumerateFileSystemEntries(Path.Combine(TemplatesFolder, template), "*", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(Path.Combine(TemplatesFolder, template), path).Replace('\\', '/'))
            .Where(path => path.StartsWith("bin", StringComparison.Ordinal) || path.StartsWith("obj", StringComparison.Ordinal)
                || path.StartsWith(".vs", StringComparison.Ordinal) || path.EndsWith(".user", StringComparison.Ordinal))
            .ToList();

        // Assert
        stray.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(Templates))]
    public void Template_ParameterDefaults_AppearInTheFilesTheyReplace(string template)
    {
        // Arrange: a parameter whose default is in no file would silently do nothing
        var config = TemplateConfig(template);
        var files = Directory.EnumerateFiles(Path.Combine(TemplatesFolder, template), "*", SearchOption.AllDirectories)
            .Where(path => !path.Contains(".template.config", StringComparison.Ordinal))
            .Select(File.ReadAllText)
            .ToList();

        // Act
        var unused = config.GetProperty("symbols").EnumerateObject()
            .Where(symbol => symbol.Value.TryGetProperty("replaces", out _))
            .Where(symbol => !files.Any(text => text.Contains(symbol.Value.GetProperty("replaces").GetString()!, StringComparison.Ordinal)))
            .Select(symbol => symbol.Name)
            .ToList();

        // Assert
        unused.ShouldBeEmpty();
    }

    private static string ProjectFile(string template) =>
        Directory.GetFiles(Path.Combine(TemplatesFolder, template), "*.csproj").Single();

    private static JsonElement TemplateConfig(string template) =>
        JsonDocument.Parse(File.ReadAllText(Path.Combine(TemplatesFolder, template, ".template.config", "template.json"))).RootElement;

    private static string LibraryVersion() => Version(Path.Combine(Root, "src", "Osirion.Blazor", "Osirion.Blazor.csproj"));

    private static string Version(string projectFile) =>
        XDocument.Load(projectFile).Descendants("Version").Single().Value;

    private static string FindRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return directory.FullName;
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    }
}
