using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Osirion.Blazor.Cms.Infrastructure.Repositories;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Repositories;

public class ContentRepositorySanitizationTests
{
    private readonly TestContentRepository _repository = new(
        new MarkdownProcessor(new DefaultMarkdownRenderer(), new MarkdownSanitizer(), new HtmlToMarkdownConverter()));

    // Fails before the fix: ingestion rendered markdown without sanitizing, so the cached item carried the payload.
    [Fact]
    public async Task ProcessMarkdownAsync_MarkdownWithScriptAndHandler_StoresSanitizedHtml()
    {
        var item = CreateItem("blog/post.md");
        const string markdown = "---\ntitle: Post\n---\n# Post\n\n<script>alert(1)</script>\n\n<img src=\"/a.png\" onerror=\"alert(2)\">\n";

        await _repository.ProcessAsync(markdown, item);

        item.Content.ShouldNotContain("<script", Case.Insensitive);
        item.Content.ShouldNotContain("onerror", Case.Insensitive);
        item.Content.ShouldContain("<img src=\"/a.png\">");
    }

    // A save writes OriginalMarkdown back when present, otherwise a conversion of the stored HTML. With the
    // HTML now sanitized, losing the source would make a save drop the removed markup from the file for good.
    [Fact]
    public async Task ProcessMarkdownAsync_Markdown_KeepsOriginalSourceNextToSanitizedHtml()
    {
        var item = CreateItem("blog/post.md");

        await _repository.ProcessAsync("---\ntitle: Post\n---\n# Post\n\n<chart type=\"bar\"></chart>\n", item);

        item.OriginalMarkdown.ShouldNotBeNull().ShouldContain("<chart type=\"bar\"></chart>");
    }

    // Fails before the fix: files excluded from markdown rendering (.txt and similar) were stored verbatim.
    [Fact]
    public async Task ProcessMarkdownAsync_RawTextFileWithMarkup_StoresSanitizedHtml()
    {
        var item = CreateItem("notes/readme.txt");

        await _repository.ProcessAsync("<p>Hello</p><img src=\"x.png\" onerror=\"alert(1)\">", item);

        item.Content.ShouldBe("<p>Hello</p><img src=\"x.png\">");
    }

    // Guards the fail-safe default of IMarkdownProcessor.SanitizeHtml: a processor written before that member
    // existed must store raw-extension content as encoded text, never as live HTML.
    [Fact]
    public async Task ProcessMarkdownAsync_RawTextFileWithProcessorWithoutSanitizeHtml_StoresEncodedText()
    {
        var repository = new TestContentRepository(new ProcessorWithoutSanitizeHtml());
        var item = CreateItem("notes/readme.txt");

        await repository.ProcessAsync("<img src=x onerror=alert(1)>", item);

        item.Content.ShouldBe("&lt;img src=x onerror=alert(1)&gt;");
    }

    private static ContentItem CreateItem(string path) =>
        ContentItem.Create("1", "Title", string.Empty, path, "test");

    /// <summary>A processor that implements only the members that existed before SanitizeHtml was added.</summary>
    private sealed class ProcessorWithoutSanitizeHtml : IMarkdownProcessor
    {
        public string RenderToHtml(string markdown, bool sanitizeHtml = true) => markdown;

        public Task<string> RenderToHtmlAsync(string markdown, bool sanitizeHtml = true) => Task.FromResult(markdown);

        public string SanitizeMarkdown(string markdown) => markdown;

        public (FrontMatter? FrontMatter, string Content) ExtractFrontMatterAndContent(string markdown) =>
            (new FrontMatter { Title = "Notes" }, markdown);

        public Task<string> ConvertHtmlToMarkdownAsync(string html) => Task.FromResult(html);

        public Task<string> ConvertHtmlToMarkdownAsync(string html, CancellationToken cancellationToken) => Task.FromResult(html);
    }

    private sealed class TestContentRepository(IMarkdownProcessor markdownProcessor)
        : BaseContentRepository("test", markdownProcessor, Substitute.For<IContentQueryFilter>(), NullLogger.Instance)
    {
        public Task ProcessAsync(string markdown, ContentItem item) =>
            ProcessMarkdownAsync(markdown, item, CancellationToken.None);

        protected override Task<Dictionary<string, ContentItem>> LoadItemsIntoCache(CancellationToken cancellationToken) =>
            Task.FromResult(new Dictionary<string, ContentItem>());

        public override Task<ContentItem> SaveWithCommitMessageAsync(ContentItem entity, string commitMessage, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public override Task DeleteWithCommitMessageAsync(string id, string commitMessage, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }
}
