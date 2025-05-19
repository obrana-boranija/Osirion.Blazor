using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services
{
    public class ContentProviderManagerTests
    {
        private readonly IContentProviderRegistry _registry;
        private readonly ILogger<ContentProviderManager> _logger;
        private readonly ContentProviderManager _manager;
        private readonly IContentProvider _defaultProvider;

        public ContentProviderManagerTests()
        {
            _registry = Substitute.For<IContentProviderRegistry>();
            _logger = Substitute.For<ILogger<ContentProviderManager>>();

            // Create mock default provider
            _defaultProvider = Substitute.For<IContentProvider>();
            _defaultProvider.ProviderId.Returns("default-provider");
            _defaultProvider.DisplayName.Returns("Default Provider");

            _registry.GetDefaultProvider().Returns(_defaultProvider);

            _manager = new ContentProviderManager(_registry, _logger);
        }

        [Fact]
        public void GetDefaultProvider_ReturnsProviderFromRegistry()
        {
            // Act
            var provider = _manager.GetDefaultProvider();

            // Assert
            provider.ShouldBe(_defaultProvider);
            _registry.Received(1).GetDefaultProvider();
        }

        [Fact]
        public void GetProvider_CallsRegistryWithCorrectId()
        {
            // Arrange
            var provider = Substitute.For<IContentProvider>();
            _registry.GetProvider("test-provider").Returns(provider);

            // Act
            var result = _manager.GetProvider("test-provider");

            // Assert
            result.ShouldBe(provider);
            _registry.Received(1).GetProvider("test-provider");
        }

        [Fact]
        public void GetAllProviders_ReturnsAllProvidersFromRegistry()
        {
            // Arrange
            var providers = new List<IContentProvider> { _defaultProvider };
            _registry.GetAllProviders().Returns(providers);

            // Act
            var result = _manager.GetAllProviders();

            // Assert
            result.ShouldBe(providers);
            _registry.Received(1).GetAllProviders();
        }

        [Fact]
        public async Task GetDirectoryTreeAsync_CallsProviderMethod()
        {
            // Arrange
            var directories = new List<DirectoryItem>
            {
                DirectoryItem.Create("dir1", "path1", "Dir 1", "default-provider"),
                DirectoryItem.Create("dir2", "path2", "Dir 2", "default-provider")
            };

            _defaultProvider.GetDirectoriesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(directories);

            // Act
            var result = await _manager.GetDirectoryTreeAsync("en");

            // Assert
            result.ShouldBe(directories);
            await _defaultProvider.Received(1).GetDirectoriesAsync("en", Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetContentByLocaleAsync_CallsProviderMethod()
        {
            // Arrange
            var contentItems = new List<ContentItem>
            {
                ContentItem.Create("id1", "Title 1", "Content 1", "path1.md", "default-provider"),
                ContentItem.Create("id2", "Title 2", "Content 2", "path2.md", "default-provider")
            };

            _defaultProvider.GetItemsByQueryAsync(Arg.Is<ContentQuery>(q => q.Locale == "en"), Arg.Any<CancellationToken>())
                .Returns(contentItems);

            // Act
            var result = await _manager.GetContentByLocaleAsync("en");

            // Assert
            result.ShouldBe(contentItems);
            await _defaultProvider.Received(1).GetItemsByQueryAsync(
                Arg.Is<ContentQuery>(q => q.Locale == "en"),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetLocalizedContentAsync_ReturnsCorrectContent()
        {
            // Arrange
            var contentId = "content-id";
            var locale = "en";

            var contentItems = new List<ContentItem>
            {
                ContentItem.Create("id1", "English Title", "English Content", "en/page.md", "default-provider"),
                ContentItem.Create("id2", "French Title", "French Content", "fr/page.md", "default-provider")
            };

            contentItems[0].SetLocale("en");
            contentItems[1].SetLocale("fr");
            contentItems[0].SetContentId(contentId);
            contentItems[1].SetContentId(contentId);

            _defaultProvider.GetItemsByQueryAsync(
                Arg.Is<ContentQuery>(q => q.LocalizationId == contentId),
                Arg.Any<CancellationToken>())
                .Returns(contentItems);

            // Act
            var result = await _manager.GetLocalizedContentAsync(contentId, locale);

            // Assert
            result.ShouldBe(contentItems[0]);
            await _defaultProvider.Received(1).GetItemsByQueryAsync(
                Arg.Is<ContentQuery>(q => q.LocalizationId == contentId),
                Arg.Any<CancellationToken>());
        }
    }
}