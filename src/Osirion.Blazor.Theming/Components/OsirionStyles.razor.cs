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

            return $@"
        <script id=""osirion-framework-script"">
            (function() {{
                // Store these values in variables with proper scope
                const FRAMEWORK_CLASS = ""{frameworkClass}"";
                const CURRENT_THEME = ""{currentTheme}"";
                const FRAMEWORK = ""{EffectiveOptions.Framework}"";
                
                function applyFrameworkIntegration() {{
                    const htmlElement = document.documentElement || document.querySelector('html');

                    if (htmlElement && FRAMEWORK_CLASS && !htmlElement.classList.contains(FRAMEWORK_CLASS)) {{
                        htmlElement.classList.add(FRAMEWORK_CLASS);
                    }}

                    // Framework-specific theme handling
                    if (FRAMEWORK === ""Bootstrap"") {{
                        if (CURRENT_THEME === ""dark"" || CURRENT_THEME === ""light"") {{
                            htmlElement.setAttribute(""data-bs-theme"", CURRENT_THEME);
                        }}
                    }}
                    else if (FRAMEWORK === ""MudBlazor"") {{
                        if (CURRENT_THEME === ""dark"") {{
                            htmlElement.classList.add(""mud-theme-dark"");
                        }} else if (CURRENT_THEME === ""light"") {{
                            htmlElement.classList.remove(""mud-theme-dark"");
                        }}
                    }}
                    else if (FRAMEWORK === ""FluentUI"") {{
                        if (CURRENT_THEME === ""dark"") {{
                            htmlElement.classList.add(""fluent-dark-theme"");
                            htmlElement.classList.remove(""fluent-light-theme"");
                        }} else if (CURRENT_THEME === ""light"") {{
                            htmlElement.classList.add(""fluent-light-theme"");
                            htmlElement.classList.remove(""fluent-dark-theme"");
                        }}
                    }}
                    else if (FRAMEWORK === ""Radzen"") {{
                        if (CURRENT_THEME === ""dark"") {{
                            htmlElement.classList.add(""rz-dark-theme"");
                        }} else if (CURRENT_THEME === ""light"") {{
                            htmlElement.classList.remove(""rz-dark-theme"");
                        }}
                    }}
                }}

                // Apply immediately
                applyFrameworkIntegration();
                
                // Apply on page load events
                if (document.readyState === ""loading"") {{
                    document.addEventListener(""DOMContentLoaded"", applyFrameworkIntegration);
                }}
                
                // Register event handlers that reapply on navigation
                window.addEventListener(""load"", applyFrameworkIntegration);
                window.addEventListener(""hashchange"", applyFrameworkIntegration);
                window.addEventListener(""popstate"", applyFrameworkIntegration);
                
                // Check periodically if class was removed (like during navigation)
                setInterval(function() {{
                    const html = document.documentElement;
                    if (html && !html.classList.contains(FRAMEWORK_CLASS)) {{
                        applyFrameworkIntegration();
                    }}
                }}, 1000);
            }})();
        </script>";
        }

        public void Dispose()
        {
            // Unsubscribe to prevent memory leaks
            ThemeService.ThemeChanged -= OnThemeChanged;
            GC.SuppressFinalize(this);
        }
    }
}