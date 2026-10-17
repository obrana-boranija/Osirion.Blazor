using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Examples.Shared.Docs;
using Shouldly;

namespace Osirion.Blazor.Examples.Tests.Site;

/// <summary>Requests the example site, hosted in memory by <see cref="SiteFactory"/>.</summary>
public partial class SiteTests(SiteFactory factory) : IClassFixture<SiteFactory>
{
    /// <summary>Every address the documentation had before it moved, and where it lives now.</summary>
    public static TheoryData<string, string> MovedAddresses()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "MovedAddresses.json");
        var map = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path))!;
        var data = new TheoryData<string, string>();
        foreach (var (old, current) in map)
        {
            data.Add(old, current);
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(MovedAddresses))]
    public async Task MovedAddress_RedirectsPermanentlyToAPageThatAnswers(string oldAddress, string newAddress)
    {
        // Arrange
        using var client = CreateClient();

        // Act
        using var redirect = await client.GetAsync(oldAddress);
        using var page = await client.GetAsync(newAddress);

        // Assert
        redirect.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
        redirect.Headers.Location?.OriginalString.ShouldBe(newAddress);
        page.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Outline_PlacesEveryDocumentationPage()
    {
        // Act
        var outline = await LoadOutlineAsync();

        // Assert
        outline.Pages.Count.ShouldBeGreaterThan(50);
        outline.Unplaced.Select(DocsOutline.UrlOf).ShouldBeEmpty();
        foreach (var group in DocsOutline.Groups.Where(group => group.PageUrl is not null))
        {
            outline.Find(group.PageUrl!).ShouldNotBeNull($"the {group.Key} group's page {group.PageUrl}");
        }
    }

    [Fact]
    public async Task DocumentationPages_LinkOnlyToAddressesThatAnswerDirectly()
    {
        // Arrange
        using var client = CreateClient();
        var outline = await LoadOutlineAsync();
        var linkedFrom = new Dictionary<string, string>(StringComparer.Ordinal);
        outline.Pages.ShouldNotBeEmpty();

        // Act
        var failures = new List<string>();
        foreach (var page in outline.Pages.Select(DocsOutline.UrlOf))
        {
            var html = await GetPageAsync(client, page, failures);
            foreach (var link in InternalLinks(html))
            {
                linkedFrom.TryAdd(link, page);
            }
        }

        foreach (var (link, source) in linkedFrom)
        {
            using var response = await client.GetAsync(link);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                failures.Add($"{link} answers {(int)response.StatusCode}, linked from {source}");
            }
        }

        // Assert
        failures.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("/components/no-such-component")]
    [InlineData("/components/Tabs")]
    [InlineData("/docs/no-such-page")]
    [InlineData("/en/docs/getting-started")]
    public async Task AddressWithoutAPage_AnswersNotFound(string address)
    {
        // Arrange
        using var client = CreateClient();

        // Act
        using var response = await client.GetAsync(address);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Gallery_ShowsEveryComponentGroupWithAnAnchorAndALinkToEachPage()
    {
        // Arrange
        using var client = CreateClient();
        var outline = await LoadOutlineAsync();

        // Act
        var html = await client.GetStringAsync(DocsOutline.GalleryUrl);

        // Assert
        foreach (var section in outline.Sections.Where(section => section.Group.InGallery && section.Members.Count > 0))
        {
            html.ShouldContain($"id=\"{section.Group.Key}\"");
            foreach (var member in section.Members)
            {
                html.ShouldContain($"href=\"{DocsOutline.UrlOf(member)}\"");
            }
        }
    }

    [Fact]
    public async Task ComponentPage_SidebarMarksThePageAndOpensOnlyItsGroup()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var html = await client.GetStringAsync("/components/tabs");
        var sidebar = SideNavigation().Match(html).Value;

        // Assert
        sidebar.ShouldContain("href=\"/components/tabs\" class=\"osirion-sidenav-link osirion-sidenav-active\" aria-current=\"page\"");
        sidebar.ShouldContain("href=\"/components/callout\"");
        sidebar.ShouldContain("href=\"/components/cards\"");
        sidebar.ShouldNotContain("href=\"/components/feature-card\"");
    }

    private HttpClient CreateClient() =>
        factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    private async Task<DocsOutline> LoadOutlineAsync()
    {
        var provider = factory.Services.GetRequiredService<IContentProviderManager>().GetDefaultProvider();
        provider.ShouldNotBeNull();
        return DocsOutline.Build(await provider.GetAllItemsAsync());
    }

    private static async Task<string> GetPageAsync(HttpClient client, string address, List<string> failures)
    {
        using var response = await client.GetAsync(address);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            failures.Add($"{address} answers {(int)response.StatusCode}");
            return string.Empty;
        }

        return await response.Content.ReadAsStringAsync();
    }

    // Site addresses in href attributes, leaving out code samples, which show markup as text, and static files.
    private static IEnumerable<string> InternalLinks(string html)
    {
        var withoutCode = CodeSample().Replace(html, string.Empty);
        foreach (Match match in Href().Matches(withoutCode))
        {
            var href = WebUtility.HtmlDecode(match.Groups["href"].Value);
            var path = href.Split('#', '?')[0];
            if (path.Length > 0 && !path.StartsWith("//", StringComparison.Ordinal) && !path.StartsWith("/_", StringComparison.Ordinal)
                && !Path.HasExtension(path))
            {
                yield return path;
            }
        }
    }

    [GeneratedRegex("""<pre\b.*?</pre>|<code\b.*?</code>""", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex CodeSample();

    [GeneratedRegex(""""<a\b[^>]*\shref="(?<href>/[^"]*)"""", RegexOptions.IgnoreCase)]
    private static partial Regex Href();

    [GeneratedRegex("""<nav class="osirion-document-sidenav".*?</nav>""", RegexOptions.Singleline)]
    private static partial Regex SideNavigation();
}
