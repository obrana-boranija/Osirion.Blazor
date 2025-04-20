using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components.GitHubCms;
using Osirion.Blazor.Options;
using Shouldly;

namespace Osirion.Blazor.Tests.Components.GitHubCms;

public class OsirionStylesTests : TestContext
{
    [Fact]
    public void OsirionStyles_ShouldRenderLinkTag_WhenStylesEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GitHubCmsOptions
        {
            UseStyles = true
        });
        Services.AddSingleton(options);

        // Act
        var cut = RenderComponent<OsirionStyles>();

        // Assert
        cut.Markup.ShouldContain("<link");
        cut.Markup.ShouldContain("href=\"_content/Osirion.Blazor/css/osirion-cms.css\"");
    }

    [Fact]
    public void OsirionStyles_ShouldNotRenderLinkTag_WhenStylesDisabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GitHubCmsOptions
        {
            UseStyles = false
        });
        Services.AddSingleton<IOptions<GitHubCmsOptions>>(options);

        // Act
        var cut = RenderComponent<OsirionStyles>();

        // Assert
        cut.Markup.ShouldNotContain("<link");
    }

    [Fact]
    public void OsirionStyles_ShouldOverrideGlobalOption_WhenParameterProvided()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GitHubCmsOptions
        {
            UseStyles = true
        });
        Services.AddSingleton<IOptions<GitHubCmsOptions>>(options);

        // Act
        var cut = RenderComponent<OsirionStyles>(parameters => parameters
            .Add(p => p.UseStyles, false)
        );

        // Assert
        cut.Markup.ShouldNotContain("<link");
    }

    [Fact]
    public void OsirionStyles_ShouldRenderCustomVariables_WhenProvided()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GitHubCmsOptions());
        Services.AddSingleton<IOptions<GitHubCmsOptions>>(options);

        const string customVars = "--osirion-primary-color: #ff0000;";

        // Act
        var cut = RenderComponent<OsirionStyles>(parameters => parameters
            .Add(p => p.CustomVariables, customVars)
        );

        // Assert
        cut.Markup.ShouldContain("<style>");
        cut.Markup.ShouldContain(":root");
        cut.Markup.ShouldContain(customVars);
    }

    [Fact]
    public void OsirionStyles_ShouldRenderBothStylesheetAndVariables_WhenBothEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GitHubCmsOptions
        {
            UseStyles = true
        });
        Services.AddSingleton<IOptions<GitHubCmsOptions>>(options);

        const string customVars = "--osirion-primary-color: #ff0000;";

        // Act
        var cut = RenderComponent<OsirionStyles>(parameters => parameters
            .Add(p => p.CustomVariables, customVars)
        );

        // Assert
        cut.Markup.ShouldContain("<link");
        cut.Markup.ShouldContain("<style>");
        cut.Markup.ShouldContain(customVars);
    }

    [Fact]
    public void OsirionStyles_ShouldUseOptionsCustomVariables_WhenNoParameterProvided()
    {
        // Arrange
        const string optionsCustomVars = "--osirion-primary-color: #00ff00;";
        var options = Microsoft.Extensions.Options.Options.Create(new GitHubCmsOptions
        {
            CustomVariables = optionsCustomVars
        });
        Services.AddSingleton<IOptions<GitHubCmsOptions>>(options);

        // Act
        var cut = RenderComponent<OsirionStyles>();

        // Assert
        cut.Markup.ShouldContain("<style>");
        cut.Markup.ShouldContain(optionsCustomVars);
    }
}