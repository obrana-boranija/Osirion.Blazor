// src/Osirion.Blazor.Cms.Admin/Components/Editor/ContentEditor.razor.cs

using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Models;
using Markdig;

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
    public CmsAdminState AdminState { get; set; } = default!;

    [Inject]
    public IGitHubAdminService GitHubService { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

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

    // Use Markdig for better markdown rendering
    private readonly MarkdownPipeline _markdownPipeline;

    public ContentEditor()
    {
        _markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .Build();
    }

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
            CommitMessage = $"Update {AdminState.EditingPost.FilePath}";
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
                // Ensure the filename has .md extension
                string filename = FileName.Trim();
                if (!filename.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                {
                    filename += ".md";
                }

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
                (AdminState.IsCreatingNewFile ? $"Create {filePath}" : $"Update {filePath}") :
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