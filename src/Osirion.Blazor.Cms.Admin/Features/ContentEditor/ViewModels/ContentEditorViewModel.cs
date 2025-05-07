using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;

public class ContentEditorViewModel : IDisposable
{
    private readonly IContentEditorService _editorService;
    private readonly IEventPublisher _eventPublisher;
    private readonly IEventSubscriber _eventSubscriber;

    // State properties
    public BlogPost? EditingPost { get; private set; }
    public bool IsCreatingNew { get; private set; }
    public bool IsSaving { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string FileName { get; set; } = string.Empty;
    public string CommitMessage { get; set; } = string.Empty;

    // State changed event
    public event Action? StateChanged;

    public ContentEditorViewModel(
        IContentEditorService editorService,
        IEventPublisher eventPublisher,
        IEventSubscriber eventSubscriber)
    {
        _editorService = editorService;
        _eventPublisher = eventPublisher;
        _eventSubscriber = eventSubscriber;

        // Subscribe to content-related events
        _eventSubscriber.Subscribe<ContentSelectedEvent>(OnContentSelected);
        _eventSubscriber.Subscribe<CreateNewContentEvent>(OnCreateNewContent);
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
            NotifyStateChanged();
        }
    }

    public async Task SavePostAsync()
    {
        if (EditingPost == null)
            return;

        try
        {
            // Update state
            IsSaving = true;
            ErrorMessage = null;
            NotifyStateChanged();

            // Update file path for new posts
            if (IsCreatingNew)
            {
                string filename = FileName.Trim();
                if (!filename.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                {
                    filename += ".md";
                }

                // Combine directory and filename
                string directory = EditingPost.Directory;
                EditingPost.FilePath = string.IsNullOrEmpty(directory)
                    ? filename
                    : $"{directory}/{filename}";
            }

            // Create commit message if empty
            if (string.IsNullOrEmpty(CommitMessage))
            {
                CommitMessage = IsCreatingNew
                    ? $"Create {Path.GetFileName(EditingPost.FilePath)}"
                    : $"Update {Path.GetFileName(EditingPost.FilePath)}";
            }

            // Save post
            var result = await _editorService.SaveBlogPostAsync(EditingPost, CommitMessage);

            // Update post with new SHA
            if (result != null)
            {
                EditingPost.Sha = result.Content.Sha;

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

    public void UpdateContent(string content)
    {
        if (EditingPost != null)
        {
            EditingPost.Content = content;
            NotifyStateChanged();
        }
    }

    public void UpdateMetadata(FrontMatter metadata)
    {
        if (EditingPost != null)
        {
            EditingPost.Metadata = metadata;
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

    private void OnContentSelected(ContentSelectedEvent e)
    {
        LoadPostAsync(e.Path).ConfigureAwait(false);
    }

    private void OnCreateNewContent(CreateNewContentEvent e)
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

    public void Dispose()
    {
        // Unsubscribe from events
        _eventSubscriber.Unsubscribe<ContentSelectedEvent>(OnContentSelected);
        _eventSubscriber.Unsubscribe<CreateNewContentEvent>(OnCreateNewContent);
    }
}