using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Options.Configuration;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Factory for creating GitHub API client instances for different providers
/// </summary>
/// <remarks>
/// Every <see cref="GetClient(string)"/> and <see cref="GetDefaultClient"/> call returns a new client owned by
/// the caller. Clients are never cached or shared, so a token, repository or branch set on one client can
/// never reach another caller, such as the public site or another user's admin session. Each client takes a
/// short-lived <see cref="HttpClient"/> named <c>GitHub_{providerName}</c> from <see cref="IHttpClientFactory"/>
/// for every operation.
/// </remarks>
public class GitHubApiClientFactory : IGitHubApiClientFactory
{
    /// <summary>
    /// Prefix of the <see cref="IHttpClientFactory"/> client name used for a provider's API calls
    /// </summary>
    internal const string HttpClientNamePrefix = "GitHub_";

    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GitHubApiClientFactory> _logger;
    private readonly Dictionary<string, GitHubProviderOptions> _webProviders = new();
    private readonly Dictionary<string, GitHubProviderOptions> _adminProviders = new();
    private string? _defaultWebProvider;
    private string? _defaultAdminProvider;

    /// <summary>Performs the GitHubApiClientFactory operation.</summary>
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

    /// <summary>
    /// Creates a new GitHub API client for the specified provider
    /// </summary>
    /// <param name="providerName">The name of the provider</param>
    /// <returns>
    /// A new client owned by the caller, seeded with the provider's owner, repository, branch and API token.
    /// Changes made through its Set* methods never affect any other client.
    /// </returns>
    /// <exception cref="ArgumentException">The provider name is empty.</exception>
    /// <exception cref="InvalidOperationException">The provider is not configured or misses its owner or repository.</exception>
    public IGitHubApiClient GetClient(string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerName))
            throw new ArgumentException("Provider name cannot be empty", nameof(providerName));

        // Try to find the provider in both web and admin configurations
        GitHubProviderOptions? options = null;

        if (_webProviders.TryGetValue(providerName, out var webOptions))
        {
            options = webOptions;
            _logger.LogDebug("Using Web provider configuration for '{Name}'", providerName);
        }
        else if (_adminProviders.TryGetValue(providerName, out var adminOptions))
        {
            options = adminOptions;
            _logger.LogDebug("Using Admin provider configuration for '{Name}'", providerName);
        }

        if (options is null)
        {
            throw new InvalidOperationException($"GitHub provider '{providerName}' not found in configuration");
        }

        // Validate the options before creating the client
        if (string.IsNullOrWhiteSpace(options.Owner))
        {
            throw new InvalidOperationException($"GitHub provider '{providerName}' is missing 'Owner' configuration");
        }

        if (string.IsNullOrWhiteSpace(options.Repository))
        {
            throw new InvalidOperationException($"GitHub provider '{providerName}' is missing 'Repository' configuration");
        }

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
            Authentication = options.Authentication
        };

        // Both services are singletons, so resolving them from the root provider captures no scope
        var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
        var clientLogger = _serviceProvider.GetRequiredService<ILogger<GitHubApiClient>>();

        _logger.LogDebug("Creating GitHub API client for provider '{Name}' with Owner='{Owner}', Repository='{Repository}', Branch='{Branch}'",
            providerName, githubOptions.Owner, githubOptions.Repository, githubOptions.Branch);

        return new GitHubApiClient(httpClientFactory, HttpClientNamePrefix + providerName, githubOptions, clientLogger);
    }

    /// <summary>
    /// Creates a new GitHub API client for the default provider
    /// </summary>
    /// <returns>A new client owned by the caller, as returned by <see cref="GetClient(string)"/>.</returns>
    /// <exception cref="InvalidOperationException">No GitHub provider is configured.</exception>
    public IGitHubApiClient GetDefaultClient()
    {
        // Try web default first, then admin default
        var defaultProvider = _defaultWebProvider ?? _defaultAdminProvider;

        if (string.IsNullOrWhiteSpace(defaultProvider))
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

        _logger.LogDebug("Getting default client for provider: {DefaultProvider}", defaultProvider);
        return GetClient(defaultProvider);
    }

    /// <summary>Gets or sets the GetProviderNames value.</summary>
    public IEnumerable<string> GetProviderNames()
    {
        // Return unique provider names from both web and admin
        return _webProviders.Keys.Union(_adminProviders.Keys).Distinct();
    }

    /// <summary>Gets or sets the ProviderExists value.</summary>
    public bool ProviderExists(string providerName)
    {
        return _webProviders.ContainsKey(providerName) || _adminProviders.ContainsKey(providerName);
    }

    /// <summary>
    /// Gets the configuration for a specific provider
    /// </summary>
    public GitHubProviderOptions? GetProviderOptions(string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerName))
            return null;

        // Check web providers first
        if (_webProviders.TryGetValue(providerName, out var webOptions))
        {
            _logger.LogDebug("Found provider options in Web configuration for '{ProviderName}'", providerName);
            return webOptions;
        }

        // Then check admin providers
        if (_adminProviders.TryGetValue(providerName, out var adminOptions))
        {
            _logger.LogDebug("Found provider options in Admin configuration for '{ProviderName}'", providerName);
            return adminOptions;
        }

        _logger.LogDebug("Provider options not found for '{ProviderName}'", providerName);
        return null;
    }

    /// <summary>
    /// Gets all configured provider options
    /// </summary>
    public IEnumerable<KeyValuePair<string, GitHubProviderOptions>> GetAllProviderOptions()
    {
        // Return all unique providers, preferring web over admin if duplicate names
        var allProviders = new Dictionary<string, GitHubProviderOptions>();

        // Add admin providers first
        foreach (var kvp in _adminProviders)
        {
            allProviders[kvp.Key] = kvp.Value;
        }

        // Then add/override with web providers (web takes precedence)
        foreach (var kvp in _webProviders)
        {
            allProviders[kvp.Key] = kvp.Value;
        }

        return allProviders;
    }
}
