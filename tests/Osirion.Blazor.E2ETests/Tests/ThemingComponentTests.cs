namespace Osirion.Blazor.E2ETests.Tests;

/// <summary>
/// Tests for Osirion Theming components
/// </summary>
public class ThemingComponentTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;
    private readonly TestApp _app;

    public ThemingComponentTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
        _app = new TestApp(fixture);
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task OsirionStyles_ShouldApplyCssVariables()
    {
        // Arrange
        await _app.NavigateToComponentAsync("osirionstyles");

        // Act
        await _app.WaitForComponentAsync("#theme-demo");

        // Assert
        // Check if CSS variables are properly applied by checking computed styles
        var primaryColor = await _fixture.Page.EvaluateAsync<string>(@"
            const element = document.querySelector('#theme-demo');
            const styles = window.getComputedStyle(element);
            return styles.getPropertyValue('--osirion-primary-color').trim();
        ");

        // Should not be empty if variables are applied
        Assert.False(string.IsNullOrWhiteSpace(primaryColor), "Primary color CSS variable should be set");

        // Take screenshot
        await _app.TakeScreenshotAsync("OsirionStyles_Applied");
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task BootstrapIntegration_ShouldApplyCorrectMappings()
    {
        // Arrange
        await _app.NavigateToComponentAsync("osirionstyles?framework=bootstrap");

        // Act
        await _app.WaitForComponentAsync(".osirion-bootstrap-integration");

        // Assert
        // Verify Bootstrap integration class is present
        var hasIntegrationClass = await _fixture.Page.EvaluateAsync<bool>(@"
            return document.documentElement.classList.contains('osirion-bootstrap-integration') ||
                   document.body.classList.contains('osirion-bootstrap-integration');
        ");

        Assert.True(hasIntegrationClass, "Bootstrap integration class should be applied");

        // Check if a Bootstrap-specific mapping is applied
        var primaryColorMapped = await _fixture.Page.EvaluateAsync<bool>(@"
            const rootStyles = getComputedStyle(document.documentElement);
            const primaryColor = rootStyles.getPropertyValue('--osirion-primary-color').trim();
            const bsColor = rootStyles.getPropertyValue('--bs-primary').trim();
            return primaryColor && bsColor && primaryColor === bsColor;
        ");

        Assert.True(primaryColorMapped, "Bootstrap color mapping should be applied");

        // Take screenshot
        await _app.TakeScreenshotAsync("BootstrapIntegration");
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task CustomVariables_ShouldOverrideDefaults()
    {
        // Arrange - Navigate to component with custom variables
        await _app.NavigateToComponentAsync("osirionstyles?customVariables=true");

        // Act
        await _app.WaitForComponentAsync("#custom-variables-demo");

        // Wait for styles to be applied
        await Task.Delay(500);

        // Assert
        // Check if custom CSS variables override defaults
        var customPrimaryColor = await _fixture.Page.EvaluateAsync<string>(@"
            const element = document.querySelector('#custom-variables-demo');
            const styles = window.getComputedStyle(element);
            return styles.getPropertyValue('--osirion-primary-color').trim();
        ");

        // Custom primary color should match the demo value
        var expectedColor = "#0077cc"; // Assuming this is the custom value in the demo
        Assert.Equal(expectedColor, customPrimaryColor);

        // Take screenshot
        await _app.TakeScreenshotAsync("CustomVariables_Applied");
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task DarkMode_ShouldApplyDarkTheme()
    {
        // Arrange - Navigate to component with dark mode
        await _app.NavigateToComponentAsync("osirionstyles?darkMode=true");

        // Act
        await _app.WaitForComponentAsync(".dark-theme");

        // Assert
        // Check if dark theme class is applied
        var hasDarkThemeClass = await _fixture.Page.EvaluateAsync<bool>(@"
            return document.documentElement.classList.contains('dark-theme') ||
                   document.body.classList.contains('dark-theme');
        ");

        Assert.True(hasDarkThemeClass, "Dark theme class should be applied");

        // Check if dark theme CSS variables are applied
        var backgroundColor = await _fixture.Page.EvaluateAsync<string>(@"
            const styles = window.getComputedStyle(document.body);
            return styles.getPropertyValue('--osirion-color-background').trim();
        ");

        // Dark background should be a dark color (not white)
        Assert.NotEqual("#ffffff", backgroundColor.ToLower());
        Assert.NotEqual("rgb(255, 255, 255)", backgroundColor.ToLower());

        // Take screenshot
        await _app.TakeScreenshotAsync("DarkTheme_Applied");
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task ThemeToggle_ShouldSwitchThemes()
    {
        // This test is for demonstration purposes and assumes a theme toggle exists in the demo

        // Arrange
        await _app.NavigateToComponentAsync("osirionstyles?themeToggle=true");

        // Act
        await _app.WaitForComponentAsync("#theme-toggle");

        // Get initial theme state
        var initialIsDarkTheme = await _fixture.Page.EvaluateAsync<bool>(@"
            return document.documentElement.classList.contains('dark-theme') ||
                   document.body.classList.contains('dark-theme');
        ");

        // Toggle theme
        await _fixture.Page.ClickAsync("#theme-toggle");

        // Wait for theme change to apply
        await Task.Delay(500);

        // Assert
        var newIsDarkTheme = await _fixture.Page.EvaluateAsync<bool>(@"
            return document.documentElement.classList.contains('dark-theme') ||
                   document.body.classList.contains('dark-theme');
        ");

        // Theme should have toggled
        Assert.NotEqual(initialIsDarkTheme, newIsDarkTheme);

        // Take screenshot
        await _app.TakeScreenshotAsync("ThemeToggle_Changed");
    }
}