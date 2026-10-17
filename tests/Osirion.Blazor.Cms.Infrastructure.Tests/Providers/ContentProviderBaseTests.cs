using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Providers;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Providers;

public class ContentProviderBaseTests
{
    private readonly RecordingLogger<TestProvider> _logger = new();
    private readonly TestProvider _provider;

    public ContentProviderBaseTests()
    {
        _provider = new TestProvider(_logger);
    }

    [Fact]
    public void Constructor_WithNullLogger_Throws()
    {
        // Act
        var act = () => new TestProvider(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("logger");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task GetContentTranslationsAsync_WithBlankId_ReturnsEmptyWithoutQuerying(string localizationId)
    {
        // Act
        var result = await _provider.GetContentTranslationsAsync(localizationId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull().ShouldBeEmpty();
        _provider.Queries.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetContentTranslationsAsync_QueriesByLocalizationId()
    {
        // Arrange
        var english = Item("en-1", "en");
        _provider.QueryResult = [english];
        using var cancellation = new CancellationTokenSource();

        // Act
        var result = await _provider.GetContentTranslationsAsync("loc-1", cancellation.Token);

        // Assert
        result.ShouldBe([english]);
        _provider.Queries.ShouldHaveSingleItem().LocalizationId.ShouldBe("loc-1");
        _provider.LastToken.ShouldBe(cancellation.Token);
    }

    [Fact]
    public async Task GetContentTranslationsAsync_WhenTheQueryFails_LogsAndReturnsEmpty()
    {
        // Arrange
        _provider.QueryException = new InvalidOperationException("broken");

        // Act
        var result = await _provider.GetContentTranslationsAsync("loc-1", CancellationToken.None);

        // Assert
        result.ShouldNotBeNull().ShouldBeEmpty();
        _logger.Entries.ShouldContain(e => e.Level == LogLevel.Error && e.Exception == _provider.QueryException);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetContentTranslationsDictionary_WithBlankId_ReturnsEmptyWithoutQuerying(string localizationId)
    {
        // Act
        var result = await _provider.GetContentTranslationsAsync(localizationId);

        // Assert
        result.ShouldBeEmpty();
        _provider.Queries.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetContentTranslationsDictionary_KeysTranslationsByLocale()
    {
        // Arrange
        var english = Item("en-1", "en");
        var french = Item("fr-1", "fr");
        _provider.QueryResult = [english, french];

        // Act
        var result = await _provider.GetContentTranslationsAsync("loc-1");

        // Assert
        result.Count.ShouldBe(2);
        result["en"].ShouldBeSameAs(english);
        result["fr"].ShouldBeSameAs(french);
    }

    [Fact]
    public async Task GetContentTranslationsDictionary_WhenTheQueryReturnsNull_ReturnsEmpty()
    {
        // Arrange
        _provider.QueryResult = null;

        // Act
        var result = await _provider.GetContentTranslationsAsync("loc-1");

        // Assert
        result.ShouldBeEmpty();
        _provider.Queries.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task GetContentTranslationsDictionary_WithTwoItemsInOneLocale_KeepsTheFirstAndLogsOnce()
    {
        // Fails before the fix: the duplicate key threw, and the translations came back empty.
        var first = Item("en-1", "en");
        var french = Item("fr-1", "fr");
        _provider.QueryResult = [first, Item("en-2", "en"), french, Item("en-3", "en")];

        var result = await _provider.GetContentTranslationsAsync("loc-1");

        result.Count.ShouldBe(2);
        result["en"].ShouldBeSameAs(first);
        result["fr"].ShouldBeSameAs(french);
        _logger.Entries.Count(e => e.Level == LogLevel.Warning).ShouldBe(1);
    }

    [Fact]
    public async Task RefreshCacheAsync_OnTheBaseClass_OnlyLogs()
    {
        // Act
        await _provider.RefreshCacheAsync();

        // Assert
        _logger.Contains(LogLevel.Debug, "RefreshCacheAsync called on base provider: test-provider").ShouldBeTrue();
        _provider.Queries.ShouldBeEmpty();
    }

    [Fact]
    public async Task InitializeAsync_OnTheBaseClass_LogsTheProvider()
    {
        // Act
        await _provider.InitializeAsync();

        // Assert
        _logger.Contains(LogLevel.Information, "Initializing content provider: test-provider").ShouldBeTrue();
    }

    [Fact]
    public async Task ExecuteWithExceptionHandlingAsync_ReturnsTheOperationResult()
    {
        // Act
        var result = await _provider.RunAsync(() => Task.FromResult<string?>("done"), "Load");

        // Assert
        result.ShouldBe("done");
        _logger.Entries.ShouldBeEmpty();
    }

    [Fact]
    public async Task ExecuteWithExceptionHandlingAsync_RethrowsContentProviderExceptionsUnchanged()
    {
        // Arrange
        var original = new ContentProviderException("already wrapped", "test-provider");

        // Act
        var thrown = await Should.ThrowAsync<ContentProviderException>(
            () => _provider.RunAsync(() => Task.FromException<string?>(original), "Load"));

        // Assert
        thrown.ShouldBeSameAs(original);
        _logger.Entries.ShouldBeEmpty();
    }

    [Fact]
    public async Task ExecuteWithExceptionHandlingAsync_WrapsOtherExceptionsAndLogsThem()
    {
        // Arrange
        var original = new TimeoutException("too slow");

        // Act
        var thrown = await Should.ThrowAsync<ContentProviderException>(
            () => _provider.RunAsync(() => Task.FromException<string?>(original), "Load"));

        // Assert
        thrown.Message.ShouldContain("Error in Load for provider Test Provider: too slow");
        thrown.InnerException.ShouldBeSameAs(original);
        _logger.Entries.ShouldContain(e => e.Level == LogLevel.Error && e.Exception == original);
    }

    [Fact]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        // Arrange
        _provider.Dispose();

        // Act
        var act = () => _provider.Dispose();

        // Assert
        act.ShouldNotThrow();
    }

    private static ContentItem Item(string id, string locale)
    {
        var item = ContentItem.Create(id, id, "body", $"{id}.md", "test-provider");
        item.Locale = locale;
        return item;
    }

    /// <summary>
    /// A minimal provider that records the queries the base class sends and returns a configured result.
    /// </summary>
    private sealed class TestProvider : ContentProviderBase
    {
        public TestProvider(ILogger? logger)
            : base(new MemoryCache(new MemoryCacheOptions()), Options.Create(new ContentProviderOptions()), logger!)
        {
        }

        public List<ContentQuery> Queries { get; } = [];

        public CancellationToken LastToken { get; private set; }

        public IReadOnlyList<ContentItem>? QueryResult { get; set; } = [];

        public Exception? QueryException { get; set; }

        public override string ProviderId => "test-provider";

        public override string DisplayName => "Test Provider";

        public override bool IsReadOnly => true;

        public Task<string?> RunAsync(Func<Task<string?>> operation, string operationName)
        {
            return ExecuteWithExceptionHandlingAsync(operation, operationName);
        }

        public override Task<IReadOnlyList<ContentItem>?> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
        {
            Queries.Add(query);
            LastToken = cancellationToken;

            return QueryException is null
                ? Task.FromResult(QueryResult)
                : Task.FromException<IReadOnlyList<ContentItem>?>(QueryException);
        }

        public override Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public override Task<ContentItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public override Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public override Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public override Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public override Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public override Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public override Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public override Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public override Task<DirectoryItem?> GetDirectoryByUrlAsync(string url, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
