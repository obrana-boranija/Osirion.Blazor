using System.Text.Json.Nodes;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class SeoMetadataRendererJsonLdTests : CmsWebComponentTestBase
{
    private const string BreakoutJsonLd =
        """{"@context":"https://schema.org","@type":"Article","name":"</script><script>alert(1)</script>"}""";

    /// <summary>Front matter JSON-LD whose string holds only the first half of a surrogate pair.</summary>
    private const string LoneSurrogateJsonLd =
        """{"@context":"https://schema.org","@type":"Article","name":"Launch \ud83d"}""";

    private readonly CapturingLogger<SeoMetadataRenderer> _logger = new();

    public SeoMetadataRendererJsonLdTests()
    {
        Services.AddSingleton<ILogger<SeoMetadataRenderer>>(_logger);
    }

    // Fails before the fix: the front matter value was written verbatim inside the script element, so
    // "</script>" ended it and the following "<script>" element ran.
    [Fact]
    public void SeoMetadataRenderer_CustomJsonLdWithScriptBreakout_RendersNoExtraScriptElement()
    {
        var cut = RenderStatic<SeoMetadataRenderer>(parameters => parameters
            .Add(p => p.Content, CreateItemWithJsonLd(BreakoutJsonLd)));

        cut.FindAll("script").ShouldAllBe(script => script.GetAttribute("type") == "application/ld+json");
    }

    [Fact]
    public void SeoMetadataRenderer_CustomJsonLdWithScriptBreakout_KeepsTheSameStructuredData()
    {
        var cut = RenderStatic<SeoMetadataRenderer>(parameters => parameters
            .Add(p => p.Content, CreateItemWithJsonLd(BreakoutJsonLd)));

        var jsonLd = cut.FindAll("script[type='application/ld+json']").ShouldHaveSingleItem();
        JsonNode.DeepEquals(JsonNode.Parse(jsonLd.TextContent), JsonNode.Parse(BreakoutJsonLd)).ShouldBeTrue();
    }

    [Fact]
    public void SeoMetadataRenderer_InvalidCustomJsonLd_RendersGeneratedStructuredDataInstead()
    {
        var cut = RenderStatic<SeoMetadataRenderer>(parameters => parameters
            .Add(p => p.Content, CreateItemWithJsonLd("</script><script>alert(1)</script>")));

        cut.FindAll("script").ShouldAllBe(script => script.GetAttribute("type") == "application/ld+json");
        var generated = JsonNode.Parse(cut.FindAll("script[type='application/ld+json']").First().TextContent).ShouldNotBeNull();
        generated["@context"]!.GetValue<string>().ShouldBe("https://schema.org");
    }

    [Fact]
    public void SeoMetadataRenderer_InvalidCustomJsonLd_LogsWarningWithContentPath()
    {
        RenderStatic<SeoMetadataRenderer>(parameters => parameters
            .Add(p => p.Content, CreateItemWithJsonLd("{ not json")));

        var entry = _logger.Entries.ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Warning);
        entry.Message.ShouldContain("blog/post.md");
    }

    // Fails before the fix: the normalizer threw on the lone surrogate escape and the exception escaped
    // OnInitialized, so the page failed to render instead of falling back to the generated schema.
    [Fact]
    public void SeoMetadataRenderer_CustomJsonLdWithLoneSurrogate_RendersGeneratedStructuredDataInstead()
    {
        var cut = RenderStatic<SeoMetadataRenderer>(parameters => parameters
            .Add(p => p.Content, CreateItemWithJsonLd(LoneSurrogateJsonLd)));

        var schemas = cut.FindAll("script[type='application/ld+json']");
        var generated = JsonNode.Parse(schemas.First().TextContent).ShouldNotBeNull();
        generated["@context"]!.GetValue<string>().ShouldBe("https://schema.org");
        schemas.ShouldAllBe(schema => !schema.TextContent.Contains("Launch"));
    }

    [Fact]
    public void SeoMetadataRenderer_CustomJsonLdWithLoneSurrogate_LogsWarning()
    {
        RenderStatic<SeoMetadataRenderer>(parameters => parameters
            .Add(p => p.Content, CreateItemWithJsonLd(LoneSurrogateJsonLd)));

        _logger.Entries.ShouldHaveSingleItem().Level.ShouldBe(LogLevel.Warning);
    }

    private static ContentItem CreateItemWithJsonLd(string jsonLd)
    {
        var item = CreateItem("<p>Body</p>");
        item.Metadata = new FrontMatter
        {
            Title = "Post",
            SeoProperties = new SeoMetadata { JsonLd = jsonLd }
        };

        return item;
    }

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Entries.Add((logLevel, formatter(state, exception)));
    }
}
