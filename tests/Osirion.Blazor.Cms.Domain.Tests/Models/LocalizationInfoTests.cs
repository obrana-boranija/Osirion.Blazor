using Osirion.Blazor.Cms.Domain.Models;

namespace Osirion.Blazor.Cms.Domain.Tests.Models;

public class LocalizationInfoTests
{
    [Fact]
    public void Constructor_InitializesDefaultValues()
    {
        // Act
        var localizationInfo = new LocalizationInfo();

        // Assert
        Assert.Equal("en", localizationInfo.DefaultLocale);
        Assert.NotNull(localizationInfo.AvailableLocales);
        Assert.Empty(localizationInfo.AvailableLocales);
        Assert.NotNull(localizationInfo.Translations);
        Assert.Empty(localizationInfo.Translations);
    }

    [Fact]
    public void GetBestMatchingLocale_WithExactMatch_ReturnsExactMatch()
    {
        // Arrange
        var localizationInfo = new LocalizationInfo
        {
            DefaultLocale = "en",
            AvailableLocales = new List<LocaleInfo>
            {
                new LocaleInfo { Code = "en" },
                new LocaleInfo { Code = "fr" },
                new LocaleInfo { Code = "de" },
                new LocaleInfo { Code = "en-US" }
            }
        };

        // Act
        var result = localizationInfo.GetBestMatchingLocale("en-US");

        // Assert
        Assert.Equal("en-US", result);
    }

    [Fact]
    public void GetBestMatchingLocale_WithLanguageMatch_ReturnsLanguageMatch()
    {
        // Arrange
        var localizationInfo = new LocalizationInfo
        {
            DefaultLocale = "en",
            AvailableLocales = new List<LocaleInfo>
            {
                new LocaleInfo { Code = "en" },
                new LocaleInfo { Code = "fr" },
                new LocaleInfo { Code = "de" }
            }
        };

        // Act
        var result = localizationInfo.GetBestMatchingLocale("en-US");

        // Assert
        Assert.Equal("en", result);
    }

    [Fact]
    public void GetBestMatchingLocale_WithLanguageMatch_ReturnsMatchingVariant()
    {
        // Arrange
        var localizationInfo = new LocalizationInfo
        {
            DefaultLocale = "en",
            AvailableLocales = new List<LocaleInfo>
            {
                new LocaleInfo { Code = "en-GB" },
                new LocaleInfo { Code = "fr" },
                new LocaleInfo { Code = "de" }
            }
        };

        // Act
        var result = localizationInfo.GetBestMatchingLocale("en");

        // Assert
        Assert.Equal("en-GB", result);
    }

    [Fact]
    public void GetBestMatchingLocale_WithNoMatch_ReturnsDefaultLocale()
    {
        // Arrange
        var localizationInfo = new LocalizationInfo
        {
            DefaultLocale = "en",
            AvailableLocales = new List<LocaleInfo>
            {
                new LocaleInfo { Code = "en" },
                new LocaleInfo { Code = "fr" },
                new LocaleInfo { Code = "de" }
            }
        };

        // Act
        var result = localizationInfo.GetBestMatchingLocale("es");

        // Assert
        Assert.Equal("en", result);
    }

    [Fact]
    public void GetBestMatchingLocale_WithEmptyInput_ReturnsDefaultLocale()
    {
        // Arrange
        var localizationInfo = new LocalizationInfo
        {
            DefaultLocale = "en",
            AvailableLocales = new List<LocaleInfo>
            {
                new LocaleInfo { Code = "en" },
                new LocaleInfo { Code = "fr" }
            }
        };

        // Act
        var result = localizationInfo.GetBestMatchingLocale("");

        // Assert
        Assert.Equal("en", result);
    }

    [Fact]
    public void GetBestMatchingLocale_WithCaseInsensitiveMatch_ReturnsMatch()
    {
        // Arrange
        var localizationInfo = new LocalizationInfo
        {
            DefaultLocale = "en",
            AvailableLocales = new List<LocaleInfo>
            {
                new LocaleInfo { Code = "en" },
                new LocaleInfo { Code = "fr" },
                new LocaleInfo { Code = "DE" }
            }
        };

        // Act
        var result = localizationInfo.GetBestMatchingLocale("de");

        // Assert
        Assert.Equal("DE", result);
    }

    [Fact]
    public void AddLocale_AddsToAvailableLocales()
    {
        // Arrange
        var localizationInfo = new LocalizationInfo();

        // Act
        localizationInfo.AddLocale("fr", "French", "Français");

        // Assert
        Assert.Single(localizationInfo.AvailableLocales);
        Assert.Equal("fr", localizationInfo.AvailableLocales[0].Code);
        Assert.Equal("French", localizationInfo.AvailableLocales[0].Name);
        Assert.Equal("Français", localizationInfo.AvailableLocales[0].NativeName);
    }

    [Fact]
    public void AddLocale_WithExistingCode_DoesNotAddDuplicate()
    {
        // Arrange
        var localizationInfo = new LocalizationInfo();
        localizationInfo.AddLocale("fr", "French", "Français");

        // Act
        localizationInfo.AddLocale("fr", "French Updated", "Français Updated");

        // Assert
        Assert.Single(localizationInfo.AvailableLocales);
        Assert.Equal("fr", localizationInfo.AvailableLocales[0].Code);
        Assert.Equal("French", localizationInfo.AvailableLocales[0].Name);
    }

    [Fact]
    public void AddLocale_WithNullNativeName_UsesNameForNativeName()
    {
        // Arrange
        var localizationInfo = new LocalizationInfo();

        // Act
        localizationInfo.AddLocale("de", "German", null);

        // Assert
        Assert.Single(localizationInfo.AvailableLocales);
        Assert.Equal("German", localizationInfo.AvailableLocales[0].NativeName);
    }
}

public class LocaleInfoTests
{
    [Fact]
    public void Constructor_InitializesDefaultValues()
    {
        // Act
        var localeInfo = new LocaleInfo();

        // Assert
        Assert.Equal("", localeInfo.Code);
        Assert.Equal("", localeInfo.Name);
        Assert.Equal("", localeInfo.NativeName);
        Assert.Null(localeInfo.Flag);
        Assert.Equal("ltr", localeInfo.Direction);
        Assert.False(localeInfo.IsDefault);
        Assert.NotNull(localeInfo.Properties);
        Assert.Empty(localeInfo.Properties);
    }
}