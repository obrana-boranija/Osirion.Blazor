using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Osirion.Blazor.Cms.Infrastructure.Events;
using Osirion.Blazor.Cms.Infrastructure.Events.Handlers;
using Osirion.Blazor.Cms.Infrastructure.Factories;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Osirion.Blazor.Cms.Infrastructure.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOsirionCms(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IContentBuilder>? configureContentProviders = null)
    {
        // Register options
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.Section));
        services.Configure<GitHubOptions>(configuration.GetSection(GitHubOptions.Section));
        services.Configure<FileSystemOptions>(configuration.GetSection(FileSystemOptions.Section));

        // Register core services
        services.AddSingleton<IMarkdownProcessor, MarkdownProcessor>();
        services.AddScoped<IMarkdownRendererService, MarkdownRendererService>();
        services.AddScoped<IStateStorageService, LocalStorageService>();

        // Register repositories
        services.AddRepositories();

        // Register cache services
        services.AddSingleton<CacheDecoratorFactory>();
        services.AddScoped<IContentCacheService, InMemoryContentCacheService>();

        // Register GitHub-specific services
        services.AddHttpClient<IGitHubApiClient, GitHubApiClient>();
        services.AddScoped<IGitHubTokenProvider, GitHubTokenProvider>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IGitHubAdminService, GitHubAdminService>();

        // Register CQRS components
        services.AddOsirionCms(configuration);

        // Register domain events
        services.AddDomainEvents();

        // Register content provider factories
        services.AddSingleton<IContentProviderFactory, ContentProviderFactory>();
        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddScoped<IContentProviderManager, ContentProviderManager>();

        // Configure content providers if specified
        if (configureContentProviders != null)
        {
            // Create a content builder
            var builder = new ContentBuilder(services, configuration);
            configureContentProviders(builder);
        }
        else
        {
            // Add default content providers
            services.AddOsirionContent(configuration, builder => {
                builder.AddGitHub();
                builder.AddFileSystem();
            });
        }

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register repository implementations
        services.AddScoped<GitHubContentRepository>();
        services.AddScoped<GitHubDirectoryRepository>();
        services.AddScoped<FileSystemContentRepository>();
        services.AddScoped<FileSystemDirectoryRepository>();

        // Register with automatic decorators using Scrutor
        services.Scan(scan => scan
            .FromAssemblyOf<GitHubContentRepository>()
            .AddClasses(classes => classes.AssignableTo(typeof(IRepository<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddDomainEvents(this IServiceCollection services)
    {
        // Register the domain event dispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Register all handlers using Scrutor
        services.Scan(scan => scan
            .FromAssemblyOf<ContentCreatedEventHandler>()
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }

    public static IServiceCollection AddOsirionContent(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IContentBuilder> configure)
    {
        // Create the content builder
        var builder = new ContentBuilder(services, configuration);

        // Configure providers
        configure(builder);

        return services;
    }
}

// Content builder that allows fluent configuration
public class ContentBuilder : IContentBuilder
{
    public IServiceCollection Services { get; }
    private readonly IConfiguration _configuration;

    public ContentBuilder(
        IServiceCollection services,
        IConfiguration configuration)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IContentBuilder AddGitHub(Action<GitHubOptions>? configure = null)
    {
        // Configure GitHub options
        if (configure != null)
        {
            Services.Configure<GitHubOptions>(configure);
        }

        // Register GitHub provider
        Services.AddScoped<GitHubContentProvider>();

        // Register with factory
        Services.PostConfigure<ServiceProviderOptions>(options =>
        {
            var sp = Services.BuildServiceProvider();
            var factory = sp.GetRequiredService<IContentProviderFactory>();
            var provider = sp.GetRequiredService<GitHubContentProvider>();

            try
            {
                factory.RegisterProvider<GitHubContentProvider>(_ => provider);

                // Make it default if configured
                var gitHubOptions = sp.GetRequiredService<IOptions<GitHubOptions>>();
                if (gitHubOptions.Value.IsDefault)
                {
                    factory.SetDefaultProvider(provider.ProviderId);
                }
            }
            catch (Exception ex)
            {
                var logger = sp.GetRequiredService<ILogger<ContentBuilder>>();
                logger.LogError(ex, "Error registering GitHub provider");
            }
        });

        return this;
    }

    public IContentBuilder AddFileSystem(Action<FileSystemOptions>? configure = null)
    {
        // Configure FileSystem options
        if (configure != null)
        {
            Services.Configure<FileSystemOptions>(configure);
        }

        // Register FileSystem provider
        Services.AddScoped<FileSystemContentProvider>();

        // Register with factory
        Services.PostConfigure<ServiceProviderOptions>(options =>
        {
            var sp = Services.BuildServiceProvider();
            var factory = sp.GetRequiredService<IContentProviderFactory>();
            var provider = sp.GetRequiredService<FileSystemContentProvider>();

            try
            {
                factory.RegisterProvider<FileSystemContentProvider>(_ => provider);

                // Make it default if configured
                var fsOptions = sp.GetRequiredService<IOptions<FileSystemOptions>>();
                if (fsOptions.Value.IsDefault)
                {
                    factory.SetDefaultProvider(provider.ProviderId);
                }
            }
            catch (Exception ex)
            {
                var logger = sp.GetRequiredService<ILogger<ContentBuilder>>();
                logger.LogError(ex, "Error registering FileSystem provider");
            }
        });

        return this;
    }

    public IContentBuilder AddProvider<TProvider>(Action<TProvider>? configure = null)
        where TProvider : class, IContentProvider
    {
        // Register the provider
        Services.AddScoped<TProvider>();

        // Configure if needed
        if (configure != null)
        {
            Services.PostConfigure<ServiceProviderOptions>(options =>
            {
                var sp = Services.BuildServiceProvider();
                var provider = sp.GetRequiredService<TProvider>();
                configure(provider);
            });
        }

        // Register with factory
        Services.PostConfigure<ServiceProviderOptions>(options =>
        {
            var sp = Services.BuildServiceProvider();
            var factory = sp.GetRequiredService<IContentProviderFactory>();
            var provider = sp.GetRequiredService<TProvider>();

            try
            {
                factory.RegisterProvider<TProvider>(_ => provider);
            }
            catch (Exception ex)
            {
                var logger = sp.GetRequiredService<ILogger<ContentBuilder>>();
                logger.LogError(ex, "Error registering custom provider");
            }
        });

        return this;
    }

    public IContentBuilder SetDefaultProvider<TProvider>()
        where TProvider : class, IContentProvider
    {
        Services.PostConfigure<ServiceProviderOptions>(options =>
        {
            var sp = Services.BuildServiceProvider();
            var factory = sp.GetRequiredService<IContentProviderFactory>();
            var provider = sp.GetRequiredService<TProvider>();

            try
            {
                factory.SetDefaultProvider(provider.ProviderId);
            }
            catch (Exception ex)
            {
                var logger = sp.GetRequiredService<ILogger<ContentBuilder>>();
                logger.LogError(ex, "Error setting default provider");
            }
        });

        return this;
    }

    public IContentBuilder SetDefaultProvider(string providerId)
    {
        Services.PostConfigure<ServiceProviderOptions>(options =>
        {
            var sp = Services.BuildServiceProvider();
            var factory = sp.GetRequiredService<IContentProviderFactory>();

            try
            {
                factory.SetDefaultProvider(providerId);
            }
            catch (Exception ex)
            {
                var logger = sp.GetRequiredService<ILogger<ContentBuilder>>();
                logger.LogError(ex, "Error setting default provider");
            }
        });

        return this;
    }
}