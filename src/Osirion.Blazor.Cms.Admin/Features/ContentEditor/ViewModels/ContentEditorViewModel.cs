using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;

/// <summary>Coordinates content editing state and persistence operations.</summary>
public class ContentEditorViewModel : IDisposable
{
    private readonly IContentEditorService _editorService;
    private readonly IEventPublisher _eventPublisher;
    private readonly IEventSubscriber _eventSubscriber;

    // State properties
    /// <summary>Gets the post currently being edited.</summary>
    public ContentItem? EditingPost { get; private set; }
    /// <summary>Gets whether a new post is being created.</summary>
    public bool IsCreatingNew { get; private set; }
    /// <summary>Gets whether a save operation is in progress.</summary>
    public bool IsSaving { get; private set; }
    /// <summary>Gets the current error message.</summary>
    public string? ErrorMessage { get; private set; }
    /// <summary>Gets or sets the file name for a new post.</summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>Gets or sets the commit message.</summary>
    public string CommitMessage { get; set; } = string.Empty;

    // State changed event
    /// <summary>Occurs when the view-model state changes.</summary>
    public event Action? StateChanged;

    /// <summary>Initializes a new content editor view-model.</summary>
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

    // New method to initialize from AdminState
    /// <summary>Initializes the editor from an existing state.</summary>
    public void InitializeFromState(ContentItem post, bool isCreatingNew)
    {
        EditingPost = post;
        IsCreatingNew = isCreatingNew;

        if (isCreatingNew)
        {
            FileName = _editorService.GenerateFileNameFromTitle(post.Metadata?.Title ?? "new-document-name");
            CommitMessage = $"Create {FileName}";
        }
        else
        {
            CommitMessage = $"Update {Path.GetFileName(post.Path)}";
        }

        NotifyStateChanged();
    }

    /// <summary>Loads a post from the configured content provider.</summary>
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

    /// <summary>Saves the currently edited post.</summary>
    public async Task SavePostAsync()
    {
        if (EditingPost is null)
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
                string directory = Path.GetDirectoryName(EditingPost.Path) ?? string.Empty;
                EditingPost.Path = string.IsNullOrWhiteSpace(directory)
                    ? filename
                    : $"{directory}/{filename}";
            }

            // Create commit message if empty
            if (string.IsNullOrWhiteSpace(CommitMessage))
            {
                CommitMessage = IsCreatingNew
                    ? $"Create {Path.GetFileName(EditingPost.Path)}"
                    : $"Update {Path.GetFileName(EditingPost.Path)}";
            }

            // Save post
            var result = await _editorService.SaveBlogPostAsync(EditingPost, CommitMessage);

            // Update post with new SHA
            if (result is not null)
            {
                EditingPost.Sha = result.Content.Sha;

                // Reset state for new posts
                if (IsCreatingNew)
                {
                    IsCreatingNew = false;
                    FileName = string.Empty;
                }
            }

            // Publish saved event
            _eventPublisher.Publish(new ContentSavedEvent(EditingPost.Path));

            // Show success message
            _eventPublisher.Publish(new StatusNotificationEvent(
                $"Saved {Path.GetFileName(EditingPost.Path)} successfully.",
                StatusType.Success));
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

    /// <summary>Updates the content of the edited post.</summary>
    public void UpdateContent(string content)
    {
        if (EditingPost is not null)
        {
            EditingPost.Content = content;
            NotifyStateChanged();
        }
    }

    /// <summary>Updates the front matter of the edited post.</summary>
    public void UpdateMetadata(FrontMatter metadata)
    {
        if (EditingPost is not null)
        {
            EditingPost.Metadata = metadata;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Updates the SEO metadata of the editing post
    /// </summary>
    public void UpdateSeoMetadata(SeoMetadata seoMetadata)
    {
        if (EditingPost is not null)
        {
            EditingPost.SetSeoMetadata(seoMetadata);
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Reloads the current post from source
    /// </summary>
    public async Task ReloadPostAsync()
    {
        if (EditingPost is not null && !string.IsNullOrWhiteSpace(EditingPost.Path))
        {
            await LoadPostAsync(EditingPost.Path);
        }
    }

    /// <summary>Discards the current editing changes.</summary>
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
        FileName = _editorService.GenerateFileNameFromTitle(EditingPost.Metadata?.Title ?? "new-document-name");

        NotifyStateChanged();
    }

    /// <summary>Performs the NotifyStateChanged operation.</summary>
    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }

    /// <summary>Releases event subscriptions held by the view-model.</summary>
    public void Dispose()
    {
        // Unsubscribe from events
        _eventSubscriber.Unsubscribe<ContentSelectedEvent>(OnContentSelected);
        _eventSubscriber.Unsubscribe<CreateNewContentEvent>(OnCreateNewContent);
    }
}
