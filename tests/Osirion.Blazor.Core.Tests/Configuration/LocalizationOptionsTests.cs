using Microsoft.Extensions.Configuration;
using Osirion.Blazor.Components;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Configuration;

public sealed class LocalizationOptionsTests
{
    private static IConfigurationSection Section(params string[] locales) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(locales.Select((locale, index) =>
                KeyValuePair.Create($"Localization:SupportedLocales:{index}", (string?)locale)))
            .Build()
            .GetSection("Localization");

    [Fact]
    public void Bind_ConfiguredLocales_ReplaceTheDefault()
    {
        // Fails before the fix: binding appended to the ["en"] default, so ["en", "sr"] became en, en, sr.
        var options = new LocalizationOptions();

        Section("en", "sr").Bind(options);

        options.SupportedLocales.ShouldBe(["en", "sr"]);
        options.GetEffectiveSupportedLocales().ShouldBe(["en", "sr"]);
    }

    [Fact]
    public void Bind_ConfiguredLocalesWithoutEnglish_AreNotMixedWithTheDefault()
    {
        var options = new LocalizationOptions();

        Section("sr", "de").Bind(options);

        options.SupportedLocales.ShouldBe(["sr", "de"]);
        options.GetEffectiveSupportedLocales().ShouldBe(["sr", "de"]);
    }

    [Fact]
    public void GetEffectiveSupportedLocales_NothingConfigured_IsEnglish()
    {
        new LocalizationOptions().GetEffectiveSupportedLocales().ShouldBe(["en"]);
    }

    [Fact]
    public void GetEffectiveSupportedLocales_AssignedEmptyList_StaysEmpty()
    {
        new LocalizationOptions { SupportedLocales = [] }.GetEffectiveSupportedLocales().ShouldBeEmpty();
    }
}
