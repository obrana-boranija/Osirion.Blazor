using Shouldly;

namespace Osirion.Blazor.ApiDocs.Tests;

public class ContentUpdaterTests
{
    private const string Table = "| Parameter |\n| --- |\n| `Id` |\n";

    [Fact]
    public void Update_Section_ReplacesTheTextBetweenTheMarkers()
    {
        var markdown = "Intro\n\n<!-- osirion:parameters OsirionTabs -->\nold table\n<!-- /osirion:parameters -->\n\nOutro";

        var updated = ContentUpdater.Update(markdown, _ => Table, out var unknown);

        updated.ShouldBe("Intro\n\n<!-- osirion:parameters OsirionTabs -->\n\n| Parameter |\n| --- |\n| `Id` |\n\n<!-- /osirion:parameters -->\n\nOutro");
        unknown.ShouldBeEmpty();
    }

    [Fact]
    public void Update_AlreadyCurrent_ReturnsTheSameText()
    {
        var once = ContentUpdater.Update("<!-- osirion:parameters A --><!-- /osirion:parameters -->", _ => Table, out _);

        ContentUpdater.Update(once, _ => Table, out _).ShouldBe(once);
    }

    [Fact]
    public void Update_CrlfFile_KeepsCrlf()
    {
        var updated = ContentUpdater.Update("<!-- osirion:parameters A -->\r\n<!-- /osirion:parameters -->\r\n", _ => Table, out _);

        updated.Replace("\r\n", string.Empty, StringComparison.Ordinal).ShouldNotContain('\n');
    }

    [Fact]
    public void Update_UnknownComponent_LeavesTheSectionAndReportsTheName()
    {
        const string markdown = "<!-- osirion:parameters Missing -->\nkept\n<!-- /osirion:parameters -->";

        var updated = ContentUpdater.Update(markdown, _ => null, out var unknown);

        updated.ShouldBe(markdown);
        unknown.ShouldBe(["Missing"]);
    }

    [Fact]
    public void Update_TwoSections_EachGetsItsOwnTable()
    {
        const string markdown = "<!-- osirion:parameters A -->\n<!-- /osirion:parameters -->\ntext\n<!-- osirion:parameters B -->\n<!-- /osirion:parameters -->";

        var updated = ContentUpdater.Update(markdown, name => $"table {name}", out _);

        updated.ShouldContain("table A");
        updated.ShouldContain("text");
        updated.ShouldContain("table B");
        updated.IndexOf("table A", StringComparison.Ordinal).ShouldBeLessThan(updated.IndexOf("text", StringComparison.Ordinal));
    }
}
