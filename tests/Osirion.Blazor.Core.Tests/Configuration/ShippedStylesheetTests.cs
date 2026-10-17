using Shouldly;

namespace Osirion.Blazor.Core.Tests.Configuration;

/// <summary>Checks rules in the stylesheets the package ships, which no component test renders.</summary>
public class ShippedStylesheetTests
{
    private const string FrameworkNoneBody =
        ":where(html[data-osirion-framework=None]) body{background-color:var(--osirion-background-primary,#fff);color:var(--osirion-text-primary,#111827)}";

    // Fails before the fix: no shipped stylesheet painted the body, so a site without a CSS framework kept the browser's
    // white page in the dark theme and drew light text on it.
    [Theory]
    [InlineData("index.min.css")]
    [InlineData("base/base-styles.min.css")]
    public void MinifiedStylesheet_FrameworkNone_PaintsTheBodyInTheThemeColours(string file)
    {
        // Act
        var css = File.ReadAllText(CoreCss(file));

        // Assert
        css.ShouldContain(FrameworkNoneBody);
    }

    [Fact]
    public void BodyRule_IsScopedToFrameworkNoneAtZeroSpecificity()
    {
        // Act
        var css = File.ReadAllText(CoreCss("index.min.css"));

        // Assert: a framework paints its own body, and any rule a site writes for body must win
        css.ShouldNotContain("}body{background", Case.Sensitive);
        css.ShouldContain(":where(html[data-osirion-framework=None]) body{");
    }

    private static string CoreCss(string file)
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return Path.Combine(directory.FullName, "src", "Osirion.Blazor.Core", "wwwroot", "css", file);
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    }
}
