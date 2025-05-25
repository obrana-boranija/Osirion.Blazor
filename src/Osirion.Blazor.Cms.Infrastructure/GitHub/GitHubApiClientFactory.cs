using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using System.Collections.Concurrent;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Factory for creating GitHub API client instances for different providers
/// </summary>
public class GitHubApiClientFactory : IGitHubApiClientFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GitHubApiClientFactory> _logger;
    private readonly ConcurrentDictionary<string, IGitHubApiClient> _clients = new();
    private readonly Dictionary<string, GitHubProviderOptions> _webProviders = new();
    private readonly Dictionary<string, GitHubProviderOptions> _adminProviders = new();
    private string? _defaultWebProvider;
    private string? _defaultAdminProvider;

    public GitHubApiClientFactory(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<GitHubApiClientFactory> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        LoadConfiguration();
    }

    private void LoadConfiguration()
    {
        // Load Web providers
        var webSection = _configuration.GetSection("Osirion:Cms:Web:GitHub");
        if (webSection.Exists())
        {
            foreach (var providerSection in webSection.GetChildren())
            {
                var providerOptions = new GitHubProviderOptions();
                providerSection.Bind(providerOptions);
                providerOptions.Name = providerSection.Key;

                // Log the loaded configuration for debugging
                _logger.LogDebug("Loaded Web provider '{Name}': Owner={Owner}, Repository={Repository}, Branch={Branch}",
                    providerOptions.Name, providerOptions.Owner, providerOptions.Repository, providerOptions.Branch);

                _webProviders[providerSection.Key] = providerOptions;

                if (providerOptions.IsDefault)
                {
                    _defaultWebProvider = providerSection.Key;
                }
            }
        }

        // Load Admin providers
        var adminSection = _configuration.GetSection("Osirion:Cms:Admin:GitHub");
        if (adminSection.Exists())
        {
            foreach (var providerSection in adminSection.GetChildren())
            {
                var providerOptions = new GitHubProviderOptions();
                providerSection.Bind(providerOptions);
                providerOptions.Name = providerSection.Key;

                // Log the loaded configuration for debugging
                _logger.LogDebug("Loaded Admin provider '{Name}': Owner={Owner}, Repository={Repository}, Branch={Branch}",
                    providerOptions.Name, providerOptions.Owner, providerOptions.Repository, providerOptions.Branch);

                _adminProviders[providerSection.Key] = providerOptions;

                if (providerOptions.IsDefault)
                {
                    _defaultAdminProvider = providerSection.Key;
                }
            }
        }

        // Fallback to legacy configuration if no new providers found
        if (!_webProviders.Any() && !_adminProviders.Any())
        {
            var legacySection = _configuration.GetSection(GitHubOptions.Section);
            if (legacySection.Exists())
            {
                var legacyOptions = new GitHubProviderOptions();
                legacySection.Bind(legacyOptions);
                legacyOptions.Name = "default";

                _webProviders["default"] = legacyOptions;
                _adminProviders["default"] = legacyOptions;
                _defaultWebProvider = "default";
                _defaultAdminProvider = "default";

                _logger.LogWarning("Using legacy GitHub configuration. Consider migrating to the new multi-provider format.");
            }
        }

        _logger.LogInformation("Loaded {WebCount} web providers and {AdminCount} admin providers",
            _webProviders.Count, _adminProviders.Count);
    }

    public IGitHubApiClient GetClient(string providerName)
    {
        if (string.IsNullOrEmpty(providerName))
            throw new ArgumentException("Provider name cannot be empty", nameof(providerName));

        return _clients.GetOrAdd(providerName, name =>
        {
            // Try to find the provider in both web and admin configurations
            GitHubProviderOptions? options = null;

            if (_webProviders.TryGetValue(name, out var webOptions))
            {
                options = webOptions;
                _logger.LogInformation("Using Web provider configuration for '{Name}'", name);
            }
            else if (_adminProviders.TryGetValue(name, out var adminOptions))
            {
                options = adminOptions;
                _logger.LogInformation("Using Admin provider configuration for '{Name}'", name);
            }

            if (options == null)
            {
                throw new InvalidOperationException($"GitHub provider '{name}' not found in configuration");
            }

            // Validate the options before creating the client
            if (string.IsNullOrEmpty(options.Owner))
            {
                throw new InvalidOperationException($"GitHub provider '{name}' is missing 'Owner' configuration");
            }

            if (string.IsNullOrEmpty(options.Repository))
            {
                throw new InvalidOperationException($"GitHub provider '{name}' is missing 'Repository' configuration");
            }

            // Create a new HttpClient
            using var scope = _serviceProvider.CreateScope();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient($"GitHub_{name}");

            // Create a proper GitHubOptions instance (not GitHubProviderOptions)
            // This ensures compatibility with the GitHubApiClient constructor
            var githubOptions = new GitHubOptions
            {
                Owner = options.Owner,
                Repository = options.Repository,
                Branch = options.Branch ?? "main",
                ApiToken = options.ApiToken,
                ApiUrl = options.ApiUrl,
                ProviderId = options.ProviderId,
                IsDefault = options.IsDefault,
                ContentPath = options.ContentPath,
                EnableLocalization = options.EnableLocalization,
                DefaultLocale = options.DefaultLocale,
                SupportedLocales = options.SupportedLocales,
                SupportedExtensions = options.SupportedExtensions,
                ValidateContent = options.ValidateContent,
                WebhookSecret = options.WebhookSecret,
                EnablePolling = options.EnablePolling,
                PollingIntervalSeconds = options.PollingIntervalSeconds,
                UseBackgroundCacheUpdate = options.UseBackgroundCacheUpdate,
                ExcludePatterns = options.ExcludePatterns,
                Authentication = options.Authentication
            };

            // Create options wrapper
            var optionsWrapper = Options.Create(githubOptions);

            // Create logger
            var clientLogger = scope.ServiceProvider.GetRequiredService<ILogger<GitHubApiClient>>();

            // Log before creating the client
            _logger.LogInformation("Creating GitHub API client for provider '{Name}' with Owner='{Owner}', Repository='{Repository}', Branch='{Branch}'",
                name, githubOptions.Owner, githubOptions.Repository, githubOptions.Branch);

            var client = new GitHubApiClient(httpClient, optionsWrapper, clientLogger);

            // The GitHubApiClient constructor should have already set the repository and branch
            // but let's ensure they're set correctly
            client.SetRepository(githubOptions.Owner, githubOptions.Repository);
            client.SetBranch(githubOptions.Branch);

            if (!string.IsNullOrEmpty(githubOptions.ApiToken))
            {
                client.SetAccessToken(githubOptions.ApiToken);
            }

            _logger.LogInformation("Created GitHub API client for provider: {ProviderName}", name);

            return client;
        });
    }

    public IGitHubApiClient GetDefaultClient()
    {
        // Try web default first, then admin default
        var defaultProvider = _defaultWebProvider ?? _defaultAdminProvider;

        if (string.IsNullOrEmpty(defaultProvider))
        {
            if (_webProviders.Any())
            {
                defaultProvider = _webProviders.Keys.First();
            }
            else if (_adminProviders.Any())
            {
                defaultProvider = _adminProviders.Keys.First();
            }
            else
            {
                throw new InvalidOperationException("No GitHub providers configured");
            }
        }

        _logger.LogInformation("Getting default client for provider: {DefaultProvider}", defaultProvider);
        return GetClient(defaultProvider);
    }

    public IEnumerable<string> GetProviderNames()
    {
        // Return unique provider names from both web and admin
        return _webProviders.Keys.Union(_adminProviders.Keys).Distinct();
    }

    public bool ProviderExists(string providerName)
    {
        return _webProviders.ContainsKey(providerName) || _adminProviders.ContainsKey(providerName);
    }

    /// <summary>
    /// Gets the configuration for a specific provider
    /// </summary>
    public GitHubProviderOptions? GetProviderOptions(string providerName)
    {
        if (_webProviders.TryGetValue(providerName, out var webOptions))
            return webOptions;

        if (_adminProviders.TryGetValue(providerName, out var adminOptions))
            return adminOptions;

        return null;
    }
}