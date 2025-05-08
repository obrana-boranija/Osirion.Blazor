using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Example.Components.Pages.CmsAdmin;

public partial class Dashboard(NavigationManager navigationManager) : OsirionComponentBase
{
    [Inject]
    private CmsState AdminState { get; set; } = default!;

    [Inject]
    private IEventPublisher EventPublisher { get; set; } = default!;

    /// <summary>
    /// Checks if both repository and branch are configured
    /// </summary>
    private bool IsRepositoryConfigured =>
        AdminState.SelectedRepository != null && AdminState.SelectedBranch != null;

    protected override void OnInitialized()
    {
        // Subscribe to state changes to trigger re-render
        AdminState.StateChanged += StateHasChanged;
    }

    /// <summary>
    /// Creates a new post and navigates to edit screen
    /// </summary>
    private void CreateNewPost()
    {
        if (!IsRepositoryConfigured)
        {
            // Cannot create without repository and branch
            return;
        }

        // Create new blog post with empty content
        var newPost = new BlogPost
        {
            Metadata = FrontMatter.Create("New Post", "Enter description here", DateTime.Now),
            Content = "## New Post\n\nStart writing your content here...",
            FilePath = string.IsNullOrEmpty(AdminState.CurrentPath) ?
                "new-post.md" :
                $"{AdminState.CurrentPath}/new-post.md"
        };

        // Update state and publish event
        AdminState.SetEditingPost(newPost, true);
        EventPublisher.Publish(new CreateNewContentEvent(AdminState.CurrentPath));

        // Navigate to edit page
        navigationManager.NavigateTo("/admin/content/edit");
    }

    public void Dispose()
    {
        // Unsubscribe from state changes when component is disposed
        AdminState.StateChanged -= StateHasChanged;
    }
}