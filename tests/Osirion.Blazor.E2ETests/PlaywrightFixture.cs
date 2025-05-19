using Microsoft.Playwright;

namespace Osirion.Blazor.E2ETests;

/// <summary>
/// Base fixture for Playwright tests
/// </summary>
public class PlaywrightFixture : IAsyncLifetime
{
    public IPlaywright Playwright { get; private set; }
    public IBrowser Browser { get; private set; }
    public IBrowserContext Context { get; private set; }
    public IPage Page { get; private set; }

    /// <summary>
    /// Base URL for the test application
    /// For local development, typically http://localhost:5000
    /// In CI/CD, this could be dynamically assigned
    /// </summary>
    public string BaseUrl { get; private set; } = "http://localhost:5000";

    public PlaywrightFixture()
    {
        // Environment override for CI/CD scenarios
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEST_BASE_URL")))
        {
            BaseUrl = Environment.GetEnvironmentVariable("TEST_BASE_URL");
        }
    }

    public async Task InitializeAsync()
    {
        // Initialize Playwright
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        // Launch browser (headless in CI, headful locally for debugging)
        bool isCI = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = isCI,
            SlowMo = isCI ? 0 : 100, // Slow down operations locally for visibility
        });

        // Create a new context with viewport and user agent
        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
            UserAgent = "Playwright-Test/1.0 (Osirion.Blazor E2E Tests)"
        });

        // Create a new page
        Page = await Context.NewPageAsync();

        // Set default timeouts
        Page.SetDefaultTimeout(30000);
        Page.SetDefaultNavigationTimeout(60000);
    }

    public async Task DisposeAsync()
    {
        // Clean up resources
        await Page?.CloseAsync();
        await Context?.CloseAsync();
        await Browser?.CloseAsync();
        Playwright?.Dispose();
    }

    /// <summary>
    /// Helper method to navigate to a specific route in the test application
    /// </summary>
    public async Task<IPage> GoToAsync(string route)
    {
        string url = $"{BaseUrl.TrimEnd('/')}/{route.TrimStart('/')}";
        await Page.GotoAsync(url);

        // Wait for Blazor to be fully loaded
        await Page.WaitForFunctionAsync("() => window['Blazor'] && window['Blazor']._internal && window['Blazor']._internal.navigationManager");

        return Page;
    }

    /// <summary>
    /// Helper method to take screenshot with timestamped filename
    /// </summary>
    public async Task TakeScreenshotAsync(string name)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = $"{name}_{timestamp}.png";
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = filename,
            FullPage = true
        });
    }
}