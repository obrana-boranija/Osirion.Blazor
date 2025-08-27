using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Theming.Services;

public class CssFrameworkDetector : ICssFrameworkDetector
{
    private readonly string _wwwrootPath;

    public CssFrameworkDetector(string? wwwrootPath = null)
    {
        // Allow override for testing
        _wwwrootPath = wwwrootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
    }

    public async Task<CssFramework> DetectFrameworkAsync()
    {
        var detected = await GetDetectedFrameworksAsync();
        // Priority order
        foreach (var fw in new[] { CssFramework.Bootstrap, CssFramework.MudBlazor, CssFramework.FluentUI, CssFramework.Radzen })
            if (detected.Contains(fw)) return fw;
        return CssFramework.None;
    }

    public async Task<List<CssFramework>> GetDetectedFrameworksAsync()
    {
        var result = new List<CssFramework>();
        if (!Directory.Exists(_wwwrootPath)) return result;
        var cssFiles = Directory.GetFiles(_wwwrootPath, "*.css", SearchOption.AllDirectories);
        foreach (var file in cssFiles)
        {
            var content = await File.ReadAllTextAsync(file);
            if (IsBootstrap(file, content)) result.Add(CssFramework.Bootstrap);
            if (IsMudBlazor(file, content)) result.Add(CssFramework.MudBlazor);
            if (IsFluentUI(file, content)) result.Add(CssFramework.FluentUI);
            if (IsRadzen(file, content)) result.Add(CssFramework.Radzen);
        }
        return result.Distinct().ToList();
    }

    private static bool IsBootstrap(string file, string content)
    {
        return file.Contains("bootstrap", StringComparison.OrdinalIgnoreCase)
            || content.Contains("Bootstrap v", StringComparison.OrdinalIgnoreCase)
            || content.Contains(".btn", StringComparison.OrdinalIgnoreCase)
            || content.Contains("--bs-", StringComparison.OrdinalIgnoreCase);
    }
    private static bool IsMudBlazor(string file, string content)
    {
        return file.Contains("mudblazor", StringComparison.OrdinalIgnoreCase)
            || content.Contains("mud-", StringComparison.OrdinalIgnoreCase)
            || content.Contains(".mud-", StringComparison.OrdinalIgnoreCase);
    }
    private static bool IsFluentUI(string file, string content)
    {
        return file.Contains("fluent", StringComparison.OrdinalIgnoreCase)
            || content.Contains("ms-Button", StringComparison.OrdinalIgnoreCase)
            || content.Contains("Fluent UI", StringComparison.OrdinalIgnoreCase);
    }
    private static bool IsRadzen(string file, string content)
    {
        return file.Contains("radzen", StringComparison.OrdinalIgnoreCase)
            || content.Contains("rz-", StringComparison.OrdinalIgnoreCase)
            || content.Contains(".rz-", StringComparison.OrdinalIgnoreCase);
    }
}
