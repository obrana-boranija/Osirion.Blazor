using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Services;

namespace Osirion.Blazor.Cms.Admin.Components;
public partial class AdminPage(CmsAdminState adminState)
{
    [Parameter]
    public string Title { get; set; } = "Osirion CMS";

    [Parameter]
    public string? Subtitle { get; set; }

    [Parameter]
    public string CurrentPage { get; set; } = string.Empty;

    [Parameter]
    public string Theme { get; set; } = "light";

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
    public bool ShowLoginForm { get; set; } = false;

    [Parameter]
    public EventCallback OnSignOut { get; set; }

    [Parameter]
    public List<BreadcrumbItem>? BreadcrumbItems { get; set; }

    protected override void OnInitialized()
    {
        // If no current page specified but we have a URL, extract the page name
        if (string.IsNullOrEmpty(CurrentPage))
        {
            try
            {
                var uri = new Uri(NavigationManager.Uri);
                var path = uri.AbsolutePath;

                // Extract the last segment of the path as the page name
                var lastSegment = path.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (!string.IsNullOrEmpty(lastSegment))
                {
                    CurrentPage = char.ToUpper(lastSegment[0]) + lastSegment.Substring(1);
                }
            }
            catch
            {
                // Ignore errors in path extraction
            }
        }
    }

    private Task HandleSignOut()
    {
        if (OnSignOut.HasDelegate)
        {
            return OnSignOut.InvokeAsync();
        }

        return Task.CompletedTask;
    }

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    // Define BreadcrumbItem class for navigation
    public class BreadcrumbItem
    {
        public string Text { get; set; }
        public string Url { get; set; }

        public BreadcrumbItem(string text, string url = "")
        {
            Text = text;
            Url = url;
        }
    }
}
