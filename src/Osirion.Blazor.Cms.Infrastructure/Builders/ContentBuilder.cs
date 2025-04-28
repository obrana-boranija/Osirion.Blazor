using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Providers;
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

        // Register GitHub repositories
        Services.AddScoped<GitHubContentRepository>();
        Services.AddScoped<GitHubDirectoryRepository>();

        // Register provider directly
        Services.AddScoped<GitHubContentProvider>();
        Services.AddScoped<IContentProvider>(sp => sp.GetRequiredService<GitHubContentProvider>());

        // Register default setter
        Services.AddSingleton<IDefaultProviderSetter>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<GitHubOptions>>().Value;
            var providerId = options.ProviderId ?? $"github-{options.Owner}-{options.Repository}";
            return new DefaultProviderSetter(providerId, options.IsDefault);
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

        // Register provider directly
        Services.AddScoped<FileSystemContentProvider>();
        Services.AddScoped<IContentProvider>(sp => sp.GetRequiredService<FileSystemContentProvider>());

        // Register default setter
        Services.AddSingleton<IDefaultProviderSetter>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<FileSystemOptions>>().Value;
            var providerId = options.ProviderId ?? $"filesystem-{options.BasePath.GetHashCode():x}";
            return new DefaultProviderSetter(providerId, options.IsDefault);
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

        // Register as IContentProvider
        Services.AddScoped<IContentProvider>(sp => {
            var provider = sp.GetRequiredService<TProvider>();

            // Apply configuration if provided
            if (configure != null)
            {
                configure(provider);
            }

            return provider;
        });

        Logger.LogInformation("Added custom content provider: {ProviderType}", typeof(TProvider).Name);
        return this;
    }
}