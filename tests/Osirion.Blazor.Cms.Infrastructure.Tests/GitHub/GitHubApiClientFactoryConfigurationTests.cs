using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.GitHub;

/// <summary>
/// Covers how <see cref="GitHubApiClientFactory"/> reads web, admin and legacy provider configuration.
/// </summary>
public class GitHubApiClientFactoryConfigurationTests
{
    private readonly RecordingHttpMessageHandler _handler = new();

    private static IConfiguration Configuration(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }

    private GitHubApiClientFactory CreateFactory(Dictionary<string, string?> values)
    {
        return GitHubTestServices.CreateFactory(_handler, Configuration(values));
    }

    [Fact]
    public void Constructor_WithNullDependencies_Throws()
    {
        var configuration = Configuration([]);
        var services = Substitute.For<IServiceProvider>();
        var logger = NullLogger<GitHubApiClientFactory>.Instance;

        Should.Throw<ArgumentNullException>(() => new GitHubApiClientFactory(null!, configuration, logger));
        Should.Throw<ArgumentNullException>(() => new GitHubApiClientFactory(services, null!, logger));
        Should.Throw<ArgumentNullException>(() => new GitHubApiClientFactory(services, configuration, null!));
    }

    [Fact]
    public async Task GetDefaultClient_WithOnlyAdminProviders_UsesTheAdminDefault()
    {
        var factory = CreateFactory(new()
        {
            ["Osirion:Cms:Admin:GitHub:Editor:Owner"] = "admin-owner",
            ["Osirion:Cms:Admin:GitHub:Editor:Repository"] = "admin-repo",
            ["Osirion:Cms:Admin:GitHub:Editor:ApiToken"] = "admin-token",
            ["Osirion:Cms:Admin:GitHub:Editor:IsDefault"] = "true",
            ["Osirion:Cms:Admin:GitHub:Other:Owner"] = "other",
            ["Osirion:Cms:Admin:GitHub:Other:Repository"] = "other-repo"
        });

        await factory.GetDefaultClient().GetFileContentAsync("a.md");

        var request = _handler.Requests.ShouldHaveSingleItem();
        request.Uri.AbsoluteUri.ShouldBe("https://api.github.com/repos/admin-owner/admin-repo/contents/a.md?ref=main");
        request.AuthorizationParameter.ShouldBe("admin-token");
        factory.GetProviderNames().ShouldBe(["Editor", "Other"], ignoreOrder: true);
    }

    [Fact]
    public async Task GetDefaultClient_WithoutDefaultFlag_UsesTheFirstWebProvider()
    {
        var factory = CreateFactory(new()
        {
            ["Osirion:Cms:Web:GitHub:Site:Owner"] = "site-owner",
            ["Osirion:Cms:Web:GitHub:Site:Repository"] = "site-repo",
            ["Osirion:Cms:Web:GitHub:Site:ApiToken"] = "site-token"
        });

        await factory.GetDefaultClient().GetFileContentAsync("a.md");

        _handler.Requests.ShouldHaveSingleItem().Uri.AbsolutePath.ShouldBe("/repos/site-owner/site-repo/contents/a.md");
    }

    [Fact]
    public async Task GetDefaultClient_WithoutDefaultFlag_UsesTheFirstAdminProviderWhenNoWebProviderExists()
    {
        var factory = CreateFactory(new()
        {
            ["Osirion:Cms:Admin:GitHub:Editor:Owner"] = "admin-owner",
            ["Osirion:Cms:Admin:GitHub:Editor:Repository"] = "admin-repo",
            ["Osirion:Cms:Admin:GitHub:Editor:ApiToken"] = "admin-token"
        });

        await factory.GetDefaultClient().GetFileContentAsync("a.md");

        _handler.Requests.ShouldHaveSingleItem().Uri.AbsolutePath.ShouldBe("/repos/admin-owner/admin-repo/contents/a.md");
    }

    [Fact]
    public void GetDefaultClient_WithoutAnyProvider_Throws()
    {
        var factory = CreateFactory([]);

        Should.Throw<InvalidOperationException>(() => factory.GetDefaultClient()).Message.ShouldBe("No GitHub providers configured");
        factory.GetProviderNames().ShouldBeEmpty();
    }

    [Fact]
    public async Task Constructor_WithOnlyLegacySection_RegistersItAsTheDefaultProvider()
    {
        var factory = CreateFactory(new()
        {
            ["Osirion:Cms:GitHub:Owner"] = "legacy-owner",
            ["Osirion:Cms:GitHub:Repository"] = "legacy-repo",
            ["Osirion:Cms:GitHub:Branch"] = "dev",
            ["Osirion:Cms:GitHub:ApiToken"] = "legacy-token"
        });

        await factory.GetDefaultClient().GetFileContentAsync("a.md");

        factory.GetProviderNames().ShouldBe(["default"]);
        factory.ProviderExists("default").ShouldBeTrue();
        var request = _handler.Requests.ShouldHaveSingleItem();
        request.Uri.AbsoluteUri.ShouldBe("https://api.github.com/repos/legacy-owner/legacy-repo/contents/a.md?ref=dev");
        request.AuthorizationParameter.ShouldBe("legacy-token");
    }

    [Fact]
    public void Constructor_WithNewAndLegacySections_IgnoresTheLegacySection()
    {
        var factory = CreateFactory(new()
        {
            ["Osirion:Cms:Web:GitHub:Site:Owner"] = "o",
            ["Osirion:Cms:Web:GitHub:Site:Repository"] = "r",
            ["Osirion:Cms:GitHub:Owner"] = "legacy-owner",
            ["Osirion:Cms:GitHub:Repository"] = "legacy-repo"
        });

        factory.ProviderExists("default").ShouldBeFalse();
        factory.GetProviderNames().ShouldBe(["Site"]);
    }

    [Fact]
    public async Task GetClient_ForAdminOnlyProvider_UsesTheAdminSettings()
    {
        var factory = GitHubTestServices.CreateFactory(_handler, Configuration(new()
        {
            ["Osirion:Cms:Web:GitHub:Site:Owner"] = "o",
            ["Osirion:Cms:Web:GitHub:Site:Repository"] = "r",
            ["Osirion:Cms:Admin:GitHub:Editor:Owner"] = "admin-owner",
            ["Osirion:Cms:Admin:GitHub:Editor:Repository"] = "admin-repo",
            ["Osirion:Cms:Admin:GitHub:Editor:ApiToken"] = "admin-token"
        }));

        await factory.GetClient("Editor").GetFileContentAsync("a.md");

        _handler.Requests.ShouldHaveSingleItem().Uri.AbsolutePath.ShouldBe("/repos/admin-owner/admin-repo/contents/a.md");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void GetClient_WithEmptyName_Throws(string providerName)
    {
        var factory = GitHubTestServices.CreateFactory(_handler);

        Should.Throw<ArgumentException>(() => factory.GetClient(providerName)).ParamName.ShouldBe("providerName");
    }

    [Fact]
    public void GetClient_ForUnknownProvider_Throws()
    {
        var factory = GitHubTestServices.CreateFactory(_handler);

        Should.Throw<InvalidOperationException>(() => factory.GetClient("Missing")).Message.ShouldContain("'Missing' not found");
    }

    [Theory]
    [InlineData("", "repo", "Owner")]
    [InlineData("owner", "", "Repository")]
    public void GetClient_WithIncompleteProvider_NamesTheMissingSetting(string owner, string repository, string missing)
    {
        var factory = CreateFactory(new()
        {
            ["Osirion:Cms:Web:GitHub:Site:Owner"] = owner,
            ["Osirion:Cms:Web:GitHub:Site:Repository"] = repository
        });

        Should.Throw<InvalidOperationException>(() => factory.GetClient("Site")).Message.ShouldContain($"missing '{missing}'");
    }

    [Fact]
    public void GetProviderOptions_FindsWebThenAdminProviders()
    {
        var factory = CreateFactory(new()
        {
            ["Osirion:Cms:Web:GitHub:Shared:Owner"] = "web-owner",
            ["Osirion:Cms:Admin:GitHub:Shared:Owner"] = "admin-owner",
            ["Osirion:Cms:Admin:GitHub:Editor:Owner"] = "editor-owner"
        });

        factory.GetProviderOptions("Shared").ShouldNotBeNull().Owner.ShouldBe("web-owner");
        factory.GetProviderOptions("Editor").ShouldNotBeNull().Name.ShouldBe("Editor");
        factory.GetProviderOptions("Missing").ShouldBeNull();
        factory.GetProviderOptions(" ").ShouldBeNull();
    }

    [Fact]
    public void GetAllProviderOptions_PrefersWebSettingsForSharedNames()
    {
        var factory = CreateFactory(new()
        {
            ["Osirion:Cms:Web:GitHub:Shared:Owner"] = "web-owner",
            ["Osirion:Cms:Admin:GitHub:Shared:Owner"] = "admin-owner",
            ["Osirion:Cms:Admin:GitHub:Editor:Owner"] = "editor-owner"
        });

        var all = factory.GetAllProviderOptions().ToDictionary(pair => pair.Key, pair => pair.Value.Owner);

        all.ShouldBe(new Dictionary<string, string> { ["Shared"] = "web-owner", ["Editor"] = "editor-owner" }, ignoreOrder: true);
        factory.ProviderExists("Editor").ShouldBeTrue();
        factory.ProviderExists("Nope").ShouldBeFalse();
    }
}
