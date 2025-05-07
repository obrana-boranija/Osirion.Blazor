using Osirion.Blazor.Cms.Admin.Application.Commands;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Services.State;
using Osirion.Blazor.Cms.Application.Commands;
using Osirion.Blazor.Cms.Application.Queries;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;

public class ContentEditorViewModel
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
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
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        CmsApplicationState appState,
        CmsEventMediator eventMediator)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _appState = appState;
        _eventMediator = eventMediator;
    }

    public async Task LoadPostAsync(string path)
    {
        try
        {
            var query = new GetBlogPostQuery { Path = path };
            EditingPost = await _queryDispatcher.DispatchAsync<GetBlogPostQuery, BlogPost>(query);

            IsCreatingNew = false;
            CommitMessage = $"Update {Path.GetFileName(path)}";

            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load post: {ex.Message}";
            _appState.SetErrorMessage(ErrorMessage);
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

                EditingPost.FilePath = string.IsNullOrEmpty(_appState.CurrentPath) ?
                    filename :
                    $"{_appState.CurrentPath}/{filename}";
            }

            // Create command
            var command = new SaveContentCommand
            {
                Path = EditingPost.FilePath,
                Content = EditingPost.ToMarkdown(),
                CommitMessage = string.IsNullOrEmpty(CommitMessage) ?
                    (IsCreatingNew ? $"Create {EditingPost.FilePath}" : $"Update {EditingPost.FilePath}") :
                    CommitMessage,
                Sha = EditingPost.Sha
            };

            // Dispatch command
            var response = await _commandDispatcher.DispatchAsync<SaveContentCommand, GitHubFileCommitResponse>(command);

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

    // Other methods...

    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}