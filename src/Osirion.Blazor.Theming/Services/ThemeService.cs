using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Theming.Services;

/// <summary>
/// Default implementation of the theme service. Framework-agnostic and SSR-friendly.
/// Persists user preference in a cookie when running under an HTTP context (Static SSR compatible).
/// </summary>
public class ThemeService : IThemeService
{
    private readonly ThemingOptions _options;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly ILogger<ThemeService>? _logger;
    private ThemeMode _currentMode;

    private const string ThemeCookieKey = "osirion-preferred-theme";

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeService"/> class.
    /// </summary>
    public ThemeService(IOptions<ThemingOptions> options, IHttpContextAccessor? httpContextAccessor = null, ILogger<ThemeService>? logger = null)
    {
        _options = options?.Value ?? new ThemingOptions();
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;

        // Initialize from default; will be adjusted per-request in CurrentMode getter
        _currentMode = _options.DefaultMode;
    }

    /// <inheritdoc/>
    public virtual CssFramework CurrentFramework => _options.Framework;

    /// <inheritdoc/>
    public ThemeMode CurrentMode
    {
        get
        {
            // Re-evaluate from cookie on each access to reflect latest user choice in SSR/static hosting scenarios
            var fromCookie = ReadThemeFromCookie();
            if (fromCookie is not null && fromCookie.Value != _currentMode)
            {
                _currentMode = fromCookie.Value;
            }
            return _currentMode;
        }
        set => SetThemeMode(value);
    }

    /// <inheritdoc/>
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    /// <inheritdoc/>
    public string GenerateThemeVariables()
    {
        var variables = new List<string>();

        // Custom variables
        if (!string.IsNullOrWhiteSpace(_options.CustomVariables))
        {
            variables.Add(_options.CustomVariables);
        }

        return string.Join("\n", variables);
    }

    /// <inheritdoc/>
    public string GetFrameworkClass()
    {
        return CurrentFramework switch
        {
            CssFramework.Bootstrap => "osirion-bootstrap-integration",
            CssFramework.FluentUI => "osirion-fluent-integration",
            CssFramework.MudBlazor => "osirion-mudblazor-integration",
            CssFramework.Radzen => "osirion-radzen-integration",
            _ => string.Empty
        };
    }

    /// <inheritdoc/>
    public void SetThemeMode(ThemeMode mode)
    {
        if (_options.EnableDarkMode == false)
        {
            return;
        }

        if (_currentMode == mode)
        {
            return;
        }

        var previousMode = _currentMode;
        _currentMode = mode;

        // Persist to cookie
        WriteThemeToCookie(mode);

        _logger?.LogInformation("Theme changed: {Previous} -> {Current}", previousMode, _currentMode);
        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(_currentMode, previousMode));
    }

    /// <summary>
    /// Toggle between light and dark mode, returns the new mode string ("light" or "dark").
    /// System mode is treated as the resolved system preference for toggling.
    /// </summary>
    public string ToggleTheme()
    {
        var current = ResolveEffectiveMode();
        var newMode = current == ThemeMode.Dark ? ThemeMode.Light : ThemeMode.Dark;
        SetThemeMode(newMode);
        return newMode == ThemeMode.Dark ? "dark" : "light";
    }

    private ThemeMode ResolveEffectiveMode()
    {
        if (_currentMode == ThemeMode.System)
        {
            // Server-side fallback when System is configured but no client preference is set
            // Client-side JavaScript will resolve this to actual system preference
            return ThemeMode.Light; // Safe fallback for SSR
        }
        return _currentMode;
    }

    private ThemeMode? ReadThemeFromCookie()
    {
        try
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            var requestCookies = httpContext?.Request?.Cookies;
            if (requestCookies is null) return null;

            // Read only the osirion cookie
            if (requestCookies.TryGetValue(ThemeCookieKey, out var value))
            {
                return ParseTheme(value);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogDebug(ex, "Failed reading theme cookie");
        }
        return null;
    }

    private static ThemeMode? ParseTheme(string? value)
    {
        return value?.ToLowerInvariant() switch
        {
            "light" => ThemeMode.Light,
            "dark" => ThemeMode.Dark,
            "system" => ThemeMode.System,
            _ => null
        };
    }

    private void WriteThemeToCookie(ThemeMode mode)
    {
        try
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext is null) return; // No HTTP context (e.g., WASM prerender) - skip persistence

            var secure = httpContext.Request?.IsHttps ?? false;
            var options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = false,
                Secure = secure,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };

            var value = mode switch
            {
                ThemeMode.Dark => "dark",
                ThemeMode.Light => "light",
                ThemeMode.System => "system",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(value))
            {
                // Write only the osirion cookie
                httpContext.Response.Cookies.Append(ThemeCookieKey, value, options);
                
                // Remove legacy cookie if it exists
                if (httpContext.Request.Cookies.ContainsKey("preferred-theme"))
                {
                    httpContext.Response.Cookies.Delete("preferred-theme");
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogDebug(ex, "Failed writing theme cookie");
        }
    }
}