using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Providers;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.DependencyInjection
{
    public class ServiceCollectionExtensionsTests
    {
        private readonly IServiceCollection _services;
        private readonly IConfiguration _configuration;

        public ServiceCollectionExtensionsTests()
        {
            _services = new ServiceCollection();

            // Create mock configuration
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"Osirion:Cms:GitHub:Owner", "testOwner"},
                {"Osirion:Cms:GitHub:Repository", "testRepo"},
                {"Osirion:Cms:GitHub:Branch", "main"},
                {"Osirion:Cms:GitHub:ContentPath", "content"},
                {"Osirion:Cms:GitHub:ApiToken", "test-token"},

                {"Osirion:Cms:FileSystem:BasePath", "test-path"},
                {"Osirion:Cms:FileSystem:ContentRoot", "content"},
                {"Osirion:Cms:FileSystem:CacheDurationMinutes", "5"}
            });

            _configuration = configBuilder.Build();
        }

        [Fact]
        public void AddCms_RegistersRequiredServices()
        {
            // Act
            _services.AddCms(_configuration);
            var provider = _services.BuildServiceProvider();

            // Assert
            provider.GetService<IMarkdownProcessor>().ShouldNotBeNull();
            provider.GetService<IContentProviderRegistry>().ShouldNotBeNull();
            provider.GetService<IContentProviderManager>().ShouldNotBeNull();
            provider.GetService<IContentProviderInitializer>().ShouldNotBeNull();
        }

        [Fact]
        public void AddGitHubContentProvider_RegistersGitHubServices()
        {
            // Act
            _services.AddGitHubContentProvider(_configuration);
            var provider = _services.BuildServiceProvider();

            // Assert
            provider.GetService<IGitHubApiClient>().ShouldNotBeNull();
            provider.GetService<GitHubContentRepository>().ShouldNotBeNull();
            provider.GetService<GitHubDirectoryRepository>().ShouldNotBeNull();
            provider.GetService<GitHubContentProvider>().ShouldNotBeNull();
            provider.GetService<IContentProvider>().ShouldNotBeNull();
            provider.GetService<IDefaultProviderSetter>().ShouldNotBeNull();
        }

        [Fact]
        public void AddFileSystemContentProvider_RegistersFileSystemServices()
        {
            // Act
            _services.AddFileSystemContentProvider(_configuration);
            var provider = _services.BuildServiceProvider();

            // Assert
            provider.GetService<FileSystemContentRepository>().ShouldNotBeNull();
            provider.GetService<FileSystemDirectoryRepository>().ShouldNotBeNull();
            provider.GetService<FileSystemContentProvider>().ShouldNotBeNull();
            provider.GetService<IContentProvider>().ShouldNotBeNull();
            provider.GetService<IDefaultProviderSetter>().ShouldNotBeNull();
        }

        [Fact(Skip = "Temporarily disabled in CI until fixed")]
        public void AddCustomContentProvider_RegistersCustomProvider()
        {
            // Arrange
            var customProvider = Substitute.For<IContentProvider>();
            customProvider.ProviderId.Returns("custom-provider");

            // Act
            _services.AddSingleton<IContentProvider>(customProvider);
            _services.AddCustomContentProvider<IContentProvider>(isDefault: true);
            var provider = _services.BuildServiceProvider();

            // Assert
            provider.GetService<IDefaultProviderSetter>().ShouldNotBeNull();

            // Get provider registry and check if our provider is registered
            var registry = provider.GetService<IContentProviderRegistry>();
            registry.ShouldNotBeNull();

            var defaultProvider = registry.GetDefaultProvider();
            defaultProvider.ShouldNotBeNull();
        }

        [Fact]
        public void AddOsirionCmsWithProviders_RegistersAllConfiguredProviders()
        {
            // Act
            _services.AddOsirionCmsWithProviders(_configuration);
            var provider = _services.BuildServiceProvider();

            // Assert
            provider.GetService<IGitHubApiClient>().ShouldNotBeNull();
            provider.GetService<GitHubContentRepository>().ShouldNotBeNull();
            provider.GetService<GitHubContentProvider>().ShouldNotBeNull();

            provider.GetService<FileSystemContentRepository>().ShouldNotBeNull();
            provider.GetService<FileSystemDirectoryRepository>().ShouldNotBeNull();
            provider.GetService<FileSystemContentProvider>().ShouldNotBeNull();

            provider.GetService<IContentProviderRegistry>().ShouldNotBeNull();
            provider.GetService<IContentProviderManager>().ShouldNotBeNull();
        }
    }
}