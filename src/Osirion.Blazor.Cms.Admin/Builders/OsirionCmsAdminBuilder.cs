using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Configuration;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Features;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Services.State;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Admin.Builders;

/// <summary>
/// Builder implementation for configuring Osirion CMS Admin
/// </summary>
public class OsirionCmsAdminBuilder : IOsirionCmsAdminBuilder
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<OsirionCmsAdminBuilder> _logger;

    /// <summary>
    /// Gets the service collection for configuration
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Creates a new instance of the OsirionCmsAdminBuilder
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="logger">The logger</param>
    public OsirionCmsAdminBuilder(
        IServiceCollection services,
        IConfiguration configuration,
        ILogger<OsirionCmsAdminBuilder> logger)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Register core services
        RegisterCoreServices();
    }

    /// <summary>
    /// Configures GitHub provider for CMS admin
    /// </summary>
    public IOsirionCmsAdminBuilder UseGitHubProvider(Action<GitHubAdminOptions>? configure = null)
    {
        _logger.LogInformation("Configuring GitHub provider for CMS admin");

        // Register GitHub services from Infrastructure
        Services.AddGitHubAdminServices(_configuration);

        if (configure != null)
        {
            Services.Configure<CmsAdminOptions>(options =>
            {
                options.DefaultContentProvider = "GitHub";
                configure(options.GitHub);
            });
        }

        return this;
    }

    /// <summary>
    /// Configures FileSystem provider for CMS admin
    /// </summary>
    public IOsirionCmsAdminBuilder UseFileSystemProvider(Action<FileSystemAdminOptions>? configure = null)
    {
        _logger.LogInformation("Configuring FileSystem provider for CMS admin");

        // Register FileSystem services from Infrastructure
        Services.AddFileSystemAdminServices(_configuration);

        if (configure != null)
        {
            Services.Configure<CmsAdminOptions>(options =>
            {
                options.DefaultContentProvider = "FileSystem";
                configure(options.FileSystem);
            });
        }

        return this;
    }

    /// <summary>
    /// Configures authentication for CMS admin
    /// </summary>
    public IOsirionCmsAdminBuilder ConfigureAuthentication(Action<AuthenticationOptions> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        _logger.LogInformation("Configuring authentication for CMS admin");

        Services.Configure<CmsAdminOptions>(options =>
        {
            configure(options.Authentication);
        });

        return this;
    }

    /// <summary>
    /// Configures UI theme for CMS admin
    /// </summary>
    public IOsirionCmsAdminBuilder ConfigureTheme(Action<ThemeOptions> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        _logger.LogInformation("Configuring theme for CMS admin");

        Services.Configure<CmsAdminOptions>(options =>
        {
            configure(options.Theme);
        });

        return this;
    }

    /// <summary>
    /// Configures content rules for CMS admin
    /// </summary>
    public IOsirionCmsAdminBuilder ConfigureContentRules(Action<ContentRulesOptions> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        _logger.LogInformation("Configuring content rules for CMS admin");

        Services.Configure<CmsAdminOptions>(options =>
        {
            configure(options.ContentRules);
        });

        return this;
    }

    /// <summary>
    /// Adds a custom content provider to CMS admin
    /// </summary>
    public IOsirionCmsAdminBuilder AddProvider<TProvider>() where TProvider : class, IContentProvider
    {
        _logger.LogInformation("Adding custom provider {ProviderType} to CMS admin", typeof(TProvider).Name);

        Services.AddScoped<IContentProvider, TProvider>();

        return this;
    }

    /// <summary>
    /// Registers core services needed for the CMS admin
    /// </summary>
    private void RegisterCoreServices()
    {
        // Register state services
        Services.AddScoped<CmsState>();
        Services.AddScoped<StateManager>();

        // Register event system
        Services.AddSingleton<CmsEventMediator>();
        Services.AddSingleton<IEventPublisher, EventBus>();
        Services.AddSingleton<IEventSubscriber>(provider =>
            provider.GetRequiredService<IEventPublisher>() as EventBus
            ?? throw new InvalidOperationException("IEventPublisher is not of type EventBus."));

        // Register error handling
        Services.AddScoped<ErrorHandlingService>();

        // Register startup service
        Services.AddScoped<CmsAdminStartupService>();

        // Register repository adapter factory
        Services.AddScoped<IContentRepositoryAdapterFactory, ContentRepositoryAdapterFactory>();
        Services.AddScoped<IContentRepositoryAdapter>(sp => {
            var factory = sp.GetRequiredService<IContentRepositoryAdapterFactory>();
            return factory.CreateDefaultAdapter();
        });

        // Register feature services
        Services.AddScoped<AuthenticationService>();
        Services.AddScoped<ContentBrowserService>();
        Services.AddScoped<ContentEditorService>();
        Services.AddScoped<RepositoryService>();

        // Register all feature modules
        Services.AddFeatures();
    }
}