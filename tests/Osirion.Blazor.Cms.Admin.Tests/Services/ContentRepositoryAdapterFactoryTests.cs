using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Services;

public class ContentRepositoryAdapterFactoryTests
{
    private readonly IGitHubAdminService _service = Substitute.For<IGitHubAdminService>();

    private ContentRepositoryAdapterFactory CreateFactory(string defaultProvider = "GitHub") =>
        new(_service, NullLogger<GitHubRepositoryAdapter>.Instance, Options.Create(new CmsAdminOptions { DefaultContentProvider = defaultProvider }));

    [Theory]
    [InlineData("github")]
    [InlineData("GitHub")]
    [InlineData("GITHUB")]
    public void CreateAdapter_GitHubInAnyCase_ReturnsAGitHubAdapterOverTheService(string providerType)
    {
        var adapter = CreateFactory().CreateAdapter(providerType);

        adapter.ShouldBeOfType<GitHubRepositoryAdapter>();
        adapter.SetBranch("main");
        _service.Received(1).SetBranch("main");
    }

    [Fact]
    public void CreateAdapter_UnknownProvider_ThrowsNamingTheProvider()
    {
        var exception = Should.Throw<ArgumentException>(() => CreateFactory().CreateAdapter("gitlab"));

        exception.Message.ShouldContain("gitlab");
    }

    [Fact]
    public void CreateDefaultAdapter_UsesTheConfiguredProvider()
    {
        CreateFactory("GitHub").CreateDefaultAdapter().ShouldBeOfType<GitHubRepositoryAdapter>();
    }

    [Fact]
    public void CreateDefaultAdapter_UnsupportedConfiguredProvider_Throws()
    {
        Should.Throw<ArgumentException>(() => CreateFactory("FileSystem").CreateDefaultAdapter());
    }
}
