using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Components.Pages;

public partial class Settings : IDisposable
{
    [Inject]
    private IAuthenticationService AuthService { get; set; } = default!;

    [Parameter]
    public string Theme { get; set; } = "light";

    [Parameter]
    public EventCallback<string> ThemeChanged { get; set; }

    protected override void OnInitialized()
    {
        // Subscribe to state changes
        AdminState.StateChanged += StateHasChanged;
    }

    protected override void OnParametersSet()
    {
        // Set default theme
        if (string.IsNullOrEmpty(Theme))
        {
            Theme = "light";
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (ThemeChanged.HasDelegate && !firstRender)
        {
            await ThemeChanged.InvokeAsync(Theme);
        }
    }

    private async Task SignOut()
    {
        try
        {
            await AuthService.SignOutAsync();
            NavigationManager.NavigateTo("/osirion/login");
        }
        catch (Exception ex)
        {
            AdminState.SetErrorMessage($"Error signing out: {ex.Message}");
        }
    }

    public void Dispose()
    {
        // Unsubscribe from state changes
        AdminState.StateChanged -= StateHasChanged;
    }
}