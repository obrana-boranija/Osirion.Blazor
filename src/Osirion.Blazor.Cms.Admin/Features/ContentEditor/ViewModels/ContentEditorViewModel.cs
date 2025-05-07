using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Domain.Models;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;

public class ContentEditorViewModel
{
    private readonly ContentEditorService _editorService;
    private readonly IEventPublisher _eventPublisher;
    private readonly IEventSubscriber _eventSubscriber;

    public BlogPost? EditingPost { get; private set; }
    public bool IsCreatingNew { get; private set; }
    public bool IsSaving { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string FileName { get; set; } = string.Empty;
    public string CommitMessage { get; set; } = string.Empty;

    public event Action? StateChanged;

    public ContentEditorViewModel(
        ContentEditorService editorService,
        IEventPublisher eventPublisher,
        IEventSubscriber eventSubscriber)
    {
        _editorService = editorService;
        _eventPublisher = eventPublisher;
        _eventSubscriber = eventSubscriber;

        // Subscribe to events that affect the editor
        _eventSubscriber.Subscribe<ContentSelectedEvent>(HandleContentSelected);
        _eventSubscriber.Subscribe<CreateNewContentEvent>(HandleCreateNewContent);
    }

    public async Task LoadPostAsync(string path)
    {
        try
        {
            var blogPost = await _editorService.GetBlogPostAsync(path);
            EditingPost = blogPost;
            IsCreatingNew = false;
            CommitMessage = $"Update {Path.GetFileName(path)}";

            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load post: {ex.Message}";
            _eventPublisher.Publish(new ErrorOccurredEvent(ErrorMessage, ex));
        }
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
            // Update file path for new posts
            if (IsCreatingNew)
            {
                string filename = FileName.Trim();
                if (!filename.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                {
                    filename += ".md";
                }

                // Get the current directory from the editing post
                string directory = Path.GetDirectoryName(EditingPost.FilePath) ?? string.Empty;
                EditingPost.FilePath = string.IsNullOrEmpty(directory) ?
                    filename :
                    Path.Combine(directory, filename).Replace('\\', '/');
            }

            // Create commit message if empty
            if (string.IsNullOrEmpty(CommitMessage))
            {
                CommitMessage = IsCreatingNew
                    ? $"Create {Path.GetFileName(EditingPost.FilePath)}"
                    : $"Update {Path.GetFileName(EditingPost.FilePath)}";
            }

            // Save post
            var result = await _editorService.SaveContentAsync(EditingPost, CommitMessage);

            // Update post with new information if successful
            if (result != null)
            {
                EditingPost.Sha = result.Content.Sha;

                // Publish content saved event
                _eventPublisher.Publish(new ContentSavedEvent(EditingPost.FilePath));
                _eventPublisher.Publish(new StatusNotificationEvent(
                    $"File saved successfully: {Path.GetFileName(EditingPost.FilePath)}",
                    StatusType.Success));

                // Reset state for new posts
                if (IsCreatingNew)
                {
                    IsCreatingNew = false;
                    FileName = string.Empty;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save post: {ex.Message}";
            _eventPublisher.Publish(new ErrorOccurredEvent(ErrorMessage, ex));
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
        ErrorMessage = null;

        NotifyStateChanged();
    }

    private void HandleContentSelected(ContentSelectedEvent e)
    {
        LoadPostAsync(e.Path).ConfigureAwait(false);
    }

    private void HandleCreateNewContent(CreateNewContentEvent e)
    {
        // Create new post
        EditingPost = _editorService.CreateNewBlogPost(e.Directory);
        IsCreatingNew = true;

        // Generate suggested filename from title
        FileName = _editorService.GenerateFileNameFromTitle(EditingPost.Metadata.Title);

        NotifyStateChanged();
    }

    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}