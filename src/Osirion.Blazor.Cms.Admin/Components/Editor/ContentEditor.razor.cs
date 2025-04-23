using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Models;

namespace Osirion.Blazor.Cms.Admin.Components.Editor;

public partial class ContentEditor
{
    [Parameter]
    public bool IsMetadataPanelVisible { get; set; } = true;

    [Parameter]
    public bool IsPreviewVisible { get; set; } = true;

    [Parameter]
    public bool AskForCommitMessage { get; set; } = true;

    [Parameter]
    public EventCallback<BlogPost> OnSaveComplete { get; set; }

    [Parameter]
    public EventCallback OnDiscard { get; set; }

    [Inject]
    public CmsAdminState AdminState { get; set; }

    [Inject]
    public IGitHubAdminService GitHubService { get; set; }

    private bool IsSaving { get; set; }
    private string? ErrorMessage { get; set; }

    private string FileName { get; set; } = string.Empty;
    private string CommitMessage
    {
        get
        {
            if (!AskForCommitMessage)
            {
                return string.Empty;
            }

            return _commitMessage;
        }
        set => _commitMessage = value;
    }
    private string _commitMessage = "Update content";

    private bool IsFileName => AdminState.IsCreatingNewFile || string.IsNullOrEmpty(AdminState.EditingPost?.FilePath);

    protected override void OnInitialized()
    {
        AdminState.StateChanged += StateHasChanged;

        if (AdminState.EditingPost != null && AdminState.IsCreatingNewFile)
        {
            // Generate filename from post title
            FileName = GenerateFileName(AdminState.EditingPost.Metadata.Title);
        }
    }

    protected override void OnParametersSet()
    {
        if (AdminState.EditingPost != null && AdminState.IsCreatingNewFile)
        {
            // Generate filename from post title if not set
            if (string.IsNullOrEmpty(FileName))
            {
                FileName = GenerateFileName(AdminState.EditingPost.Metadata.Title);
            }

            // Set default commit message
            CommitMessage = "Create new file";
        }
        else if (AdminState.EditingPost != null && !AdminState.IsCreatingNewFile)
        {
            // Set default commit message for existing file
            CommitMessage = "Update content";
        }
    }

    public void Dispose()
    {
        AdminState.StateChanged -= StateHasChanged;
    }

    private void ToggleMetadataPanel()
    {
        IsMetadataPanelVisible = !IsMetadataPanelVisible;
    }

    private void TogglePreview()
    {
        IsPreviewVisible = !IsPreviewVisible;
    }

    private async Task SaveChanges()
    {
        if (AdminState.EditingPost == null)
        {
            return;
        }

        IsSaving = true;
        AdminState.SetSaving(true);
        ErrorMessage = null;

        try
        {
            string filePath;
            string? existingSha = null;

            if (AdminState.IsCreatingNewFile)
            {
                // Construct full path for new file
                var filename = $"{FileName.Trim()}.md";
                filePath = string.IsNullOrEmpty(AdminState.CurrentPath) ?
                    filename :
                    $"{AdminState.CurrentPath}/{filename}";
            }
            else
            {
                // Use existing file path
                filePath = AdminState.EditingPost.FilePath;
                existingSha = AdminState.EditingPost.Sha;
            }

            // Get full content with frontmatter
            var fullContent = AdminState.EditingPost.ToMarkdown();

            // Commit message
            var message = string.IsNullOrEmpty(CommitMessage) ?
                (AdminState.IsCreatingNewFile ? "Create new file" : "Update content") :
                CommitMessage;

            // Save the file
            var response = await GitHubService.CreateOrUpdateFileAsync(
                filePath,
                fullContent,
                message,
                existingSha);

            // Update the blog post with new information
            AdminState.EditingPost.FilePath = response.Content.Path;
            AdminState.EditingPost.Sha = response.Content.Sha;

            // Reset state
            if (OnSaveComplete.HasDelegate)
            {
                await OnSaveComplete.InvokeAsync(AdminState.EditingPost);
            }

            // Show success message
            AdminState.SetStatusMessage($"File saved successfully: {filePath}");

            // Reset file name for new files
            if (AdminState.IsCreatingNewFile)
            {
                FileName = string.Empty;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save file: {ex.Message}";
            AdminState.SetErrorMessage(ErrorMessage);
        }
        finally
        {
            IsSaving = false;
            AdminState.SetSaving(false);
        }
    }

    private async Task DiscardChanges()
    {
        if (OnDiscard.HasDelegate)
        {
            await OnDiscard.InvokeAsync();
        }

        AdminState.ClearEditing();
        FileName = string.Empty;
        CommitMessage = string.Empty;
    }

    private string GenerateFileName(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return "new-post";
        }

        // Generate a slug from the title
        return title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("?", "")
            .Replace("!", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("/", "-")
            .Replace("\\", "-");
    }

    private string RenderMarkdown(string markdown)
    {
        // Use the Markdig library to render markdown to HTML
        // For simplicity in this sample, we'll use a basic replacement

        // In a real implementation, you would use a proper markdown parser
        return markdown
            .Replace("# ", "<h1>").Replace("\n# ", "<h1>")
            .Replace("## ", "<h2>").Replace("\n## ", "<h2>")
            .Replace("### ", "<h3>").Replace("\n### ", "<h3>")
            .Replace("#### ", "<h4>").Replace("\n#### ", "<h4>")
            .Replace("##### ", "<h5>").Replace("\n##### ", "<h5>")
            .Replace("###### ", "<h6>").Replace("\n###### ", "<h6>")
            .Replace("**", "<strong>").Replace("**", "</strong>")
            .Replace("*", "<em>").Replace("*", "</em>")
            .Replace("\n", "<br />");
    }

    private void InsertHeading()
    {
        if (AdminState.EditingPost == null)
        {
            return;
        }

        AdminState.EditingPost.Content += "\n## New Heading";
    }

    private void InsertBold()
    {
        if (AdminState.EditingPost == null)
        {
            return;
        }

        AdminState.EditingPost.Content += "**bold text**";
    }

    private void InsertItalic()
    {
        if (AdminState.EditingPost == null)
        {
            return;
        }

        AdminState.EditingPost.Content += "*italic text*";
    }

    private void InsertLink()
    {
        if (AdminState.EditingPost == null)
        {
            return;
        }

        AdminState.EditingPost.Content += "[link text](https://example.com)";
    }

    private void InsertImage()
    {
        if (AdminState.EditingPost == null)
        {
            return;
        }

        AdminState.EditingPost.Content += "![alt text](https://example.com/image.jpg)";
    }

    private void InsertList()
    {
        if (AdminState.EditingPost == null)
        {
            return;
        }

        AdminState.EditingPost.Content += "\n- Item 1\n- Item 2\n- Item 3";
    }

    private void InsertCodeBlock()
    {
        if (AdminState.EditingPost == null)
        {
            return;
        }

        AdminState.EditingPost.Content += "\n```\ncode goes here\n```";
    }

    private string GetEditorLayoutClass()
    {
        string layoutClass = "";

        if (IsMetadataPanelVisible && IsPreviewVisible)
        {
            layoutClass = "three-panel";
        }
        else if (IsMetadataPanelVisible)
        {
            layoutClass = "with-metadata";
        }
        else if (IsPreviewVisible)
        {
            layoutClass = "with-preview";
        }

        return layoutClass;
    }

    private string GetContentEditorClass()
    {
        return $"osirion-admin-content-editor {CssClass}".Trim();
    }
}
