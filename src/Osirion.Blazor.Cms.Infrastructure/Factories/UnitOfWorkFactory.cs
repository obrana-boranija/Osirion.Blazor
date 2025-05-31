using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.UnitOfWork;

namespace Osirion.Blazor.Cms.Infrastructure.Factories;

/// <summary>
/// Factory for creating unit of work instances
/// </summary>
public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IContentProviderRegistry _providerRegistry;
    private readonly IConfiguration _configuration;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILoggerFactory _loggerFactory;

    public UnitOfWorkFactory(
        IServiceProvider serviceProvider,
        IContentProviderRegistry providerRegistry,
        IConfiguration configuration,
        IDomainEventDispatcher eventDispatcher,
        ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _providerRegistry = providerRegistry ?? throw new ArgumentNullException(nameof(providerRegistry));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _eventDispatcher = eventDispatcher;
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public IUnitOfWork Create(string providerId)
    {
        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        // Get provider to determine provider type
        var provider = _providerRegistry.GetProvider(providerId);
        if (provider is null)
            throw new InvalidOperationException($"Provider not found with ID: {providerId}");

        // Create appropriate unit of work based on provider type
        if (providerId.StartsWith("github"))
        {
            var apiClient = _serviceProvider.GetRequiredService<IGitHubApiClient>();
            var contentRepo = _serviceProvider.GetRequiredService<GitHubContentRepository>();
            var directoryRepo = _serviceProvider.GetRequiredService<GitHubDirectoryRepository>();
            var logger = _loggerFactory.CreateLogger<GitHubUnitOfWork>();

            return new GitHubUnitOfWork(apiClient, contentRepo, directoryRepo, _eventDispatcher, logger);
        }
        else if (providerId.StartsWith("filesystem"))
        {
            // Get backup directory from configuration or use a default
            string backupDirectory = _configuration["Osirion:Cms:FileSystem:BackupDirectory"] ??
                                    Path.Combine(Path.GetTempPath(), "osirion-cms-backup");

            var contentRepo = _serviceProvider.GetRequiredService<FileSystemContentRepository>();
            var directoryRepo = _serviceProvider.GetRequiredService<FileSystemDirectoryRepository>();
            var logger = _loggerFactory.CreateLogger<FileSystemUnitOfWork>();

            return new FileSystemUnitOfWork(contentRepo, directoryRepo, _eventDispatcher, logger, backupDirectory);
        }
        else
        {
            throw new ArgumentException($"Unsupported provider type: {providerId}", nameof(providerId));
        }
    }

    public IUnitOfWork CreateForDefaultProvider()
    {
        var defaultProvider = _providerRegistry.GetDefaultProvider();
        if (defaultProvider is null)
            throw new InvalidOperationException("No default provider is configured");

        return Create(defaultProvider.ProviderId);
    }
}