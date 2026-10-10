using System.Text.Json.Nodes;
using Osirion.Blazor.Core.Utils;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Utils;

public class JsonLdNormalizerTests
{
    [Fact]
    public void TryNormalize_ValidObject_KeepsTheSameJson()
    {
        const string jsonLd = """{"@context":"https://schema.org","@type":"Article","headline":"Hello","wordCount":120,"isFamilyFriendly":true}""";

        JsonLdNormalizer.TryNormalize(jsonLd, out var normalized).ShouldBeTrue();

        JsonNode.DeepEquals(JsonNode.Parse(normalized), JsonNode.Parse(jsonLd)).ShouldBeTrue();
    }

    [Fact]
    public void TryNormalize_TopLevelArray_ReturnsTrue()
    {
        const string jsonLd = """[{"@type":"Organization","name":"A"},{"@type":"WebSite","name":"B"}]""";

        JsonLdNormalizer.TryNormalize(jsonLd, out var normalized).ShouldBeTrue();

        JsonNode.DeepEquals(JsonNode.Parse(normalized), JsonNode.Parse(jsonLd)).ShouldBeTrue();
    }

    // Proves a front matter value cannot end the surrounding <script type="application/ld+json"> element.
    [Theory]
    [InlineData("""{"name":"</script><script>alert(1)</script>"}""")]
    [InlineData("""{"</script><script>alert(1)</script>":"value"}""")]
    [InlineData("""{"name":"<!--<script>"}""")]
    [InlineData("""{"name":"<\/script>"}""")]
    public void TryNormalize_ScriptBreakout_EscapesMarkupCharactersWithoutChangingTheData(string jsonLd)
    {
        JsonLdNormalizer.TryNormalize(jsonLd, out var normalized).ShouldBeTrue();

        normalized.ShouldNotContain("<");
        normalized.ShouldNotContain(">");
        JsonNode.DeepEquals(JsonNode.Parse(normalized), JsonNode.Parse(jsonLd)).ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("{")]
    [InlineData("""{"name":"a",}""")]
    [InlineData("""{"name": "a" /* comment */}""")]
    [InlineData("// comment\n{\"name\":\"a\"}")]
    [InlineData("""</script><script>alert(1)</script>""")]
    [InlineData("""{"name":"a"}</script><script>alert(1)</script>""")]
    [InlineData("\"just a string\"")]
    [InlineData("42")]
    [InlineData("null")]
    public void TryNormalize_NotAJsonObjectOrArray_ReturnsFalse(string? jsonLd)
    {
        JsonLdNormalizer.TryNormalize(jsonLd, out var normalized).ShouldBeFalse();

        normalized.ShouldBeNull();
    }

    // Fails before the fix: half of a surrogate pair, for example an emoji escape cut in two, parsed but
    // threw while being written back, so the page that carried it failed to render.
    [Theory]
    [InlineData("""{"name":"\ud800"}""")]
    [InlineData("""{"headline":"Launch \ud83d"}""")]
    [InlineData("""{"a":"x\uD800y"}""")]
    public void TryNormalize_EscapedLoneSurrogate_ReturnsFalse(string jsonLd)
    {
        JsonLdNormalizer.TryNormalize(jsonLd, out var normalized).ShouldBeFalse();

        normalized.ShouldBeNull();
    }

    // Fails before the fix: a raw lone surrogate threw an ArgumentException from the parser, which only
    // JSON syntax errors were caught for.
    [Fact]
    public void TryNormalize_RawLoneSurrogate_ReturnsFalse()
    {
        var jsonLd = "{\"name\":\"a" + (char)0xD800 + "b\"}";

        JsonLdNormalizer.TryNormalize(jsonLd, out var normalized).ShouldBeFalse();

        normalized.ShouldBeNull();
    }
}
