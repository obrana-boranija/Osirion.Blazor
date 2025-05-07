using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Services.State;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.Extensions;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;

public class ContentEditorViewModel
{
    private readonly ContentEditorService _editorService;
    private readonly CmsApplicationState _appState;
    private readonly CmsEventMediator _eventMediator;

    public BlogPost? EditingPost { get; private set; }
    public bool IsCreatingNew { get; private set; }
    public bool IsSaving { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string FileName { get; set; } = string.Empty;
    public string CommitMessage { get; set; } = string.Empty;

    public event Action? StateChanged;

    public ContentEditorViewModel(
        ContentEditorService editorService,
        CmsApplicationState appState,
        CmsEventMediator eventMediator)
    {
        _editorService = editorService;
        _appState = appState;
        _eventMediator = eventMediator;

        // Subscribe to content selected events
        _eventMediator.Subscribe<ContentSelectedEvent>(OnContentSelected);
    }

    public async Task LoadPostAsync(string path)
    {
        try
        {
            EditingPost = await _editorService.GetBlogPostAsync(path);
            IsCreatingNew = false;

            // Set default commit message
            CommitMessage = $"Update {System.IO.Path.GetFileName(path)}";

            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load post: {ex.Message}";
            _appState.SetErrorMessage(ErrorMessage);
        }
    }

    public void CreateNewPost()
    {
        var newPost = new BlogPost
        {
            Metadata = FrontMatter.Create("New Post", "Enter description here", DateTime.Now),
            Content = "## New Post\n\nStart writing your content here...",
            FilePath = string.IsNullOrEmpty(_appState.CurrentPath) ?
                "new-post.md" :
                $"{_appState.CurrentPath}/new-post.md"
        };

        EditingPost = newPost;
        IsCreatingNew = true;

        // Set default values
        FileName = GenerateFileName(newPost.Metadata.Title);
        CommitMessage = "Create new file";

        NotifyStateChanged();
    }

    public async Task SavePostAsync()
    {
        if (EditingPost == null)
            return;

        IsSaving = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            // For new posts, update the file path with the specified filename
            if (IsCreatingNew)
            {
                string filename = FileName.Trim();
                if (!filename.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                {
                    filename += ".md";
                }

                EditingPost.FilePath = string.IsNullOrEmpty(_appState.CurrentPath) ?
                    filename :
                    $"{_appState.CurrentPath}/{filename}";
            }

            var response = await _editorService.SaveContentAsync(EditingPost, CommitMessage);

            // Update post with new information
            EditingPost.FilePath = response.Content.Path;
            EditingPost.Sha = response.Content.Sha;

            // Publish content saved event
            _eventMediator.Publish(new ContentSavedEvent(EditingPost.FilePath));

            _appState.SetStatusMessage($"File saved successfully: {EditingPost.FilePath}");

            // Reset state for new posts
            if (IsCreatingNew)
            {
                IsCreatingNew = false;
                FileName = string.Empty;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save post: {ex.Message}";
            _appState.SetErrorMessage(ErrorMessage);
        }
        finally
        {
            IsSaving = false;
            NotifyStateChanged();
        }
    }

    public void DiscardChanges()
    {
        EditingPost = null;
        IsCreatingNew = false;
        FileName = string.Empty;
        CommitMessage = string.Empty;

        NotifyStateChanged();
    }

    private void OnContentSelected(ContentSelectedEvent e)
    {
        if (e.Item.IsMarkdownFile)
        {
            _ = LoadPostAsync(e.Item.Path);
        }
    }

    private string GenerateFileName(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return "new-post";
        }

        return title.ToUrlSlug();
    }

    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}