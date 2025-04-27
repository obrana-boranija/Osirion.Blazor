using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Services;
using System.Net.Http.Headers;

namespace Osirion.Blazor.Cms.Internal;

/// <summary>
/// Implementation of the content builder
/// </summary>
internal class ContentBuilder : IContentBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentBuilder"/> class.
    /// </summary>
    public ContentBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));

        // Ensure the provider factory is registered
        Services.TryAddSingleton<IContentProviderFactory, ContentProviderFactory>();
    }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <inheritdoc/>
    public IContentBuilder AddGitHub(Action<GitHubOptions>? configure = null)
    {
        var options = new GitHubOptions();
        configure?.Invoke(options);

        // Validate options
        if (string.IsNullOrEmpty(options.Owner))
        {
            throw new ArgumentException("GitHub owner is required", nameof(configure));
        }

        if (string.IsNullOrEmpty(options.Repository))
        {
            throw new ArgumentException("GitHub repository is required", nameof(configure));
        }

        // Register options
        Services.Configure<GitHubOptions>(opt =>
        {
            opt.Owner = options.Owner;
            opt.Repository = options.Repository;
            opt.ContentPath = options.ContentPath;
            opt.Branch = options.Branch;
            opt.ApiToken = options.ApiToken;
            opt.ProviderId = options.ProviderId;
            opt.EnableCaching = options.EnableCaching;
            opt.CacheDurationMinutes = options.CacheDurationMinutes;
            opt.SupportedExtensions = options.SupportedExtensions;
            opt.CommitterName = options.CommitterName;
            opt.CommitterEmail = options.CommitterEmail;
            opt.ApiUrl = options.ApiUrl;
            opt.IsDefault = options.IsDefault;
        });

        // Configure HttpClient properly during registration
        Services.AddHttpClient<IGitHubApiClient, GitHubApiClient>(client =>
        {
            client.BaseAddress = new Uri(options.ApiUrl ?? "https://api.github.com/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("OsirionBlazor", "2.0"));

            if (!string.IsNullOrEmpty(options.ApiToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiToken);
            }
        });

        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder AddFileSystem(Action<FileSystemOptions>? configure = null)
    {
        var options = new FileSystemOptions();
        configure?.Invoke(options);

        // Validate options
        if (string.IsNullOrEmpty(options.BasePath))
        {
            throw new ArgumentException("FileSystem base path is required", nameof(configure));
        }

        // Register options
        Services.Configure<FileSystemOptions>(opt =>
        {
            opt.BasePath = options.BasePath;
            opt.WatchForChanges = options.WatchForChanges;
            opt.ProviderId = options.ProviderId;
            opt.EnableCaching = options.EnableCaching;
            opt.CacheDurationMinutes = options.CacheDurationMinutes;
            opt.SupportedExtensions = options.SupportedExtensions;
            opt.IsDefault = options.IsDefault;
        });

        // Register file system provider
        Services.TryAddScoped<FileSystemOptions>();
        //Services.TryAddScoped<IContentProvider>(sp => sp.GetRequiredService<FileSystemOptions>());

        // Set as default if specified
        if (options.IsDefault)
        {
            var providerId = options.ProviderId ?? $"filesystem-{options.BasePath.GetHashCode():x}";
            SetDefaultProvider(providerId);
        }

        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder AddProvider<TProvider>(Action<TProvider>? configure = null)
        where TProvider : class, IContentProvider
    {
        if (configure != null)
        {
            // Register with factory method to allow configuration
            Services.AddScoped(sp => {
                var provider = ActivatorUtilities.CreateInstance<TProvider>(sp);
                configure(provider);
                return provider;
            });
        }
        else
        {
            // Standard registration
            Services.AddScoped<TProvider>();
        }

        // Register as IContentProvider
        Services.AddScoped<IContentProvider>(sp => sp.GetRequiredService<TProvider>());

        // Register with the factory
        Services.PostConfigure<IContentProviderFactory>(factory =>
        {
            if (factory is ContentProviderFactory contentFactory)
            {
                contentFactory.RegisterProvider<TProvider>(sp => sp.GetRequiredService<TProvider>());
            }
        });

        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder SetDefaultProvider<TProvider>()
        where TProvider : class, IContentProvider
    {
        // We need to resolve the provider to get its ID
        Services.PostConfigure<IContentProviderFactory>(factory =>
        {
            if (factory is ContentProviderFactory contentFactory)
            {
                var provider = Services
                    .BuildServiceProvider()
                    .GetRequiredService<TProvider>();

                contentFactory.SetDefaultProvider(provider.ProviderId);
            }
        });

        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder SetDefaultProvider(string providerId)
    {
        if (string.IsNullOrEmpty(providerId))
        {
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));
        }

        Services.PostConfigure<IContentProviderFactory>(factory =>
        {
            if (factory is ContentProviderFactory contentFactory)
            {
                contentFactory.SetDefaultProvider(providerId);
            }
        });

        return this;
    }
}