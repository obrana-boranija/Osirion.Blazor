using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Builders;

/// <summary>
/// Implements the content builder for configuring content providers
/// </summary>
public class ContentBuilder : CmsBuilderBase, IContentBuilder
{
    /// <summary>
    /// Initializes a new instance of the ContentBuilder class
    /// </summary>
    public ContentBuilder(
        IServiceCollection services,
        IConfiguration configuration,
        ILogger<ContentBuilder> logger)
        : base(services, configuration, logger)
    {
    }

    /// <inheritdoc/>
    public IContentBuilder AddGitHub(Action<GitHubOptions>? configure = null)
    {
        // Configure GitHub options
        if (configure != null)
        {
            Services.Configure<GitHubOptions>(options => {
                // First apply configuration from appsettings
                Configuration.GetSection(GitHubOptions.Section).Bind(options);

                // Then apply custom configuration
                configure(options);
            });
        }
        else
        {
            // Just use configuration from appsettings
            Services.Configure<GitHubOptions>(Configuration.GetSection(GitHubOptions.Section));
        }

        // Register HTTP clients first
        RegisterHttpClient<IGitHubApiClient, GitHubApiClient>();
        RegisterHttpClient<IGitHubTokenProvider, GitHubTokenProvider>();
        RegisterHttpClient<IAuthenticationService, AuthenticationService>();
        RegisterHttpClient<IGitHubAdminService, GitHubAdminService>();

        // Register GitHub repositories
        Services.AddScoped<GitHubContentRepository>();
        Services.AddScoped<GitHubDirectoryRepository>();
        Services.AddScoped<GitHubContentProvider>();

        // Register provider factory registration
        Services.AddSingleton<IProviderRegistration>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<GitHubOptions>>().Value;
            var providerId = options.ProviderId ?? $"github-{options.Owner}-{options.Repository}";

            return new ProviderRegistration(
                providerId,
                sp => sp.GetRequiredService<GitHubContentProvider>(),
                options.IsDefault);
        });

        Logger.LogInformation("Added GitHub content provider");
        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder AddFileSystem(Action<FileSystemOptions>? configure = null)
    {
        // Configure FileSystem options
        if (configure != null)
        {
            Services.Configure<FileSystemOptions>(options => {
                // First apply configuration from appsettings
                Configuration.GetSection(FileSystemOptions.Section).Bind(options);

                // Then apply custom configuration
                configure(options);
            });
        }
        else
        {
            // Just use configuration from appsettings
            Services.Configure<FileSystemOptions>(Configuration.GetSection(FileSystemOptions.Section));
        }

        // Register FileSystem repositories
        Services.AddScoped<FileSystemContentRepository>();
        Services.AddScoped<FileSystemDirectoryRepository>();
        Services.AddScoped<FileSystemContentProvider>();

        // Register provider factory registration
        Services.AddSingleton<IProviderRegistration>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<FileSystemOptions>>().Value;
            var providerId = options.ProviderId ?? $"filesystem-{options.BasePath.GetHashCode():x}";

            return new ProviderRegistration(
                providerId,
                sp => sp.GetRequiredService<FileSystemContentProvider>(),
                options.IsDefault);
        });

        Logger.LogInformation("Added FileSystem content provider");
        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder AddProvider<TProvider>(Action<TProvider>? configure = null)
        where TProvider : class, IContentProvider
    {
        // Register the provider
        Services.AddScoped<TProvider>();

        // Store the configurator for use during initialization
        if (configure != null)
        {
            Services.AddSingleton<IProviderConfigurator>(new ProviderConfigurator<TProvider>(configure));
        }

        // Register provider registration
        Services.AddSingleton<IProviderRegistration>(sp =>
        {
            var provider = sp.GetRequiredService<TProvider>();
            var providerId = provider.ProviderId;

            return new ProviderRegistration(
                providerId,
                sp => {
                    var p = sp.GetRequiredService<TProvider>();

                    // Apply configuration if available
                    sp.GetServices<IProviderConfigurator>()
                        .OfType<ProviderConfigurator<TProvider>>()
                        .ToList()
                        .ForEach(config => config.Configure(p));

                    return p;
                },
                false);
        });

        Logger.LogInformation("Added custom content provider: {ProviderType}", typeof(TProvider).Name);
        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder SetDefaultProvider<TProvider>()
        where TProvider : class, IContentProvider
    {
        // Register default provider setter
        Services.AddSingleton<IDefaultProviderSetter>(sp =>
            new TypeDefaultProviderSetter<TProvider>());

        Logger.LogInformation("Set default provider to type: {ProviderType}", typeof(TProvider).Name);
        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder SetDefaultProvider(string providerId)
    {
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        // Register default provider setter
        Services.AddSingleton<IDefaultProviderSetter>(
            new IdDefaultProviderSetter(providerId));

        Logger.LogInformation("Set default provider to ID: {ProviderId}", providerId);
        return this;
    }
}