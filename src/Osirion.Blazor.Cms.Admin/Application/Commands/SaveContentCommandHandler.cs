using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Application.Commands;

namespace Osirion.Blazor.Cms.Admin.Application.Commands;

public class SaveContentCommandHandler : ICommandHandler<SaveContentCommand, SaveContentResult>
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

    public async Task<SaveContentResult> HandleAsync(SaveContentCommand command, CancellationToken cancellationToken)
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

            return new SaveContentResult
            {
                IsSuccess = true,
                ContentId = response.Content.Path,
                Sha = response.Content.Sha,
                Message = "Content saved successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving content: {Path}", command.Path);

            return new SaveContentResult
            {
                IsSuccess = false,
                Message = $"Failed to save content: {ex.Message}"
            };
        }
    }
}