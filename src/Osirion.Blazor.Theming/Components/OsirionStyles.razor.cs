using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Services;

namespace Osirion.Blazor.Theming.Components
{
    public partial class OsirionStyles : OsirionComponentBase, IDisposable
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
        private ThemingOptions EffectiveOptions => new()
        {
            UseDefaultStyles = UseStyles ?? Options?.Value.UseDefaultStyles ?? true,
            CustomVariables = CustomVariables ?? Options?.Value.CustomVariables,
            Framework = Framework ?? Options?.Value.Framework ?? CssFramework.None,
            DefaultMode = Mode ?? Options?.Value.DefaultMode ?? ThemeMode.Light
        };

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

        public void Dispose()
        {
            // Unsubscribe to prevent memory leaks
            ThemeService.ThemeChanged -= OnThemeChanged;
            GC.SuppressFinalize(this);
        }
    }
}