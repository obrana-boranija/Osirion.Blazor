using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Services;

namespace Osirion.Blazor.Theming.Components
{
    /// <summary>Defines the OsirionStyles type.</summary>
    public partial class OsirionStyles : OsirionComponentBase, IDisposable
    {
        /// <summary>
        /// Gets or sets whether only the Core stylesheet is linked. When <see langword="true"/>, the framework adapter and
        /// the theme stylesheet are left out. When <see langword="null"/>, <c>ThemingOptions.UseDefaultStyles</c> applies
        /// (<see langword="false"/> unless configured).
        /// </summary>
        [Parameter] public bool? UseStyles { get; set; }
        /// <summary>
        /// Gets or sets CSS declarations written, as raw CSS without encoding, into a <c>:root</c> block. The configured
        /// <c>ThemingOptions.CustomVariables</c> value is written in a second block after this one, so for the same
        /// property the configured value wins. Pass only values from your own code or configuration.
        /// </summary>
        [Parameter] public string? CustomVariables { get; set; }
        /// <summary>
        /// Gets or sets which framework adapter stylesheet is linked: <c>Bootstrap</c>, <c>FluentUI</c>, <c>MudBlazor</c>
        /// or <c>Radzen</c>; <c>None</c> links none. When <see langword="null"/>, <c>ThemingOptions.Framework</c> applies.
        /// The framework class on <c>&lt;html&gt;</c> comes from the configured framework, so set the framework in
        /// configuration as well.
        /// </summary>
        [Parameter] public new CssFramework? Framework { get; set; }
        /// <summary>
        /// Gets or sets the theme used when the visitor has not picked one: <c>Light</c>, <c>Dark</c> or <c>System</c>.
        /// <c>Inverted</c> is not handled by the page script, which then follows the operating system. When
        /// <see langword="null"/>, <c>ThemingOptions.DefaultMode</c> applies (<c>System</c> unless configured).
        /// </summary>
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
                    FollowSystemPreference = options.FollowSystemPreference,
                    DefaultPalette = options.DefaultPalette,
                    AllowPaletteSelection = options.AllowPaletteSelection
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

        /// <summary>The address of the palette stylesheet; <see cref="ThemePicker"/> adds it when a page lacks it.</summary>
        internal const string PaletteStylesheet = "_content/Osirion.Blazor.Theming/css/palettes.min.css";

        // Linked whenever a palette may be in use, so it is painted with the first frame, not after a script loads it.
        private bool LinksPalettes =>
            EffectiveOptions.AllowPaletteSelection || ThemeService.CurrentPalette != ThemePalette.Framework;

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
