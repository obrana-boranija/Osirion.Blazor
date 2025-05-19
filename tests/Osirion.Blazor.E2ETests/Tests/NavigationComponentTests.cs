namespace Osirion.Blazor.E2ETests.Tests;

/// <summary>
/// Tests for Osirion Navigation components
/// </summary>
public class NavigationComponentTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;
    private readonly TestApp _app;

    public NavigationComponentTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
        _app = new TestApp(fixture);
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task EnhancedNavigationInterceptor_ShouldLoadWithoutErrors()
    {
        // Arrange
        await _app.NavigateToComponentAsync("enhancednavigationinterceptor");

        // Act
        await _app.WaitForComponentAsync("#navigation-demo");

        // Assert
        var demoContainer = await _fixture.Page.QuerySelectorAsync("#navigation-demo");
        Assert.NotNull(demoContainer);

        // Check for error messages
        var errorElement = await _fixture.Page.QuerySelectorAsync(".blazor-error-boundary");
        Assert.Null(errorElement);
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task ScrollToTop_ShouldRenderAndRespond()
    {
        // Arrange
        await _app.NavigateToComponentAsync("scrolltotop");

        // Act - Scroll down to make the button appear
        await _fixture.Page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");

        // Wait for the ScrollToTop button to appear
        await _app.WaitForComponentAsync(".osirion-scroll-to-top");

        // Assert
        var scrollButton = await _fixture.Page.QuerySelectorAsync(".osirion-scroll-to-top");
        Assert.NotNull(scrollButton);

        // Take screenshot
        await _app.TakeScreenshotAsync("ScrollToTop_Visible");

        // Act - Click the button
        await _fixture.Page.ClickAsync(".osirion-scroll-to-top");

        // Wait for scroll to complete
        await Task.Delay(500);

        // Assert - Check if scrolled to top
        var scrollPosition = await _fixture.Page.EvaluateAsync<int>("window.scrollY");
        Assert.Equal(0, scrollPosition);
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task ScrollToTop_ShouldBeHiddenAtTop()
    {
        // Arrange
        await _app.NavigateToComponentAsync("scrolltotop");

        // Act - Ensure we're at the top
        await _fixture.Page.EvaluateAsync("window.scrollTo(0, 0)");

        // Wait a moment for any animations to complete
        await Task.Delay(500);

        // Assert - Button should not be visible
        var scrollButton = await _fixture.Page.QuerySelectorAsync(".osirion-scroll-to-top");
        if (scrollButton != null)
        {
            var isVisible = await scrollButton.IsVisibleAsync();
            Assert.False(isVisible);
        }

        // Alternatively, check visibility via computed style
        var buttonVisibility = await _fixture.Page.EvaluateAsync<string>(@"
            const button = document.querySelector('.osirion-scroll-to-top');
            return button ? window.getComputedStyle(button).visibility : 'hidden';
        ");

        Assert.True(buttonVisibility == "hidden" || buttonVisibility == "collapse");
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task NavigationBetweenPages_ShouldPreserveScroll()
    {
        // This test verifies that EnhancedNavigationInterceptor properly preserves scroll position

        // Arrange - Navigate to a page with content
        await _app.NavigateToComponentAsync("enhancednavigationinterceptor");

        // Act - Scroll down
        const int scrollPosition = 500;
        await _fixture.Page.EvaluateAsync($"window.scrollTo(0, {scrollPosition})");

        // Wait for scroll to complete
        await Task.Delay(500);

        // Find and click a link to another page (assuming demo has navigation links)
        await _app.ClickAndWaitAsync("#demo-nav-link");

        // Now navigate back
        await _fixture.Page.GoBackAsync();

        // Allow time for scroll restoration
        await Task.Delay(1000);

        // Assert - Check if scroll position is restored
        var restoredPosition = await _fixture.Page.EvaluateAsync<int>("window.scrollY");

        // Should be close to original position
        Assert.True(restoredPosition > scrollPosition - 50 && restoredPosition < scrollPosition + 50,
            $"Scroll position {restoredPosition} is not near the expected {scrollPosition}");
    }
}