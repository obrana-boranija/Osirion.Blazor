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