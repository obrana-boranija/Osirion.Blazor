using System.Collections.Concurrent;
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
/// <remarks>
/// The provider of the single-provider registration (<c>Osirion:Cms:GitHub</c>) polls with the injected
/// <see cref="GitHubOptions"/>. Each provider registered under <c>Osirion:Cms:Web:GitHub:&lt;Name&gt;</c> or
/// <c>Osirion:Cms:Admin:GitHub:&lt;Name&gt;</c> polls with its own <see cref="GitHubOptions.EnablePolling"/> and
/// <see cref="GitHubOptions.PollingIntervalSeconds"/>.
/// </remarks>
public class GitHubPollingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<GitHubPollingService> _logger;
    private readonly GitHubOptions _options;
    private readonly IReadOnlyList<GitHubPollingTarget> _namedTargets;
    private readonly ConcurrentDictionary<string, string> _lastKnownShas = new();

    /// <summary>Creates the polling service.</summary>
    /// <param name="serviceProvider">The provider used to create a scope for each polling pass.</param>
    /// <param name="options">The options of the single-provider registration.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="namedTargets">The providers registered by name, each polled with its own options.</param>
    public GitHubPollingService(
        IServiceProvider serviceProvider,
        IOptions<GitHubOptions> options,
        ILogger<GitHubPollingService> logger,
        IEnumerable<GitHubPollingTarget>? namedTargets = null)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _namedTargets = (namedTargets ?? []).DistinctBy(target => target.ProviderName, StringComparer.Ordinal).ToList();
    }

    /// <summary>Performs the Execute operation asynchronously.</summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var targets = new List<GitHubPollingTarget>();
        if (_options.EnablePolling)
        {
            targets.Add(new GitHubPollingTarget(string.Empty, _options));
        }

        targets.AddRange(_namedTargets.Where(target => target.Options.EnablePolling));

        // Don't start polling if polling is disabled everywhere
        if (targets.Count == 0)
        {
            _logger.LogInformation("GitHub polling is disabled. Background service will not run.");
            return;
        }

        await Task.WhenAll(targets.Select(target => PollAsync(target, stoppingToken)));
    }

    private async Task PollAsync(GitHubPollingTarget target, CancellationToken stoppingToken)
    {
        var options = target.Options;
        _logger.LogInformation("GitHub polling service started for {Owner}/{Repository}. Polling interval: {Interval} seconds",
            options.Owner, options.Repository, options.PollingIntervalSeconds);

        // Set up a timer to check for changes periodically
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(options.PollingIntervalSeconds));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await CheckForChangesAsync(target, stoppingToken);
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
    /// <remarks>Internal so one polling pass can be tested without waiting for the timer.</remarks>
    internal Task CheckForChangesAsync(CancellationToken stoppingToken)
    {
        return CheckForChangesAsync(new GitHubPollingTarget(string.Empty, _options), stoppingToken);
    }

    private async Task CheckForChangesAsync(GitHubPollingTarget target, CancellationToken stoppingToken)
    {
        var options = target.Options;
        using var scope = _serviceProvider.CreateScope();

        try
        {
            var providerManager = scope.ServiceProvider.GetRequiredService<IContentProviderManager>();
            // A named provider has its own repository, so its client comes from the factory.
            var apiClient = string.IsNullOrEmpty(target.ProviderName)
                ? scope.ServiceProvider.GetRequiredService<IGitHubApiClient>()
                : scope.ServiceProvider.GetRequiredService<IGitHubApiClientFactory>().GetClient(target.ProviderName);

            // Exact match: a provider for another repository can share a prefix with this one (github-owner-repo2).
            var configuredProviderId = GitHubProviderId.For(options);
            var providers = providerManager.GetAllProviders()
                .OfType<IContentCacheUpdater>()
                .Where(p => string.Equals(p.ProviderId, configuredProviderId, StringComparison.Ordinal))
                .ToList();

            if (!providers.Any())
            {
                _logger.LogDebug("No GitHub content providers found");
                return;
            }

            foreach (var provider in providers)
            {
                try
                {
                    // Get the latest commit SHA for the branch
                    var latestSha = await GetLatestCommitShaAsync(apiClient, options, stoppingToken);
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
    private async Task<string?> GetLatestCommitShaAsync(IGitHubApiClient apiClient, GitHubOptions options, CancellationToken cancellationToken)
    {
        try
        {
            // Get the branch information
            var branches = await apiClient.GetBranchesAsync(cancellationToken);
            var branch = branches?.FirstOrDefault(b => b.Name == options.Branch);

            if (branch is null)
            {
                _logger.LogWarning("Branch {Branch} not found in repository {Owner}/{Repository}",
                    options.Branch, options.Owner, options.Repository);
                return null;
            }

            return branch.Commit.Sha;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting latest commit SHA for branch {Branch} in repository {Owner}/{Repository}",
                options.Branch, options.Owner, options.Repository);
            return null;
        }
    }
}
