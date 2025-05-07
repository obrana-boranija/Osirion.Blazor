using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Application.Commands;

public class SaveContentCommandHandler : ICommandHandler<SaveContentCommand, GitHubFileCommitResponse>
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly ILogger<SaveContentCommandHandler> _logger;

    public SaveContentCommandHandler(
        IContentRepositoryAdapter repositoryAdapter,
        ILogger<SaveContentCommandHandler> logger)
    {
        _repositoryAdapter = repositoryAdapter;
        _logger = logger;
    }

    public async Task<GitHubFileCommitResponse> HandleAsync(SaveContentCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Saving content: {Path}", command.Path);

            var response = await _repositoryAdapter.SaveContentAsync(
                command.Path,
                command.Content,
                command.CommitMessage,
                command.Sha);

            _logger.LogInformation("Content saved successfully: {Path}", command.Path);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving content: {Path}", command.Path);
            throw new ApplicationException($"Failed to save content: {ex.Message}", ex);
        }
    }
}