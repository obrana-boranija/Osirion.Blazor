using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Models;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Features.Layouts.Components;

public partial class AdminLayout : IDisposable
{
    [Inject]
    protected IAuthenticationService authService { get; set; } = default!;

    [Parameter]
    public string Title { get; set; } = "Osirion CMS";

    [Parameter]
    public string? Subtitle { get; set; }

    [Parameter]
    public string Theme { get; set; } = "light";

    [Parameter]
    public EventCallback<string> ThemeChanged { get; set; }

    [Parameter]
    public string? StatusMessage { get; set; }

    [Parameter]
    public EventCallback<string?> StatusMessageChanged { get; set; }

    [Parameter]
    public string? ErrorMessage { get; set; }

    [Parameter]
    public EventCallback<string?> ErrorMessageChanged { get; set; }

    [Parameter]
    public string? CurrentPage { get; set; }

    [Parameter]
    public List<BreadcrumbItem>? BreadcrumbItems { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    [Parameter]
    public RenderFragment? NavigationTemplate { get; set; }

    [Parameter]
    public RenderFragment? ActionsTemplate { get; set; }

    [Parameter]
    public RenderFragment? SidebarFooterTemplate { get; set; }

    [Parameter]
    public EventCallback OnSignOut { get; set; }

    protected override void OnInitialized()
    {
        AdminState.StateChanged += StateHasChanged;
    }

    private async Task SignOut()
    {
        await authService.SignOutAsync();

        if (OnSignOut.HasDelegate)
        {
            await OnSignOut.InvokeAsync();
        }
    }

    private async Task ClearStatusMessage()
    {
        StatusMessage = null;
        await StatusMessageChanged.InvokeAsync(null);
    }

    private async Task ClearErrorMessage()
    {
        ErrorMessage = null;
        await ErrorMessageChanged.InvokeAsync(null);
    }

    private async Task ToggleTheme()
    {
        Theme = Theme == "light" ? "dark" : "light";
        if (ThemeChanged.HasDelegate)
        {
            await ThemeChanged.InvokeAsync(Theme);
        }
    }

    public void Dispose()
    {
        AdminState.StateChanged -= StateHasChanged;
    }
}