using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Services;

public sealed class CssFrameworkDetectorPriorityTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), "osirion-css-" + Guid.NewGuid().ToString("N"));

    public CssFrameworkDetectorPriorityTests()
    {
        Directory.CreateDirectory(_tempDir);
    }

    [Fact]
    public async Task DetectFrameworkAsync_MissingDirectory_ReturnsNone()
    {
        var detector = new CssFrameworkDetector(Path.Combine(_tempDir, "missing"));

        var framework = await detector.DetectFrameworkAsync();

        framework.ShouldBe(CssFramework.None);
    }

    [Fact]
    public async Task DetectFrameworkAsync_SeveralFrameworks_PrefersBootstrap()
    {
        await WriteCss("radzen.css", ".x{}");
        await WriteCss("theme.css", "--bs-primary: red;");
        var detector = new CssFrameworkDetector(_tempDir);

        var framework = await detector.DetectFrameworkAsync();

        framework.ShouldBe(CssFramework.Bootstrap);
    }

    [Fact]
    public async Task DetectFrameworkAsync_MudBlazorAndFluent_PrefersMudBlazor()
    {
        await WriteCss("fluent.css", ".x{}");
        await WriteCss("mudblazor.css", ".x{}");
        var detector = new CssFrameworkDetector(_tempDir);

        var framework = await detector.DetectFrameworkAsync();

        framework.ShouldBe(CssFramework.MudBlazor);
    }

    [Fact]
    public async Task DetectFrameworkAsync_FluentAndRadzen_PrefersFluentUI()
    {
        await WriteCss("a.css", ".ms-Button{}");
        await WriteCss("b.css", ".rz-grid{}");
        var detector = new CssFrameworkDetector(_tempDir);

        var framework = await detector.DetectFrameworkAsync();

        framework.ShouldBe(CssFramework.FluentUI);
    }

    [Fact]
    public async Task DetectFrameworkAsync_OnlyRadzen_ReturnsRadzen()
    {
        await WriteCss("site.css", ".rz-grid{}");
        var detector = new CssFrameworkDetector(_tempDir);

        var framework = await detector.DetectFrameworkAsync();

        framework.ShouldBe(CssFramework.Radzen);
    }

    [Fact]
    public async Task DetectFrameworkAsync_NoKnownFramework_ReturnsNone()
    {
        await WriteCss("site.css", ".plain{}");
        var detector = new CssFrameworkDetector(_tempDir);

        var framework = await detector.DetectFrameworkAsync();

        framework.ShouldBe(CssFramework.None);
    }

    [Fact]
    public async Task GetDetectedFrameworksAsync_SameFrameworkInTwoFiles_ListsItOnce()
    {
        await WriteCss("bootstrap.css", ".x{}");
        await WriteCss(Path.Combine("lib", "other.css"), "/* Bootstrap v5.3 */");
        var detector = new CssFrameworkDetector(_tempDir);

        var frameworks = await detector.GetDetectedFrameworksAsync();

        frameworks.ShouldBe([CssFramework.Bootstrap]);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    private async Task WriteCss(string relativePath, string content)
    {
        var path = Path.Combine(_tempDir, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(path, content);
    }
}
