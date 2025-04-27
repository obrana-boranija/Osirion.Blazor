using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.UnitOfWork;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Osirion.Blazor.Cms.Infrastructure.Factories;

/// <summary>
/// Factory for creating unit of work instances
/// </summary>
public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;

    public UnitOfWorkFactory(
        IServiceProvider serviceProvider,
        IRepositoryFactory repositoryFactory,
        IConfiguration configuration,
        ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public IUnitOfWork Create(string providerId)
    {
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        // Create appropriate repositories
        var contentRepository = _repositoryFactory.CreateContentRepository(providerId);
        var directoryRepository = _repositoryFactory.CreateDirectoryRepository(providerId);

        // Create the right UnitOfWork based on provider type
        if (providerId.StartsWith("github"))
        {
            var apiClient = _serviceProvider.GetRequiredService<IGitHubApiClient>();
            var logger = _loggerFactory.CreateLogger<GitHubUnitOfWork>();

            return new GitHubUnitOfWork(apiClient, contentRepository, directoryRepository, logger);
        }
        else if (providerId.StartsWith("filesystem"))
        {
            // Get backup directory from configuration or use a default
            string backupDirectory = _configuration["Osirion:Cms:FileSystem:BackupDirectory"] ??
                                    Path.Combine(Path.GetTempPath(), "osirion-cms-backup");

            var logger = _loggerFactory.CreateLogger<FileSystemUnitOfWork>();

            return new FileSystemUnitOfWork(contentRepository, directoryRepository, logger, backupDirectory);
        }
        else
        {
            throw new ArgumentException($"Unsupported provider type: {providerId}", nameof(providerId));
        }
    }

    public IUnitOfWork CreateForDefaultProvider()
    {
        var defaultProviderId = _repositoryFactory.GetDefaultProviderId();

        if (string.IsNullOrEmpty(defaultProviderId))
            throw new InvalidOperationException("No default provider is configured");

        return Create(defaultProviderId);
    }
}