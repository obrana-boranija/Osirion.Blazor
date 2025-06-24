using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Models;
using Osirion.Blazor.Cms.Admin.Shared.Components;

namespace Osirion.Blazor.Cms.Admin.Features.Layouts.Components;

public partial class CmsLayoutEditor : BaseComponent
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
    public List<BreadcrumbItem>? BreadcrumbItems { get; set; }

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

    private string? LocalStatusMessage => StatusMessage ?? AdminState.StatusMessage;
    private string? LocalErrorMessage => ErrorMessage ?? AdminState.ErrorMessage;

    private void ClearStatusMessage()
    {
        if (StatusMessage is not null)
        {
            StatusMessage = null;
        }
        else
        {
            AdminState.ClearMessages();
        }
    }

    private void ClearErrorMessage()
    {
        if (ErrorMessage is not null)
        {
            ErrorMessage = null;
        }
        else
        {
            AdminState.ClearMessages();
        }
    }

    private string GetEditorLayoutClass()
    {
        return $"osirion-editor-layout osirion-admin-theme-{Theme} {Class}".Trim();
    }
}