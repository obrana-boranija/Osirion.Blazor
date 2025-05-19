using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Shouldly;

namespace Osirion.Blazor.Cms.Tests.Options;

public class LocalizationOptionsTests
{
    [Fact]
    public void DefaultProperties_HaveExpectedValues()
    {
        // Arrange
        var options = new LocalizationOptions();

        // Assert
        options.SupportedCultures.ShouldNotBeNull();
        options.SupportedCultures.Count.ShouldBe(1);
        options.SupportedCultures.ShouldContain("en-US");
        options.DefaultCulture.ShouldBe("en-US");
        options.EnableCultureFallback.ShouldBeTrue();
        options.ShowCultureSelector.ShouldBeTrue();
        options.CultureDisplayFormat.ShouldBe("NativeName");
        options.RequireCulturePrefix.ShouldBeFalse();
    }

    [Fact]
    public void Properties_CanBeSet_AndRetrieved()
    {
        // Arrange
        var options = new LocalizationOptions
        {
            DefaultCulture = "fr-FR",
            EnableCultureFallback = false,
            ShowCultureSelector = false,
            CultureDisplayFormat = "EnglishName",
            RequireCulturePrefix = true,
            SupportedCultures = new List<string> { "en-US", "fr-FR", "de-DE" }
        };

        // Assert
        options.DefaultCulture.ShouldBe("fr-FR");
        options.EnableCultureFallback.ShouldBeFalse();
        options.ShowCultureSelector.ShouldBeFalse();
        options.CultureDisplayFormat.ShouldBe("EnglishName");
        options.RequireCulturePrefix.ShouldBeTrue();
        options.SupportedCultures.Count.ShouldBe(3);
        options.SupportedCultures.ShouldContain("en-US");
        options.SupportedCultures.ShouldContain("fr-FR");
        options.SupportedCultures.ShouldContain("de-DE");
    }

    [Fact]
    public void AddSupportedCultures_AddsCulturesToTheList()
    {
        // Arrange
        var options = new LocalizationOptions();

        // Act
        options.AddSupportedCultures("fr-FR", "de-DE");

        // Assert
        options.SupportedCultures.Count.ShouldBe(3); // Including default en-US
        options.SupportedCultures.ShouldContain("en-US");
        options.SupportedCultures.ShouldContain("fr-FR");
        options.SupportedCultures.ShouldContain("de-DE");
    }

    [Fact]
    public void AddSupportedCultures_SkipsDuplicates()
    {
        // Arrange
        var options = new LocalizationOptions();

        // Act
        options.AddSupportedCultures("en-US", "en-US", "fr-FR");

        // Assert
        options.SupportedCultures.Count.ShouldBe(2);
        options.SupportedCultures.ShouldContain("en-US");
        options.SupportedCultures.ShouldContain("fr-FR");
    }

    [Fact]
    public void SetDefaultCulture_UpdatesDefaultCulture()
    {
        // Arrange
        var options = new LocalizationOptions();

        // Act
        options.SetDefaultCulture("fr-FR");

        // Assert
        options.DefaultCulture.ShouldBe("fr-FR");
    }

    [Fact]
    public void SetDefaultCulture_AddsToSupportedCulturesIfNotPresent()
    {
        // Arrange
        var options = new LocalizationOptions
        {
            SupportedCultures = new List<string> { "en-US" }
        };

        // Act
        options.SetDefaultCulture("fr-FR");

        // Assert
        options.DefaultCulture.ShouldBe("fr-FR");
        options.SupportedCultures.Count.ShouldBe(2);
        options.SupportedCultures.ShouldContain("en-US");
        options.SupportedCultures.ShouldContain("fr-FR");
    }

    [Fact]
    public void SetDefaultCulture_DoesNotAddDuplicatesToSupportedCultures()
    {
        // Arrange
        var options = new LocalizationOptions
        {
            SupportedCultures = new List<string> { "en-US", "fr-FR" }
        };

        // Act
        options.SetDefaultCulture("fr-FR");

        // Assert
        options.DefaultCulture.ShouldBe("fr-FR");
        options.SupportedCultures.Count.ShouldBe(2);
        options.SupportedCultures.ShouldContain("en-US");
        options.SupportedCultures.ShouldContain("fr-FR");
    }

    [Fact]
    public void SetDefaultCulture_AlwaysAllowsEnUS()
    {
        // Arrange
        var options = new LocalizationOptions
        {
            SupportedCultures = new List<string> { "fr-FR" }
        };

        // Act
        options.SetDefaultCulture("en-US");

        // Assert
        options.DefaultCulture.ShouldBe("en-US");
        options.SupportedCultures.Count.ShouldBe(2);
        options.SupportedCultures.ShouldContain("en-US");
        options.SupportedCultures.ShouldContain("fr-FR");
    }
}