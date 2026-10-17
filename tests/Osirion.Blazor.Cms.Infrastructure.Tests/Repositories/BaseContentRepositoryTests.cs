using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Osirion.Blazor.Cms.Infrastructure.Repositories;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Repositories;

public class BaseContentRepositoryTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    private static IMarkdownProcessor CreateProcessor() =>
        new MarkdownProcessor(new DefaultMarkdownRenderer(), new MarkdownSanitizer(), new HtmlToMarkdownConverter());

    // Fails before the fix: a reader that arrived while the first load was running returned at once with an empty cache.
    [Fact]
    public async Task GetAllAsync_WhileColdLoadIsRunning_WaitsForTheLoad()
    {
        var repository = new GatedRepository();
        var first = repository.GetAllAsync();
        await repository.WaitForLoadStartAsync(1);

        var second = repository.GetAllAsync();
        await Task.Delay(50);
        second.IsCompleted.ShouldBeFalse();

        repository.ReleaseLoad(1, Item("a"));

        (await first.WaitAsync(Timeout)).Count.ShouldBe(1);
        (await second.WaitAsync(Timeout)).Count.ShouldBe(1);
        repository.LoadCount.ShouldBe(1);
    }

    // Fails before the fix: a refresh requested during a refresh returned without ever reloading.
    [Fact]
    public async Task RefreshCacheAsync_RequestedDuringARefresh_RunsOneMoreLoadAfterwards()
    {
        var repository = new GatedRepository();
        var firstRefresh = repository.RefreshCacheAsync();
        await repository.WaitForLoadStartAsync(1);

        var secondRefresh = repository.RefreshCacheAsync();
        var thirdRefresh = repository.RefreshCacheAsync();
        repository.ReleaseLoad(1, Item("old"));
        await repository.WaitForLoadStartAsync(2);
        repository.ReleaseLoad(2, Item("new"));

        await Task.WhenAll(firstRefresh, secondRefresh, thirdRefresh).WaitAsync(Timeout);

        repository.LoadCount.ShouldBe(2);
        var items = await repository.GetAllAsync();
        items.Single().Id.ShouldBe("new");
    }

    [Fact]
    public async Task RefreshCacheAsync_WhenLoadFails_KeepsThePreviousCache()
    {
        var repository = new GatedRepository();
        var initial = repository.RefreshCacheAsync();
        await repository.WaitForLoadStartAsync(1);
        repository.ReleaseLoad(1, Item("kept"));
        await initial.WaitAsync(Timeout);

        var failing = repository.RefreshCacheAsync();
        await repository.WaitForLoadStartAsync(2);
        repository.FailLoad(2, new InvalidOperationException("rate limited"));
        await failing.WaitAsync(Timeout);

        repository.LastRefreshSucceeded.ShouldBeFalse();

        (await repository.GetAllAsync()).Single().Id.ShouldBe("kept");
    }

    [Fact]
    public async Task RefreshCacheAsync_CancellingOneCaller_DoesNotCancelTheSharedLoad()
    {
        var repository = new GatedRepository();
        using var cts = new CancellationTokenSource();
        var cancelled = repository.RefreshCacheAsync(cts.Token);
        await repository.WaitForLoadStartAsync(1);
        var patient = repository.GetAllAsync();

        await cts.CancelAsync();
        await Should.ThrowAsync<OperationCanceledException>(() => cancelled.WaitAsync(Timeout));
        repository.ReleaseLoad(1, Item("a"));
        await patient.WaitAsync(Timeout);
        // The load itself must still have been given a token that the cancelled caller cannot cancel.
        repository.LastLoadToken.CanBeCanceled.ShouldBeFalse();

        (await repository.GetAllAsync()).Count.ShouldBe(1);
    }

    // Fails before the fix: a refresh whose token was already cancelled still started a load, so the next refresh
    // joined that load and had to wait for a second one; and when the load finished first, the cancelled call
    // completed instead of throwing.
    [Fact]
    public async Task RefreshCacheAsync_TokenAlreadyCancelled_ThrowsWithoutStartingALoad()
    {
        var repository = new GatedRepository();

        await Should.ThrowAsync<OperationCanceledException>(() => repository.RefreshCacheAsync(new CancellationToken(true)));
        var refresh = repository.RefreshCacheAsync();
        await repository.WaitForLoadStartAsync(1);
        repository.ReleaseLoad(1, Item("a"));
        await refresh.WaitAsync(Timeout);

        repository.LoadCount.ShouldBe(1);
    }

    // Fails before the fix: slug, lang and featured_image were written without escaping.
    [Fact]
    public async Task GenerateMarkdown_SlugWithQuoteAndNewline_CannotInjectMetadata()
    {
        var repository = new GatedRepository();
        var item = ContentItem.Create("1", "Title", string.Empty, "blog/post.md", "test");
        item.Slug = "x\"\nadmin: true\nslug: \"y";
        item.Locale = "en\"\nrole: admin";
        item.FeaturedImageUrl = "/a.png\"\nlayout: evil";

        var markdown = await repository.GenerateAsync(item);

        var lines = markdown.Split('\n');
        lines.ShouldNotContain(line => line.StartsWith("admin:", StringComparison.Ordinal));
        lines.ShouldNotContain(line => line.StartsWith("role:", StringComparison.Ordinal));
        lines.ShouldNotContain(line => line.StartsWith("layout:", StringComparison.Ordinal));

        var parsed = CreateProcessor().ExtractFrontMatterAndContent(markdown).FrontMatter.ShouldNotBeNull();
        parsed.Slug.ShouldBe("x\"\nadmin: true\nslug: \"y");
        parsed.Lang.ShouldBe("en\"\nrole: admin");
        parsed.FeaturedImage.ShouldBe("/a.png\"\nlayout: evil");
    }

    [Fact]
    public void EscapeYamlString_ControlAndLineSeparatorCharacters_AreEscaped()
    {
        var repository = new GatedRepository();

        var escaped = repository.Escape("a" + (char)0 + "b" + (char)0x85 + "c" + (char)0x2028 + "d" + (char)0x2029 + "e");

        escaped.ShouldBe("a\\u0000b\\u0085c\\u2028d\\u2029e");
    }

    private static ContentItem Item(string id) => ContentItem.Create(id, "Title", "Body", id + ".md", "test");

    private sealed class GatedRepository()
        : BaseContentRepository("test", CreateProcessor(), Substitute.For<IContentQueryFilter>(), NullLogger.Instance)
    {
        private readonly object _gate = new();
        private readonly Dictionary<int, TaskCompletionSource<Dictionary<string, ContentItem>>> _loads = new();
        private readonly Dictionary<int, TaskCompletionSource> _started = new();
        private int _loadCount;

        public int LoadCount => Volatile.Read(ref _loadCount);

        public CancellationToken LastLoadToken { get; private set; }

        public Task<string> GenerateAsync(ContentItem item) => GenerateMarkdownWithFrontMatterAsync(item);

        public string Escape(string value) => EscapeYamlString(value);

        public Task WaitForLoadStartAsync(int number) => Started(number).Task.WaitAsync(Timeout);

        public void ReleaseLoad(int number, params ContentItem[] items) =>
            Load(number).SetResult(items.ToDictionary(item => item.Id));

        public void FailLoad(int number, Exception exception) => Load(number).SetException(exception);

        protected override Task<Dictionary<string, ContentItem>> LoadItemsIntoCache(CancellationToken cancellationToken)
        {
            var number = Interlocked.Increment(ref _loadCount);
            LastLoadToken = cancellationToken;
            var load = Load(number);
            Started(number).TrySetResult();
            return load.Task;
        }

        public override Task<ContentItem> SaveWithCommitMessageAsync(ContentItem entity, string commitMessage, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public override Task DeleteWithCommitMessageAsync(string id, string commitMessage, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        private TaskCompletionSource<Dictionary<string, ContentItem>> Load(int number)
        {
            lock (_gate)
            {
                if (!_loads.TryGetValue(number, out var load))
                {
                    _loads[number] = load = new TaskCompletionSource<Dictionary<string, ContentItem>>(TaskCreationOptions.RunContinuationsAsynchronously);
                }

                return load;
            }
        }

        private TaskCompletionSource Started(int number)
        {
            lock (_gate)
            {
                if (!_started.TryGetValue(number, out var started))
                {
                    _started[number] = started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                }

                return started;
            }
        }
    }
}
