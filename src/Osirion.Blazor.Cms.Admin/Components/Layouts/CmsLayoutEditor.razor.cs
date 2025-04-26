using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Services;

namespace Osirion.Blazor.Cms.Admin.Components;

public partial class CmsLayoutEditor(CmsAdminState adminState)
{
    /// <summary>
    /// Gets or sets the page title
    /// </summary>
    [Parameter]
    public string Title { get; set; } = "Content Editor";

    /// <summary>
    /// Gets or sets the current theme (light or dark)
    /// </summary>
    [Parameter]
    public string Theme { get; set; } = "light";

    /// <summary>
    /// Gets or sets breadcrumb items for navigation
    /// </summary>
    [Parameter]
    public List<AdminPage.BreadcrumbItem>? BreadcrumbItems { get; set; }

    /// <summary>
    /// Gets or sets the child content
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the actions template
    /// </summary>
    [Parameter]
    public RenderFragment? ActionsTemplate { get; set; }

    /// <summary>
    /// Gets or sets the status message
    /// </summary>
    [Parameter]
    public string? StatusMessage { get; set; }

    /// <summary>
    /// Gets or sets the error message
    /// </summary>
    [Parameter]
    public string? ErrorMessage { get; set; }

    private string? LocalStatusMessage => StatusMessage ?? adminState.StatusMessage;
    private string? LocalErrorMessage => ErrorMessage ?? adminState.ErrorMessage;

    private void ClearStatusMessage()
    {
        if (StatusMessage != null)
        {
            StatusMessage = null;
        }
        else
        {
            adminState.ClearMessages();
        }
    }

    private void ClearErrorMessage()
    {
        if (ErrorMessage != null)
        {
            ErrorMessage = null;
        }
        else
        {
            adminState.ClearMessages();
        }
    }

    private string GetEditorLayoutClass()
    {
        return $"osirion-editor-layout osirion-admin-theme-{Theme} {CssClass}".Trim();
    }
}
