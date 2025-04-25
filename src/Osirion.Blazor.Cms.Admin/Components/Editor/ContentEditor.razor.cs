using Markdig;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Models;

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
        try
        {
            // Use Markdig properly with the pipeline to render markdown to HTML
            return Markdown.ToHtml(markdown, _markdownPipeline);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error rendering markdown: {ex.Message}");

            // Fallback to basic formatting if Markdig fails
            return markdown
                .Replace("# ", "<h1>").Replace("\n# ", "\n<h1>").Replace("</h1>", "</h1>\n")
                .Replace("## ", "<h2>").Replace("\n## ", "\n<h2>").Replace("</h2>", "</h2>\n")
                .Replace("### ", "<h3>").Replace("\n### ", "\n<h3>").Replace("</h3>", "</h3>\n")
                .Replace("#### ", "<h4>").Replace("\n#### ", "\n<h4>").Replace("</h4>", "</h4>\n")
                .Replace("**", "<strong>").Replace("**", "</strong>")
                .Replace("*", "<em>").Replace("*", "</em>")
                .Replace("\n", "<br />");
        }
    }

    private void InsertHeading()
    {
        if (adminState.EditingPost == null)
        {
            return;
        }

        adminState.EditingPost.Content += "\n## New Heading";
    }

    private void InsertBold()
    {
        if (adminState.EditingPost == null)
        {
            return;
        }

        adminState.EditingPost.Content += "**bold text**";
    }

    private void InsertItalic()
    {
        if (adminState.EditingPost == null)
        {
            return;
        }

        adminState.EditingPost.Content += "*italic text*";
    }

    private void InsertLink()
    {
        if (adminState.EditingPost == null)
        {
            return;
        }

        adminState.EditingPost.Content += "[link text](https://example.com)";
    }

    private void InsertImage()
    {
        if (adminState.EditingPost == null)
        {
            return;
        }

        adminState.EditingPost.Content += "![alt text](https://example.com/image.jpg)";
    }

    private void InsertList()
    {
        if (adminState.EditingPost == null)
        {
            return;
        }

        adminState.EditingPost.Content += "\n- Item 1\n- Item 2\n- Item 3";
    }

    private void InsertCodeBlock()
    {
        if (adminState.EditingPost == null)
        {
            return;
        }

        adminState.EditingPost.Content += "\n```\ncode goes here\n```";
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