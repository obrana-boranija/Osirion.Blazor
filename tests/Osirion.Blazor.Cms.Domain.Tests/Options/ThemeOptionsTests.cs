using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Shouldly;

namespace Osirion.Blazor.Cms.Tests.Options;

public class ThemeOptionsTests
{
    [Fact]
    public void DefaultProperties_HaveExpectedValues()
    {
        // Arrange
        var options = new ThemeOptions();

        // Assert
        options.DefaultMode.ShouldBe("light");
        options.PrimaryColor.ShouldBe("#0366d6");
        options.UseDarkMode.ShouldBeFalse();
        options.RespectSystemPreferences.ShouldBeTrue();
    }

    [Fact]
    public void Properties_CanBeSet_AndRetrieved()
    {
        // Arrange
        var options = new ThemeOptions
        {
            DefaultMode = "dark",
            PrimaryColor = "#ff5722",
            UseDarkMode = true,
            RespectSystemPreferences = false
        };

        // Assert
        options.DefaultMode.ShouldBe("dark");
        options.PrimaryColor.ShouldBe("#ff5722");
        options.UseDarkMode.ShouldBeTrue();
        options.RespectSystemPreferences.ShouldBeFalse();
    }

    [Theory]
    [InlineData("light")]
    [InlineData("dark")]
    [InlineData("system")]
    [InlineData("custom")]
    public void DefaultMode_CanBeSetToAnyValue(string mode)
    {
        // Arrange
        var options = new ThemeOptions
        {
            DefaultMode = mode
        };

        // Assert
        options.DefaultMode.ShouldBe(mode);
    }

    [Theory]
    [InlineData("#000000")]
    [InlineData("#FFFFFF")]
    [InlineData("#ff5722")]
    [InlineData("rgb(255, 0, 0)")]
    [InlineData("var(--my-color)")]
    public void PrimaryColor_CanBeSetToAnyValue(string color)
    {
        // Arrange
        var options = new ThemeOptions
        {
            PrimaryColor = color
        };

        // Assert
        options.PrimaryColor.ShouldBe(color);
    }

    [Fact]
    public void UseDarkMode_DefaultValueIsFalse()
    {
        // Arrange
        var options = new ThemeOptions();

        // Assert
        options.UseDarkMode.ShouldBeFalse();
    }

    [Fact]
    public void RespectSystemPreferences_DefaultValueIsTrue()
    {
        // Arrange
        var options = new ThemeOptions();

        // Assert
        options.RespectSystemPreferences.ShouldBeTrue();
    }
}