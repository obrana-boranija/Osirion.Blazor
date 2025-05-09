using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Components.Pages;

public partial class Dashboard
{
    /// <summary>
    /// Checks if both repository and branch are configured
    /// </summary>
    private bool IsRepositoryConfigured =>
        AdminState.SelectedRepository != null && AdminState.SelectedBranch != null;

    //protected override void OnInitialized()
    //{
    //    // Subscribe to state changes to trigger re-render
    //    AdminState.StateChanged += StateHasChanged;
    //}

    /// <summary>
    /// Creates a new post and navigates to edit screen
    /// </summary>
    private void CreateNewPost()
    {
        if (!IsRepositoryConfigured)
        {
            // Cannot create without repository and branch
            AdminState.SetErrorMessage("Please select a repository and branch before creating content.");
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
        NavigationManager.NavigateTo("/osirion/content/edit");
    }

    //public void Dispose()
    //{
    //    // Unsubscribe from state changes when component is disposed
    //    AdminState.StateChanged -= StateHasChanged;
    //}
}