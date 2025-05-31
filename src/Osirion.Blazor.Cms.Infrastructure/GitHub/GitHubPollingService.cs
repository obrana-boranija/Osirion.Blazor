using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Background service that periodically polls GitHub repositories for changes
/// </summary>
public class GitHubPollingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<GitHubPollingService> _logger;
    private readonly GitHubOptions _options;
    private readonly Dictionary<string, string> _lastKnownShas = new();

    public GitHubPollingService(
        IServiceProvider serviceProvider,
        IOptions<GitHubOptions> options,
        ILogger<GitHubPollingService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Don't start polling if polling is disabled
        if (!_options.EnablePolling)
        {
            _logger.LogInformation("GitHub polling is disabled. Background service will not run.");
            return;
        }

        _logger.LogInformation("GitHub polling service started. Polling interval: {Interval} seconds",
            _options.PollingIntervalSeconds);

        // Set up a timer to check for changes periodically
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.PollingIntervalSeconds));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await CheckForChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for GitHub repository changes");
            }
        }
    }

    /// <summary>
    /// Checks for changes in GitHub repositories and updates content cache if needed
    /// </summary>
    private async Task CheckForChangesAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();

        try
        {
            var providerManager = scope.ServiceProvider.GetRequiredService<IContentProviderManager>();
            var apiClient = scope.ServiceProvider.GetRequiredService<IGitHubApiClient>();

            // Get all GitHub-based content providers
            var providers = providerManager.GetAllProviders()
                .OfType<IContentCacheUpdater>()
                .Where(p => p.ProviderId.Contains("github"))
                .ToList();

            if (!providers.Any())
            {
                _logger.LogDebug("No GitHub content providers found");
                return;
            }

            foreach (var provider in providers)
            {
                // Skip if provider ID doesn't match our configured provider
                if (!provider.ProviderId.Contains(_options.ProviderId ?? $"github-{_options.Owner}-{_options.Repository}"))
                {
                    continue;
                }

                try
                {
                    // Get the latest commit SHA for the branch
                    var latestSha = await GetLatestCommitShaAsync(apiClient, stoppingToken);
                    if (string.IsNullOrWhiteSpace(latestSha))
                    {
                        continue;
                    }

                    // Check if the SHA has changed
                    if (!_lastKnownShas.TryGetValue(provider.ProviderId, out var lastSha) || lastSha != latestSha)
                    {
                        _logger.LogInformation("GitHub repository changed for provider {ProviderId}. " +
                            "Previous SHA: {PreviousSha}, New SHA: {NewSha}",
                            provider.ProviderId, lastSha ?? "none", latestSha);

                        // Update the cache
                        await provider.UpdateCacheAsync(latestSha);

                        // Update the last known SHA
                        _lastKnownShas[provider.ProviderId] = latestSha;

                        _logger.LogInformation("Updated cache for provider: {ProviderId}", provider.ProviderId);
                    }
                    else
                    {
                        _logger.LogDebug("No changes detected for provider: {ProviderId}", provider.ProviderId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking for changes in provider: {ProviderId}", provider.ProviderId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during GitHub polling");
        }
    }

    /// <summary>
    /// Gets the latest commit SHA for the configured branch
    /// </summary>
    private async Task<string?> GetLatestCommitShaAsync(IGitHubApiClient apiClient, CancellationToken cancellationToken)
    {
        try
        {
            // Get the branch information
            var branches = await apiClient.GetBranchesAsync(cancellationToken);
            var branch = branches.FirstOrDefault(b => b.Name == _options.Branch);

            if (branch is null)
            {
                _logger.LogWarning("Branch {Branch} not found in repository {Owner}/{Repository}",
                    _options.Branch, _options.Owner, _options.Repository);
                return null;
            }

            return branch.Commit.Sha;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting latest commit SHA for branch {Branch} in repository {Owner}/{Repository}",
                _options.Branch, _options.Owner, _options.Repository);
            return null;
        }
    }
}