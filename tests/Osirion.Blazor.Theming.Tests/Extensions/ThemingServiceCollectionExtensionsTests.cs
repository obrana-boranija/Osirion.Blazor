using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Extensions;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Extensions;

public class ThemingServiceCollectionExtensionsTests
{
    public static TheoryData<string> Registrations => new() { "default", "builder", "configuration" };

    [Theory]
    [MemberData(nameof(Registrations))]
    public void AddOsirionTheming_RegistersThemeServicePerScope(string registration)
    {
        var services = Register(registration);

        var descriptor = services.Last(d => d.ServiceType == typeof(IThemeService));

        descriptor.Lifetime.ShouldBe(ServiceLifetime.Scoped);
    }

    [Theory]
    [MemberData(nameof(Registrations))]
    public void AddOsirionTheming_ThemeChosenInOneScope_IsNotSeenByAnotherScope(string registration)
    {
        using var provider = Register(registration).BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });
        using var firstUser = provider.CreateScope();
        using var secondUser = provider.CreateScope();
        var firstTheme = firstUser.ServiceProvider.GetRequiredService<IThemeService>();
        var secondTheme = secondUser.ServiceProvider.GetRequiredService<IThemeService>();
        var secondUserModeBefore = secondTheme.CurrentMode;
        var secondUserNotifications = 0;
        secondTheme.ThemeChanged += (_, _) => secondUserNotifications++;

        firstTheme.SetThemeMode(ThemeMode.Dark);

        firstTheme.CurrentMode.ShouldBe(ThemeMode.Dark);
        secondTheme.CurrentMode.ShouldBe(secondUserModeBefore);
        secondTheme.CurrentMode.ShouldNotBe(ThemeMode.Dark);
        secondUserNotifications.ShouldBe(0);
    }

    private static ServiceCollection Register(string registration)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        switch (registration)
        {
            case "default":
                services.AddOsirionTheming();
                break;
            case "builder":
                services.AddOsirionTheming(builder => builder.EnableDarkMode(true));
                break;
            default:
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        [$"{ThemingOptions.Section}:DefaultMode"] = "Light",
                        [$"{ThemingOptions.Section}:EnableDarkMode"] = "true"
                    })
                    .Build();
                services.AddOsirionTheming(configuration);
                break;
        }

        return services;
    }
}
