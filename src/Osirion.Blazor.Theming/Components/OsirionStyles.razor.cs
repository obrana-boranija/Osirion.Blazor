using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Services;

namespace Osirion.Blazor.Theming.Components
{
    public partial class OsirionStyles : OsirionComponentBase
    {
        [Parameter] public bool? UseStyles { get; set; }
        [Parameter] public string? CustomVariables { get; set; }
        [Parameter] public CssFramework? Framework { get; set; }
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

        protected override void OnInitialized()
        {
            ThemeService.ThemeChanged += OnThemeChanged;
            base.OnInitialized();
        }

        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            ThemeService.ThemeChanged -= OnThemeChanged;
            GC.SuppressFinalize(this);
        }
    }
}