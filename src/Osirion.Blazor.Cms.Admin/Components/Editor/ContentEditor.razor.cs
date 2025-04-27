using Markdig;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Infrastructure.Extensions;

namespace Osirion.Blazor.Cms.Admin.Components.Editor;

public partial class ContentEditor(CmsAdminState adminState, IGitHubAdminService gitHubService)
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

    private bool IsSaving { get; set; }
    private string? ErrorMessage { get; set; }

    private MarkdownEditorPreview? EditorPreviewRef;

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

    private bool IsFileName => adminState.IsCreatingNewFile || string.IsNullOrEmpty(adminState.EditingPost?.FilePath);

    // Use Markdig for better markdown rendering
    private MarkdownPipeline _markdownPipeline = default!;

    protected override void OnInitialized()
    {
        adminState.StateChanged += StateHasChanged;

        _markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .Build();

        if (adminState.EditingPost != null && adminState.IsCreatingNewFile)
        {
            // Generate filename from post title
            FileName = GenerateFileName(adminState.EditingPost.Metadata.Title);
        }
    }

    protected override void OnParametersSet()
    {
        if (adminState.EditingPost != null && adminState.IsCreatingNewFile)
        {
            // Generate filename from post title if not set
            if (string.IsNullOrEmpty(FileName))
            {
                FileName = GenerateFileName(adminState.EditingPost.Metadata.Title);
            }

            // Set default commit message
            CommitMessage = "Create new file";
        }
        else if (adminState.EditingPost != null && !adminState.IsCreatingNewFile)
        {
            // Set default commit message for existing file
            CommitMessage = $"Update {adminState.EditingPost.FilePath}";
        }
    }

    public void Dispose()
    {
        adminState.StateChanged -= StateHasChanged;
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
        if (adminState.EditingPost == null)
        {
            return;
        }

        IsSaving = true;
        adminState.SetSaving(true);
        ErrorMessage = null;

        try
        {
            string filePath;
            string? existingSha = null;

            if (adminState.IsCreatingNewFile)
            {
                // Construct full path for new file
                // Ensure the filename has .md extension
                string filename = FileName.Trim();
                if (!filename.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                {
                    filename += ".md";
                }

                filePath = string.IsNullOrEmpty(adminState.CurrentPath) ?
                    filename :
                    $"{adminState.CurrentPath}/{filename}";
            }
            else
            {
                // Use existing file path
                filePath = adminState.EditingPost.FilePath;
                existingSha = adminState.EditingPost.Sha;
            }

            // Get full content with frontmatter
            var fullContent = adminState.EditingPost.ToMarkdown();

            // Commit message
            var message = string.IsNullOrEmpty(CommitMessage) ?
                (adminState.IsCreatingNewFile ? $"Create {filePath}" : $"Update {filePath}") :
                CommitMessage;

            // Save the file
            var response = await gitHubService.CreateOrUpdateFileAsync(
                filePath,
                fullContent,
                message,
                existingSha);

            // Update the blog post with new information
            adminState.EditingPost.FilePath = response.Content.Path;
            adminState.EditingPost.Sha = response.Content.Sha;

            // Reset state
            if (OnSaveComplete.HasDelegate)
            {
                await OnSaveComplete.InvokeAsync(adminState.EditingPost);
            }

            // Show success message
            adminState.SetStatusMessage($"File saved successfully: {filePath}");

            // Reset file name for new files
            if (adminState.IsCreatingNewFile)
            {
                FileName = string.Empty;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save file: {ex.Message}";
            adminState.SetErrorMessage(ErrorMessage);
        }
        finally
        {
            IsSaving = false;
            adminState.SetSaving(false);
        }
    }

    private async Task DiscardChanges()
    {
        if (OnDiscard.HasDelegate)
        {
            await OnDiscard.InvokeAsync();
        }

        adminState.ClearEditing();
        FileName = string.Empty;
        CommitMessage = string.Empty;
    }

    private string GenerateFileName(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return "new-".ToUrlSlug(10);
        }

        // Generate a slug from the title
        return title.ToUrlSlug();
    }

    private string GetContentEditorClass()
    {
        return $"osirion-admin-content-editor {CssClass}".Trim();
    }
}