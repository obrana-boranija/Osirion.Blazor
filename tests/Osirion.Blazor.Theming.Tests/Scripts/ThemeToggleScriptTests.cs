using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Scripts;

/// <summary>
/// Reads the shipped theme-toggle script, which has no script test runner, and checks the cookie it writes.
/// </summary>
public class ThemeToggleScriptTests
{
    // Fails before the fix: the script wrote osirion-preferred-theme without Secure on HTTPS, while ThemeService and
    // theme-picker.js add it, so the same cookie was Secure or not depending on which code wrote it last.
    [Fact]
    public void ThemeToggleScript_ThemeCookie_IsSecureOverHttps()
    {
        var script = File.ReadAllText(ScriptPath("theme-toggle.js"));

        script.ShouldContain("window.location.protocol === 'https:' ? '; Secure' : ''");
        var write = script.Split('\n').Single(line => line.Contains("document.cookie = `osirion-preferred-theme="));
        write.ShouldContain("${secure}");
        write.ShouldContain("SameSite=Lax");
    }

    private static string ScriptPath(string file)
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return Path.Combine(directory.FullName, "src", "Osirion.Blazor.Theming", "wwwroot", "js", file);
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    }
}
