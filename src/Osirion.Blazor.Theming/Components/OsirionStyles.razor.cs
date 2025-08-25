using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Services;

namespace Osirion.Blazor.Theming.Components
{
    public partial class OsirionStyles : OsirionComponentBase
    {
        /// <summary>
        /// Gets or sets whether to use default styles
        /// </summary>
        [Parameter]
        public bool? UseStyles { get; set; }

        /// <summary>
        /// Gets or sets custom CSS variables
        /// </summary>
        [Parameter]
        public string? CustomVariables { get; set; }

        /// <summary>
        /// Gets or sets the CSS framework to integrate with
        /// </summary>
        [Parameter]
        public CssFramework? Framework { get; set; }

        /// <summary>
        /// Gets or sets the theme mode
        /// </summary>
        [Parameter]
        public ThemeMode? Mode { get; set; }

        /// <summary>
        /// Gets the effective options, merging parameters with configured options
        /// </summary>
        private ThemingOptions EffectiveOptions
        {
            get
            {
                var options = Options?.Value ?? new ThemingOptions();
                return new ThemingOptions
                {
                    UseDefaultStyles = UseStyles ?? options.UseDefaultStyles,
                    CustomVariables = CustomVariables ?? options.CustomVariables,
                    Framework = Framework ?? options.Framework,
                    DefaultMode = Mode ?? options.DefaultMode,
                    EnableDarkMode = options.EnableDarkMode,
                    FollowSystemPreference = options.FollowSystemPreference
                };
            }
        }

        /// <summary>
        /// Gets the generated CSS variables for the current theme
        /// </summary>
        private string GeneratedVariables => ThemeService.GenerateThemeVariables();

        /// <summary>
        /// Gets the framework class based on the selected framework
        /// </summary>
        private string GetFrameworkClass => ThemeService.GetFrameworkClass();

        protected override void OnInitialized()
        {
            // Subscribe to theme changes to trigger re-render when theme changes
            ThemeService.ThemeChanged += OnThemeChanged;
            base.OnInitialized();
        }

        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        private string GetScript()
        {
            var frameworkClass = GetFrameworkClass;
            var currentTheme = ThemeService.CurrentMode.ToString().ToLowerInvariant();
            var framework = EffectiveOptions.Framework.ToString();

            // Use C# raw interpolated string literal to avoid escaping braces in JS
            return $$"""
        <script id="osirion-framework-script">
            (function() {
                const HTML = document.documentElement || document.querySelector('html');
                const FRAMEWORK_CLASS = '{{frameworkClass}}';
                const SELECTED_FRAMEWORK = '{{framework}}';
                let CURRENT_THEME = '{{currentTheme}}';
                let isInitialized = false;
                let lastAppliedTheme = null;

                function normalizeTheme(value) {
                    const t = (value || '').toLowerCase();
                    return (t === 'dark' || t === 'light') ? t : 'light';
                }

                function readCookie(name) {
                    const m = document.cookie.match(new RegExp('(?:^|; )' + name.replace(/([.$?*|{}()\[\]\\/+^])/g, '\\$1') + '=([^;]*)'));
                    return m ? decodeURIComponent(m[1]) : null;
                }

                function clearFrameworkMarkers() {
                    if (!HTML) return;
                    HTML.removeAttribute('data-bs-theme');
                    HTML.classList.remove('mud-theme-dark');
                    HTML.classList.remove('fluent-dark-theme');
                    HTML.classList.remove('fluent-light-theme');
                    HTML.classList.remove('rz-dark-theme');
                }

                function resolveTheme() {
                    // Only read osirion cookie (no legacy fallback)
                    const osirionCookie = readCookie('osirion-preferred-theme');
                    if (osirionCookie) return normalizeTheme(osirionCookie);
                    
                    // Fallback to existing attribute or server value
                    const attr = HTML?.getAttribute('data-osirion-theme');
                    return normalizeTheme(attr || CURRENT_THEME || 'light');
                }

                function applyTheme(theme) {
                    if (!HTML) return;
                    const t = normalizeTheme(theme);

                    // Skip if already applied this theme to prevent excessive re-application
                    if (lastAppliedTheme === t && isInitialized) {
                        return;
                    }

                    lastAppliedTheme = t;

                    // Generic attribute for all frameworks/consumers
                    HTML.setAttribute('data-osirion-theme', t);

                    // Save selected framework for other components
                    HTML.setAttribute('data-osirion-framework', SELECTED_FRAMEWORK);

                    // Framework class for CSS variable mapping
                    if (FRAMEWORK_CLASS && !HTML.classList.contains(FRAMEWORK_CLASS)) {
                        HTML.classList.add(FRAMEWORK_CLASS);
                    }

                    // Remove any previous framework markers to prevent conflicts
                    clearFrameworkMarkers();

                    // Optional framework-specific mirroring (only for the selected framework)
                    if (SELECTED_FRAMEWORK === 'Bootstrap') {
                        HTML.setAttribute('data-bs-theme', t);
                    } else if (SELECTED_FRAMEWORK === 'MudBlazor') {
                        HTML.classList.toggle('mud-theme-dark', t === 'dark');
                    } else if (SELECTED_FRAMEWORK === 'FluentUI') {
                        HTML.classList.toggle('fluent-dark-theme', t === 'dark');
                        HTML.classList.toggle('fluent-light-theme', t === 'light');
                    } else if (SELECTED_FRAMEWORK === 'Radzen') {
                        HTML.classList.toggle('rz-dark-theme', t === 'dark');
                    }
                }

                // Throttled function to prevent excessive calls
                let reapplyTimeout = null;
                function reapplyFromCookies() {
                    if (reapplyTimeout) {
                        clearTimeout(reapplyTimeout);
                    }
                    reapplyTimeout = setTimeout(() => {
                        const currentTheme = resolveTheme();
                        applyTheme(currentTheme);
                        reapplyTimeout = null;
                    }, 16); // ~60fps throttling
                }

                // Apply immediately on script load
                const initialTheme = resolveTheme();
                applyTheme(initialTheme);
                isInitialized = true;

                // Enhanced Navigation support for Blazor (.NET 8+)
                if (typeof Blazor !== 'undefined' && Blazor.addEventListener) {
                    // Listen for enhanced load events (Blazor enhanced navigation)
                    Blazor.addEventListener('enhancedload', reapplyFromCookies);
                }

                // Standard navigation events (throttled)
                if (document.readyState === 'loading') {
                    document.addEventListener('DOMContentLoaded', reapplyFromCookies);
                }
                window.addEventListener('load', reapplyFromCookies);
                window.addEventListener('popstate', reapplyFromCookies);
                window.addEventListener('hashchange', reapplyFromCookies);
                
                // Custom event for theme updates (immediate, not throttled)
                window.addEventListener('osirion-theme-update', () => {
                    const currentTheme = resolveTheme();
                    applyTheme(currentTheme);
                });
                
                // Reduced mutation observer frequency
                if (typeof MutationObserver !== 'undefined') {
                    let mutationTimeout = null;
                    const observer = new MutationObserver(function(mutations) {
                        let needsReapply = false;
                        mutations.forEach(function(mutation) {
                            if (mutation.type === 'attributes' && 
                                (mutation.attributeName === 'data-osirion-theme' || 
                                 mutation.attributeName === 'data-bs-theme')) {
                                needsReapply = true;
                            }
                        });
                        if (needsReapply) {
                          if (mutationTimeout) {
                            clearTimeout(mutationTimeout);
                          }
                          mutationTimeout = setTimeout(reapplyFromCookies, 100);
                        }
                    });
                    observer.observe(HTML, { 
                        attributes: true, 
                        attributeFilter: ['data-osirion-theme', 'data-bs-theme'] 
                    });
                }
            })();
        </script>
        """;
        }

        public void Dispose()
        {
            // Unsubscribe to prevent memory leaks
            ThemeService.ThemeChanged -= OnThemeChanged;
            GC.SuppressFinalize(this);
        }
    }
}