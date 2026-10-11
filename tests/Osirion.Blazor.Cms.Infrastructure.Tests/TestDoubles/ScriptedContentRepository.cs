using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Infrastructure.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;

/// <summary>
/// A content repository whose cache load is scripted by the test and whose protected helpers are exposed, so the
/// shared behavior of <see cref="BaseContentRepository"/> can be checked without a real content source.
/// </summary>
internal sealed class ScriptedContentRepository : BaseContentRepository
{
    public ScriptedContentRepository(IMarkdownProcessor? markdownProcessor = null, IContentQueryFilter? queryFilter = null)
        : base(
            "scripted",
            markdownProcessor ?? Substitute.For<IMarkdownProcessor>(),
            queryFilter ?? Substitute.For<IContentQueryFilter>(),
            NullLogger.Instance)
    {
    }

    /// <summary>Gets or sets what the next cache load returns; <see langword="null"/> simulates a provider returning null.</summary>
    public Func<Dictionary<string, ContentItem>?> Loader { get; set; } = () => new Dictionary<string, ContentItem>();

    /// <summary>Gets or sets an exception that the cache check throws, to drive the read methods' error paths.</summary>
    public Exception? EnsureCacheFailure { get; set; }

    public int LoadCount { get; private set; }

    public List<(ContentItem Item, string Message)> Saved { get; } = new();

    public List<(string Id, string Message)> Deleted { get; } = new();

    public void ConfigureLocalization(string contentPath, params string[] supportedLocales)
    {
        EnableLocalization = true;
        ContentPath = contentPath;
        SupportedLocales = supportedLocales.ToList();
    }

    public Task<string> GenerateAsync(ContentItem item) => GenerateMarkdownWithFrontMatterAsync(item);

#pragma warning disable CS0618 // The obsolete synchronous wrapper is still public surface and must keep its output.
    public string Generate(ContentItem item) => GenerateMarkdownWithFrontMatter(item);
#pragma warning restore CS0618

    public string Escape(string value) => EscapeYamlString(value);

    public bool IsStandardKey(string key) => IsStandardFrontMatterKey(key);

    public void AssignLocalizationId(ContentItem item) => AssignDefaultLocalizationId(item);

    public Task ProcessAsync(string markdown, ContentItem item) => ProcessMarkdownAsync(markdown, item, CancellationToken.None);

    public override Task<ContentItem> SaveWithCommitMessageAsync(ContentItem entity, string commitMessage, CancellationToken cancellationToken = default)
    {
        Saved.Add((entity, commitMessage));
        return Task.FromResult(entity);
    }

    public override Task DeleteWithCommitMessageAsync(string id, string commitMessage, CancellationToken cancellationToken = default)
    {
        Deleted.Add((id, commitMessage));
        return Task.CompletedTask;
    }

    protected override Task EnsureCacheIsLoaded(CancellationToken cancellationToken, bool forceRefresh = false)
    {
        if (EnsureCacheFailure is not null)
        {
            throw EnsureCacheFailure;
        }

        return base.EnsureCacheIsLoaded(cancellationToken, forceRefresh);
    }

    protected override Task<Dictionary<string, ContentItem>> LoadItemsIntoCache(CancellationToken cancellationToken)
    {
        LoadCount++;
        return Task.FromResult(Loader()!);
    }
}
