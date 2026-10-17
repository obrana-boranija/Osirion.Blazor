using Microsoft.Extensions.Configuration;
using Osirion.Blazor.Cms.Domain.Options;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.DependencyInjection;

public sealed class SupportedLocalesBindingTests
{
    private static IConfigurationSection Section(params string[] locales)
    {
        var values = locales
            .Select((locale, index) => KeyValuePair.Create($"Provider:SupportedLocales:{index}", (string?)locale))
            .ToList();
        values.Add(KeyValuePair.Create("Provider:DefaultLocale", (string?)"en"));

        return new ConfigurationBuilder().AddInMemoryCollection(values).Build().GetSection("Provider");
    }

    [Fact]
    public void Bind_ConfiguredLocales_ReplaceTheDefault()
    {
        // Fails before the fix: binding appended to the default list, so ["en", "sr"] became en, en, sr.
        var options = new FileSystemOptions();

        Section("en", "sr").Bind(options);

        options.SupportedLocales.ShouldBe(["en", "sr"]);
        options.GetEffectiveSupportedLocales().ShouldBe(["en", "sr"]);
    }

    [Fact]
    public void Bind_ConfiguredLocalesWithoutEnglish_AreNotMixedWithTheDefault()
    {
        var options = new GitHubOptions();

        Section("sr", "de").Bind(options);

        options.SupportedLocales.ShouldBe(["sr", "de"]);
        options.GetEffectiveSupportedLocales().ShouldBe(["sr", "de"]);
    }

    [Fact]
    public void Bind_NothingConfigured_KeepsEnglish()
    {
        var options = new FileSystemOptions();

        new ConfigurationBuilder().Build().GetSection("Provider").Bind(options);

        options.GetEffectiveSupportedLocales().ShouldBe(["en"]);
    }

    [Fact]
    public void GetEffectiveSupportedLocales_AssignedEmptyList_StaysEmpty()
    {
        var options = new FileSystemOptions { SupportedLocales = [] };

        options.GetEffectiveSupportedLocales().ShouldBeEmpty();
    }

    [Fact]
    public void GetEffectiveSupportedLocales_LocaleAddedToTheDefault_IsTheWholeList()
    {
        var options = new FileSystemOptions();

        options.SupportedLocales.Add("de");

        options.GetEffectiveSupportedLocales().ShouldBe(["de"]);
    }
}
