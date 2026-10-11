using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Providers;
using Shouldly;

namespace Osirion.Blazor.Cms.Tests.Providers
{
    public class ContentProviderBaseTests
    {
        private readonly ILogger<TestContentProvider> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly TestContentProvider _provider;
        private readonly IOptions<ContentProviderOptions> _options;

        public ContentProviderBaseTests()
        {
            _logger = Substitute.For<ILogger<TestContentProvider>>();
            _memoryCache = Substitute.For<IMemoryCache>();
            _options = Substitute.For<IOptions<ContentProviderOptions>>();
            _provider = new TestContentProvider(_memoryCache, _options, _logger);
        }

        [Fact]
        public void ProviderId_ReturnsExpectedValue()
        {
            // Assert
            _provider.ProviderId.ShouldBe("test-provider");
        }

        [Fact]
        public void DisplayName_ReturnsExpectedValue()
        {
            // Assert
            _provider.DisplayName.ShouldBe("Test Provider");
        }

        [Fact]
        public void IsReadOnly_ReturnsExpectedValue()
        {
            // Assert
            _provider.IsReadOnly.ShouldBeTrue();
        }

        [Fact]
        public async Task GetAllItemsAsync_ReturnsExpectedItems()
        {
            // Arrange
            var expectedItems = new List<ContentItem>
            {
                ContentItem.Create("1", "Item 1", "Content 1", "path1", "test-provider"),
                ContentItem.Create("2", "Item 2", "Content 2", "path2", "test-provider")
            };

            _provider.SetupTestItems(expectedItems);

            // Act
            var result = await _provider.GetAllItemsAsync();

            // Assert
            result.Count.ShouldBe(expectedItems.Count);
            result[0].Id.ShouldBe(expectedItems[0].Id);
            result[1].Id.ShouldBe(expectedItems[1].Id);
        }

        [Fact]
        public async Task GetItemByIdAsync_WithValidId_ReturnsItem()
        {
            // Arrange
            var items = new List<ContentItem>
            {
                ContentItem.Create("1", "Item 1", "Content 1", "path1", "test-provider"),
                ContentItem.Create("2", "Item 2", "Content 2", "path2", "test-provider")
            };

            _provider.SetupTestItems(items);

            // Act
            var result = await _provider.GetItemByIdAsync("1");

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe("1");
            result.Title.ShouldBe("Item 1");
        }

        [Fact]
        public async Task GetItemByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var items = new List<ContentItem>
            {
                ContentItem.Create("1", "Item 1", "Content 1", "path1", "test-provider")
            };

            _provider.SetupTestItems(items);

            // Act
            var result = await _provider.GetItemByIdAsync("999");

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetItemByPathAsync_WithValidPath_ReturnsItem()
        {
            // Arrange
            var items = new List<ContentItem>
            {
                ContentItem.Create("1", "Item 1", "Content 1", "path1", "test-provider"),
                ContentItem.Create("2", "Item 2", "Content 2", "path2", "test-provider")
            };

            _provider.SetupTestItems(items);

            // Act
            var result = await _provider.GetItemByPathAsync("path1");

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe("1");
            result.Title.ShouldBe("Item 1");
        }

        [Fact]
        public async Task GetItemByPathAsync_WithInvalidPath_ReturnsNull()
        {
            // Arrange
            var items = new List<ContentItem>
            {
                ContentItem.Create("1", "Item 1", "Content 1", "path1", "test-provider")
            };

            _provider.SetupTestItems(items);

            // Act
            var result = await _provider.GetItemByPathAsync("non-existent-path");

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetItemByUrlAsync_WithValidUrl_ReturnsItem()
        {
            // Arrange
            var items = new List<ContentItem>
            {
                ContentItem.Create("1", "Item 1", "Content 1", "path1", "test-provider"),
                ContentItem.Create("2", "Item 2", "Content 2", "path2", "test-provider")
            };

            items[0].SetUrl("url1");
            items[1].SetUrl("url2");

            _provider.SetupTestItems(items);

            // Act
            var result = await _provider.GetItemByUrlAsync("url1");

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe("1");
            result.Title.ShouldBe("Item 1");
        }

        [Fact]
        public async Task GetTagsAsync_ReturnsAllTags()
        {
            // Arrange
            var tags = new List<ContentTag>
            {
                ContentTag.Create("Tag 1", "tag-1", 5),
                ContentTag.Create("Tag 2", "tag-2", 3),
                ContentTag.Create("Tag 3", "tag-3", 7)
            };

            _provider.SetupTestTags(tags);

            // Act
            var result = await _provider.GetTagsAsync();

            // Assert
            result.Count.ShouldBe(3);
            result.ShouldContain(t => t.Name == "Tag 1" && t.Count == 5);
            result.ShouldContain(t => t.Name == "Tag 2" && t.Count == 3);
            result.ShouldContain(t => t.Name == "Tag 3" && t.Count == 7);
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<ContentCategory>
            {
                ContentCategory.Create("Category 1", "category-1", 4),
                ContentCategory.Create("Category 2", "category-2", 2),
                ContentCategory.Create("Category 3", "category-3", 6)
            };

            _provider.SetupTestCategories(categories);

            // Act
            var result = await _provider.GetCategoriesAsync();

            // Assert
            result.Count.ShouldBe(3);
            result.ShouldContain(c => c.Name == "Category 1" && c.Count == 4);
            result.ShouldContain(c => c.Name == "Category 2" && c.Count == 2);
            result.ShouldContain(c => c.Name == "Category 3" && c.Count == 6);
        }

        [Fact]
        public async Task GetDirectoriesAsync_ReturnsAllDirectories()
        {
            // Arrange
            var directories = new List<DirectoryItem>
            {
                DirectoryItem.Create("dir1", "blog", "Blog", "test-provider"),
                DirectoryItem.Create("dir2", "news", "News", "test-provider"),
                DirectoryItem.Create("dir3", "projects", "Projects", "test-provider")
            };

            _provider.SetupTestDirectories(directories);

            // Act
            var result = await _provider.GetDirectoriesAsync();

            // Assert
            result.Count.ShouldBe(3);
            result.ShouldContain(d => d.Id == "dir1" && d.Name == "Blog");
            result.ShouldContain(d => d.Id == "dir2" && d.Name == "News");
            result.ShouldContain(d => d.Id == "dir3" && d.Name == "Projects");
        }

        [Fact]
        public async Task GetDirectoryByIdAsync_WithValidId_ReturnsDirectory()
        {
            // Arrange
            var directories = new List<DirectoryItem>
            {
                DirectoryItem.Create("dir1", "blog", "Blog", "test-provider"),
                DirectoryItem.Create("dir2", "news", "News", "test-provider")
            };

            _provider.SetupTestDirectories(directories);

            // Act
            var result = await _provider.GetDirectoryByIdAsync("dir1");

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe("dir1");
            result.Name.ShouldBe("Blog");
        }

        [Fact]
        public async Task GetDirectoryByPathAsync_WithValidPath_ReturnsDirectory()
        {
            // Arrange
            var directories = new List<DirectoryItem>
            {
                DirectoryItem.Create("dir1", "blog", "Blog", "test-provider"),
                DirectoryItem.Create("dir2", "news", "News", "test-provider")
            };

            _provider.SetupTestDirectories(directories);

            // Act
            var result = await _provider.GetDirectoryByPathAsync("blog");

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe("dir1");
            result.Name.ShouldBe("Blog");
        }

        [Fact]
        public async Task GetDirectoryByUrlAsync_WithValidUrl_ReturnsDirectory()
        {
            // Arrange
            var directories = new List<DirectoryItem>
            {
                DirectoryItem.Create("dir1", "blog", "Blog", "test-provider"),
                DirectoryItem.Create("dir2", "news", "News", "test-provider")
            };

            directories[0].SetUrl("blog-url");
            directories[1].SetUrl("news-url");

            _provider.SetupTestDirectories(directories);

            // Act
            var result = await _provider.GetDirectoryByUrlAsync("blog-url");

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe("dir1");
            result.Name.ShouldBe("Blog");
        }

        // Test implementation of ContentProviderBase for testing
        public class TestContentProvider : ContentProviderBase
        {
            private List<ContentItem> _testItems = new List<ContentItem>();
            private List<DirectoryItem> _testDirectories = new List<DirectoryItem>();
            private List<ContentTag> _testTags = new List<ContentTag>();
            private List<ContentCategory> _testCategories = new List<ContentCategory>();

            public override string ProviderId => "test-provider";
            public override string DisplayName => "Test Provider";
            public override bool IsReadOnly => true;

            public TestContentProvider(IMemoryCache memoryCache, IOptions<ContentProviderOptions> options, ILogger<TestContentProvider> logger)
                : base(memoryCache, options, logger)
            {
            }

            public void SetupTestItems(List<ContentItem> items)
            {
                _testItems = items;
            }

            public void SetupTestDirectories(List<DirectoryItem> directories)
            {
                _testDirectories = directories;
            }

            public void SetupTestTags(List<ContentTag> tags)
            {
                _testTags = tags;
            }

            public void SetupTestCategories(List<ContentCategory> categories)
            {
                _testCategories = categories;
            }

            public override Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
            {
                return Task.FromResult<IReadOnlyList<ContentItem>>(_testItems);
            }

            public override Task<ContentItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default)
            {
                var item = _testItems.FirstOrDefault(i => i.Id == id);
                return Task.FromResult(item);
            }

            public override Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default)
            {
                var item = _testItems.FirstOrDefault(i => i.Path == path);
                return Task.FromResult(item);
            }

            public override Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default)
            {
                var item = _testItems.FirstOrDefault(i => i.Url == url);
                return Task.FromResult(item);
            }

            public override Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default)
            {
                return Task.FromResult<IReadOnlyList<ContentTag>>(_testTags);
            }

            public override Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default)
            {
                return Task.FromResult<IReadOnlyList<ContentCategory>>(_testCategories);
            }

            public override Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default)
            {
                if (string.IsNullOrWhiteSpace(locale))
                {
                    return Task.FromResult<IReadOnlyList<DirectoryItem>>(_testDirectories);
                }

                var filteredDirectories = _testDirectories
                    .Where(d => string.IsNullOrWhiteSpace(d.Locale) || d.Locale == locale)
                    .ToList();

                return Task.FromResult<IReadOnlyList<DirectoryItem>>(filteredDirectories);
            }

            public override Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default)
            {
                var directory = _testDirectories.FirstOrDefault(d => d.Id == id);

                if (directory is not null && !string.IsNullOrWhiteSpace(locale) &&
                    !string.IsNullOrWhiteSpace(directory.Locale) && directory.Locale != locale)
                {
                    return Task.FromResult<DirectoryItem?>(null);
                }

                return Task.FromResult(directory);
            }

            public override Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default)
            {
                var directory = _testDirectories.FirstOrDefault(d => d.Path == path);
                return Task.FromResult(directory);
            }

            public override Task<DirectoryItem?> GetDirectoryByUrlAsync(string url, CancellationToken cancellationToken = default)
            {
                var directory = _testDirectories.FirstOrDefault(d => d.Url == url);
                return Task.FromResult(directory);
            }

            public override Task<Dictionary<string, ContentItem>> GetContentTranslationsAsync(string localizationId)
            {
                var translations = _testItems
                    .Where(i => i.ContentId == localizationId)
                    .ToDictionary(i => i.Locale);

                return Task.FromResult(translations);
            }

            public override Task<IReadOnlyList<ContentItem>?> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}