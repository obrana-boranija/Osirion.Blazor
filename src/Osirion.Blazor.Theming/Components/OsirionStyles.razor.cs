using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Services;

namespace Osirion.Blazor.Theming.Components
{
    /// <summary>Defines the OsirionStyles type.</summary>
    public partial class OsirionStyles : OsirionComponentBase
    {
        /// <summary>Gets or sets whether the default Osirion styles are rendered.</summary>
        [Parameter] public bool? UseStyles { get; set; }
        /// <summary>Gets or sets custom CSS variables appended to the generated theme styles.</summary>
        [Parameter] public string? CustomVariables { get; set; }
        /// <summary>Gets or sets the CSS framework adapter used by the component.</summary>
        [Parameter] public new CssFramework? Framework { get; set; }
        /// <summary>Gets or sets the theme mode used for generated styles.</summary>
        [Parameter] public ThemeMode? Mode { get; set; }

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

        private string ThemeAdapter() => EffectiveOptions.Framework switch
        {
            CssFramework.Bootstrap => "_content/Osirion.Blazor.Theming/css/adapters/bootstrap-adapter.min.css",
            CssFramework.FluentUI => "_content/Osirion.Blazor.Theming/css/adapters/fluentui-adapter.min.css",
            CssFramework.MudBlazor => "_content/Osirion.Blazor.Theming/css/adapters/mudblazor-adapter.min.css",
            CssFramework.Radzen => "_content/Osirion.Blazor.Theming/css/adapters/radzen-adapter.min.css",
            CssFramework.None => string.Empty,
            _ => string.Empty
        };

        private string GeneratedVariables => ThemeService.GenerateThemeVariables();
        private string GetFrameworkClass => ThemeService.GetFrameworkClass();

        /// <summary>Initializes the component state and required services.</summary>
        protected override void OnInitialized()
        {
            ThemeService.ThemeChanged += OnThemeChanged;
            base.OnInitialized();
        }

        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        /// <summary>Releases resources held by the component or service.</summary>
        public void Dispose()
        {
            ThemeService.ThemeChanged -= OnThemeChanged;
            GC.SuppressFinalize(this);
        }
    }
}
