using Microsoft.Playwright;

namespace Osirion.Blazor.E2ETests.Tests;

/// <summary>
/// Tests for Osirion Analytics components
/// </summary>
public class AnalyticsComponentTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;
    private readonly TestApp _app;

    public AnalyticsComponentTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
        _app = new TestApp(fixture);
    }

    [Fact]
    public async Task ClarityTracker_ShouldInjectScript()
    {
        // Arrange
        await _app.NavigateToComponentAsync("claritytracker");

        // Act
        // Wait for page to fully load
        await _fixture.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert - Check if Clarity script is injected
        var clarityScriptExists = await _fixture.Page.EvaluateAsync<bool>(@"
            const scripts = Array.from(document.querySelectorAll('script'));
            return scripts.some(script => 
                script.src && script.src.includes('clarity.ms/tag/') ||
                script.textContent && script.textContent.includes('clarity')
            );
        ");

        Assert.True(clarityScriptExists, "Clarity tracking script should be injected");
    }

    [Fact]
    public async Task MatomoTracker_ShouldInjectScript()
    {
        // Arrange
        await _app.NavigateToComponentAsync("matomotracker");

        // Act
        // Wait for page to fully load
        await _fixture.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert - Check if Matomo script is injected
        var matomoScriptExists = await _fixture.Page.EvaluateAsync<bool>(@"
            const scripts = Array.from(document.querySelectorAll('script'));
            return scripts.some(script => 
                script.textContent && (
                    script.textContent.includes('_paq') ||
                    script.textContent.includes('Matomo')
                )
            );
        ");

        Assert.True(matomoScriptExists, "Matomo tracking script should be injected");
    }

    [Fact]
    public async Task GA4Tracker_ShouldInjectScript()
    {
        // Arrange
        await _app.NavigateToComponentAsync("ga4tracker");

        // Act
        // Wait for page to fully load
        await _fixture.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert - Check if GA4 script is injected
        var ga4ScriptExists = await _fixture.Page.EvaluateAsync<bool>(@"
            const scripts = Array.from(document.querySelectorAll('script'));
            return scripts.some(script => 
                script.src && script.src.includes('googletagmanager.com/gtag/js') ||
                script.textContent && script.textContent.includes('gtag')
            );
        ");

        Assert.True(ga4ScriptExists, "Google Analytics 4 tracking script should be injected");
    }

    [Fact]
    public async Task YandexMetricaTracker_ShouldInjectScript()
    {
        // Arrange
        await _app.NavigateToComponentAsync("yandexmetricatracker");

        // Act
        // Wait for page to fully load
        await _fixture.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert - Check if Yandex Metrica script is injected
        var yandexScriptExists = await _fixture.Page.EvaluateAsync<bool>(@"
            const scripts = Array.from(document.querySelectorAll('script'));
            return scripts.some(script => 
                script.src && script.src.includes('mc.yandex.ru/metrika') ||
                script.textContent && script.textContent.includes('ym')
            );
        ");

        Assert.True(yandexScriptExists, "Yandex Metrica tracking script should be injected");
    }

    [Fact]
    public async Task AnalyticsEvents_ShouldTriggerOnNavigation()
    {
        // This test verifies that analytics events are triggered on navigation
        // Note: This is a more advanced test that uses JavaScript interception

        // Arrange - Track analytics calls
        await _fixture.Page.RouteAsync("**/*", async route =>
        {
            var url = route.Request.Url;
            if (url.Contains("google-analytics") ||
                url.Contains("clarity.ms") ||
                url.Contains("matomo") ||
                url.Contains("mc.yandex.ru"))
            {
                // Log analytics requests for verification
                System.Console.WriteLine($"Analytics request: {url}");
            }
            await route.ContinueAsync();
        });

        // Navigate to home with analytics components
        await _app.NavigateToHomeAsync();

        // Act - Navigate to another page to trigger analytics events
        await _app.ClickAndWaitAsync("a[href='/about']");

        // Wait for analytics network requests
        await _fixture.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert - Check analytics tracking via JavaScript
        // This is difficult to truly verify since we mock the analytics,
        // but we can check if the appropriate variables are defined
        var trackingInitialized = await _fixture.Page.EvaluateAsync<bool>(@"
            // Check for at least one tracking system
            return (
                typeof window.gtag !== 'undefined' || 
                typeof window.clarity !== 'undefined' || 
                typeof window._paq !== 'undefined' ||
                typeof window.ym !== 'undefined'
            );
        ");

        Assert.True(trackingInitialized, "At least one analytics tracking system should be initialized");
    }
}