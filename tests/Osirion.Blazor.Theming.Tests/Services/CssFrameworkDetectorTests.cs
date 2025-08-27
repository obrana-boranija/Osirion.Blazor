using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Services;

namespace Osirion.Blazor.Theming.Tests.Services;

public class CssFrameworkDetectorTests : IDisposable
{
    private readonly string _tempDir;

    public CssFrameworkDetectorTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
    }

    [Fact]
    public async Task Detects_Bootstrap_By_FileName_And_Content()
    {
        var css = ".btn { color: red; } /* Bootstrap v5 */";
        var file = Path.Combine(_tempDir, "bootstrap.min.css");
        await File.WriteAllTextAsync(file, css);
        var detector = new CssFrameworkDetector(_tempDir);
        var detected = await detector.GetDetectedFrameworksAsync();
        Assert.Contains(CssFramework.Bootstrap, detected);
    }

    [Fact]
    public async Task Detects_MudBlazor_By_Class()
    {
        var css = ".mud-button { color: blue; }";
        var file = Path.Combine(_tempDir, "mudblazor.css");
        await File.WriteAllTextAsync(file, css);
        var detector = new CssFrameworkDetector(_tempDir);
        var detected = await detector.GetDetectedFrameworksAsync();
        Assert.Contains(CssFramework.MudBlazor, detected);
    }

    [Fact]
    public async Task Detects_FluentUI_By_Class()
    {
        var css = ".ms-Button { color: green; } /* Fluent UI */";
        var file = Path.Combine(_tempDir, "fluentui.css");
        await File.WriteAllTextAsync(file, css);
        var detector = new CssFrameworkDetector(_tempDir);
        var detected = await detector.GetDetectedFrameworksAsync();
        Assert.Contains(CssFramework.FluentUI, detected);
    }

    [Fact]
    public async Task Detects_Radzen_By_Class()
    {
        var css = ".rz-button { color: purple; }";
        var file = Path.Combine(_tempDir, "radzen.css");
        await File.WriteAllTextAsync(file, css);
        var detector = new CssFrameworkDetector(_tempDir);
        var detected = await detector.GetDetectedFrameworksAsync();
        Assert.Contains(CssFramework.Radzen, detected);
    }

    [Fact]
    public async Task Returns_None_If_No_Framework()
    {
        var css = ".custom-class { color: black; }";
        var file = Path.Combine(_tempDir, "custom.css");
        await File.WriteAllTextAsync(file, css);
        var detector = new CssFrameworkDetector(_tempDir);
        var detected = await detector.GetDetectedFrameworksAsync();
        Assert.DoesNotContain(CssFramework.Bootstrap, detected);
        Assert.DoesNotContain(CssFramework.MudBlazor, detected);
        Assert.DoesNotContain(CssFramework.FluentUI, detected);
        Assert.DoesNotContain(CssFramework.Radzen, detected);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }
}
