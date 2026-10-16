using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Extensions;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Internal;

public class ThemingBuilderTests
{
    [Fact]
    public void ThemingBuilder_FluentCalls_ApplyEveryOption()
    {
        var services = new ServiceCollection();

        services.AddOsirionTheming(theming => theming
            .UseFramework(CssFramework.FluentUI)
            .EnableDarkMode(false)
            .UseSystemPreference()
            .WithCustomVariables("--accent: teal;"));

        var options = Resolve(services);
        options.Framework.ShouldBe(CssFramework.FluentUI);
        options.EnableDarkMode.ShouldBeFalse();
        options.FollowSystemPreference.ShouldBeTrue();
        options.CustomVariables.ShouldBe("--accent: teal;");
    }

    [Fact]
    public void ThemingBuilder_Configure_AppliesTheCallback()
    {
        var services = new ServiceCollection();

        services.AddOsirionTheming(theming => theming.Configure(options => options.DefaultMode = ThemeMode.Dark));

        Resolve(services).DefaultMode.ShouldBe(ThemeMode.Dark);
    }

    [Fact]
    public void ThemingBuilder_ConfigureNull_Throws()
    {
        var services = new ServiceCollection();

        var exception = Should.Throw<ArgumentNullException>(
            () => services.AddOsirionTheming(theming => theming.Configure(null!)));

        exception.ParamName.ShouldBe("configure");
    }

    [Fact]
    public void ThemingBuilder_Services_IsTheCollectionBeingConfigured()
    {
        var services = new ServiceCollection();
        IServiceCollection? seen = null;

        services.AddOsirionTheming(theming => seen = theming.Services);

        seen.ShouldBeSameAs(services);
    }

    [Fact]
    public void AddOsirionAutoTheming_RegistersDetectorAndAutoThemeService()
    {
        var services = new ServiceCollection();

        var returned = Osirion.Blazor.Theming.ThemingServiceCollectionExtensions.AddOsirionAutoTheming(services);

        returned.ShouldBeSameAs(services);
        services.ShouldContain(d => d.ServiceType == typeof(ICssFrameworkDetector)
            && d.ImplementationType == typeof(CssFrameworkDetector)
            && d.Lifetime == ServiceLifetime.Singleton);
        services.ShouldContain(d => d.ServiceType == typeof(IThemeService)
            && d.ImplementationType == typeof(AutoThemeService)
            && d.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void ThemingBuilder_PaletteCalls_SetTheDefaultPaletteAndAllowSelection()
    {
        var services = new ServiceCollection();

        services.AddOsirionTheming(theming => theming.UsePalette(ThemePalette.Editor).AllowPaletteSelection());

        var options = Resolve(services);
        options.DefaultPalette.ShouldBe(ThemePalette.Editor);
        options.AllowPaletteSelection.ShouldBeTrue();
    }

    [Fact]
    public void ThemingBuilder_Configuration_BindsThePaletteOptions()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{ThemingOptions.Section}:DefaultPalette"] = "StudioBlue",
                [$"{ThemingOptions.Section}:AllowPaletteSelection"] = "true",
            })
            .Build();
        var services = new ServiceCollection();

        services.AddOsirionTheming(configuration);

        var options = Resolve(services);
        options.DefaultPalette.ShouldBe(ThemePalette.StudioBlue);
        options.AllowPaletteSelection.ShouldBeTrue();
    }

    private static ThemingOptions Resolve(IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IOptions<ThemingOptions>>().Value;
    }
}
