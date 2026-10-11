using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Models;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Cards;

public class OsirionAuthorCardTests : OsirionComponentTestBase
{
    public OsirionAuthorCardTests() => SetRendererInfo(new RendererInfo("Static", false));

    [Fact]
    public void OsirionAuthorCard_AllDetails_RendersAvatarNameRoleBioAndLinks()
    {
        var cut = Render<OsirionAuthorCard>(p => p
            .Add(c => c.Name, "Jane Developer")
            .Add(c => c.Role, "Principal engineer")
            .Add(c => c.Bio, "Writes about Blazor.")
            .Add(c => c.AvatarUrl, "/img/jane.jpg")
            .Add(c => c.ProfileUrl, "/authors/jane")
            .Add(c => c.Links, [new AuthorLink("GitHub", "https://github.com/jane"), new AuthorLink("Email", "mailto:jane@example.com")]));

        var avatar = cut.Find("img.osirion-author-card-avatar");
        avatar.GetAttribute("src").ShouldBe("/img/jane.jpg");
        avatar.GetAttribute("alt").ShouldBe(string.Empty);
        var name = cut.Find(".osirion-author-card-name a");
        name.GetAttribute("href").ShouldBe("/authors/jane");
        name.GetAttribute("rel").ShouldBe("author");
        name.TextContent.ShouldBe("Jane Developer");
        cut.Find(".osirion-author-card-role").TextContent.ShouldBe("Principal engineer");
        cut.Find(".osirion-author-card-bio").TextContent.ShouldBe("Writes about Blazor.");
        var links = cut.FindAll(".osirion-author-card-links a");
        links.Select(a => a.GetAttribute("href")).ShouldBe(["https://github.com/jane", "mailto:jane@example.com"]);
        links.ShouldAllBe(a => a.GetAttribute("rel") == "me");
    }

    [Fact]
    public void OsirionAuthorCard_NameOnly_ShowsInitialsAndNoOptionalParts()
    {
        var cut = Render<OsirionAuthorCard>(p => p.Add(c => c.Name, "jane van developer"));

        cut.Find(".osirion-author-card-initials").TextContent.ShouldBe("JV");
        cut.FindAll("img").ShouldBeEmpty();
        cut.FindAll(".osirion-author-card-name a").ShouldBeEmpty();
        cut.FindAll(".osirion-author-card-role, .osirion-author-card-bio, .osirion-author-card-links").ShouldBeEmpty();
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("//evil.example/x")]
    [InlineData("data:text/html,<script>alert(1)</script>")]
    public void OsirionAuthorCard_UnsafeUrls_AreNotRendered(string url)
    {
        var cut = Render<OsirionAuthorCard>(p => p
            .Add(c => c.Name, "Jane")
            .Add(c => c.AvatarUrl, url)
            .Add(c => c.ProfileUrl, url)
            .Add(c => c.Links, [new AuthorLink("Site", url)]));

        cut.FindAll("img").ShouldBeEmpty();
        cut.FindAll("a").ShouldBeEmpty();
    }
}
