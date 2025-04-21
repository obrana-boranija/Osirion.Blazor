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
        var options = Microsoft.Extensions.Options.Options.Create(new OsirionStyleOptions
        {
            UseStyles = true
        });
        Services.AddSingleton(options);

        // Act
        var cut = RenderComponent<OsirionStyles>();

        // Assert
        cut.Markup.ShouldContain("<link");
        cut.Markup.ShouldContain("href=\"_content/Osirion.Blazor/css/osirion.css\"");
    }

    [Fact]
    public void OsirionStyles_ShouldNotRenderLinkTag_WhenStylesDisabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new OsirionStyleOptions
        {
            UseStyles = false
        });
        Services.AddSingleton<IOptions<OsirionStyleOptions>>(options);

        // Act
        var cut = RenderComponent<OsirionStyles>();

        // Assert
        cut.Markup.ShouldNotContain("<link");
    }

    [Fact]
    public void OsirionStyles_ShouldOverrideGlobalOption_WhenParameterProvided()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new OsirionStyleOptions
        {
            UseStyles = true
        });
        Services.AddSingleton<IOptions<OsirionStyleOptions>>(options);

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
        var options = Microsoft.Extensions.Options.Options.Create(new OsirionStyleOptions());
        Services.AddSingleton<IOptions<OsirionStyleOptions>>(options);

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
        var options = Microsoft.Extensions.Options.Options.Create(new OsirionStyleOptions
        {
            UseStyles = true
        });
        Services.AddSingleton<IOptions<OsirionStyleOptions>>(options);

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
        var options = Microsoft.Extensions.Options.Options.Create(new OsirionStyleOptions
        {
            CustomVariables = optionsCustomVars
        });
        Services.AddSingleton<IOptions<OsirionStyleOptions>>(options);

        // Act
        var cut = RenderComponent<OsirionStyles>();

        // Assert
        cut.Markup.ShouldContain("<style>");
        cut.Markup.ShouldContain(optionsCustomVars);
    }

    [Fact]
    public void OsirionStyles_ShouldRenderFrameworkIntegrationScript_WhenFrameworkSpecified()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new OsirionStyleOptions());
        Services.AddSingleton<IOptions<OsirionStyleOptions>>(options);

        // Act
        var cut = RenderComponent<OsirionStyles>(parameters => parameters
            .Add(p => p.FrameworkIntegration, CssFramework.Bootstrap)
        );

        // Assert
        cut.Markup.ShouldContain("<script>");
        cut.Markup.ShouldContain("osirion-bootstrap-integration");
        cut.Markup.ShouldContain("applyFrameworkIntegration");
    }

    [Fact]
    public void OsirionStyles_ShouldUseFrameworkFromOptions_WhenNoParameterProvided()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new OsirionStyleOptions
        {
            FrameworkIntegration = CssFramework.Tailwind
        });
        Services.AddSingleton<IOptions<OsirionStyleOptions>>(options);

        // Act
        var cut = RenderComponent<OsirionStyles>();

        // Assert
        cut.Markup.ShouldContain("<script>");
        cut.Markup.ShouldContain("osirion-tailwind-integration");
    }

    [Fact]
    public void OsirionStyles_ShouldOverrideOptionsFramework_WhenParameterProvided()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new OsirionStyleOptions
        {
            FrameworkIntegration = CssFramework.Tailwind
        });
        Services.AddSingleton<IOptions<OsirionStyleOptions>>(options);

        // Act
        var cut = RenderComponent<OsirionStyles>(parameters => parameters
            .Add(p => p.FrameworkIntegration, CssFramework.MudBlazor)
        );

        // Assert
        cut.Markup.ShouldContain("osirion-mudblazor-integration");
        cut.Markup.ShouldNotContain("osirion-tailwind-integration");
    }

    [Fact]
    public void OsirionStyles_ShouldNotRenderFrameworkScript_WhenFrameworkIsNone()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new OsirionStyleOptions
        {
            FrameworkIntegration = CssFramework.None
        });
        Services.AddSingleton<IOptions<OsirionStyleOptions>>(options);

        // Act
        var cut = RenderComponent<OsirionStyles>();

        // Assert
        cut.Markup.ShouldNotContain("applyFrameworkIntegration");
    }
}