using Microsoft.Playwright;

namespace Osirion.Blazor.E2ETests;

/// <summary>
/// Helper class for test application setup
/// </summary>
public class TestApp
{
    private readonly PlaywrightFixture _fixture;
    public IPage Page => _fixture.Page;

    public TestApp(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Navigates to the test application home page
    /// </summary>
    public async Task NavigateToHomeAsync()
    {
        await _fixture.GoToAsync("/");
    }

    /// <summary>
    /// Navigates to a specific component demo page
    /// </summary>
    public async Task NavigateToComponentAsync(string componentName)
    {
        await _fixture.GoToAsync($"/components/{componentName.ToLowerInvariant()}");
    }

    /// <summary>
    /// Waits for a component to be rendered and visible
    /// </summary>
    public async Task WaitForComponentAsync(string selector)
    {
        await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 10000
        });
    }

    /// <summary>
    /// Checks if a component is present in the DOM
    /// </summary>
    public async Task<bool> IsComponentPresentAsync(string selector)
    {
        var element = await Page.QuerySelectorAsync(selector);
        return element != null;
    }

    /// <summary>
    /// Gets text content from an element
    /// </summary>
    public async Task<string> GetTextContentAsync(string selector)
    {
        await WaitForComponentAsync(selector);
        return await Page.TextContentAsync(selector);
    }

    /// <summary>
    /// Clicks an element and waits for navigation or network idle
    /// </summary>
    public async Task ClickAndWaitAsync(string selector)
    {
        await WaitForComponentAsync(selector);
        await Page.ClickAsync(selector);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Takes a screenshot of the current page
    /// </summary>
    public async Task TakeScreenshotAsync(string name)
    {
        await _fixture.TakeScreenshotAsync(name);
    }
}