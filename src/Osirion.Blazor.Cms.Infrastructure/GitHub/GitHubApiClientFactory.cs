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
            }
            else if (_adminProviders.TryGetValue(name, out var adminOptions))
            {
                options = adminOptions;
            }

            if (options == null)
            {
                throw new InvalidOperationException($"GitHub provider '{name}' not found in configuration");
            }

            // Create a new HttpClient and GitHubApiClient instance
            using var scope = _serviceProvider.CreateScope();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient($"GitHub_{name}");

            // Create options wrapper
            var optionsWrapper = Options.Create(options);

            // Create logger
            var clientLogger = scope.ServiceProvider.GetRequiredService<ILogger<GitHubApiClient>>();

            var client = new GitHubApiClient(httpClient, optionsWrapper, clientLogger);

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