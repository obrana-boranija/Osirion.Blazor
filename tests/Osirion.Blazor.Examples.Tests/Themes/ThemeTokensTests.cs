using System.Text.RegularExpressions;
using Osirion.Blazor.Examples.Shared.Themes;
using Shouldly;

namespace Osirion.Blazor.Examples.Tests.Themes;

/// <summary>The theme page lists tokens by name, so a token the themes rename or drop must fail here, not show blank.</summary>
public partial class ThemeTokensTests
{
    public static TheoryData<string> Tokens()
    {
        var data = new TheoryData<string>();
        foreach (var token in ThemeTokens.Groups.SelectMany(group => group.Tokens))
        {
            data.Add(token.Name);
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(Tokens))]
    public void ListedToken_IsDeclaredByBothThemes(string token)
    {
        // Arrange
        var css = File.ReadAllText(ThemingStylesheet());

        // Act and assert
        foreach (var mode in new[] { "light", "dark" })
        {
            ThemeBlock(css, mode).ShouldContain($"--osirion-{token}:", customMessage: $"the {mode} theme");
        }
    }

    [Fact]
    public void Tokens_AreListedOnce()
    {
        ThemeTokens.Groups.SelectMany(group => group.Tokens).Select(token => token.Name).ShouldBeUnique();
    }

    private static string ThemeBlock(string css, string mode)
    {
        var rule = Rule().Matches(css)
            .First(match => match.Groups["selector"].Value.Contains($":root[data-osirion-theme={mode}]", StringComparison.Ordinal)
                && match.Groups["body"].Value.Contains("--osirion-text-primary:", StringComparison.Ordinal));
        return rule.Groups["body"].Value;
    }

    private static string ThemingStylesheet()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return Path.Combine(directory.FullName, "src", "Osirion.Blazor.Theming", "wwwroot", "css", "index.min.css");
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    }

    [GeneratedRegex(@"(?<selector>[^{}]+)\{(?<body>[^{}]*)\}")]
    private static partial Regex Rule();
}
