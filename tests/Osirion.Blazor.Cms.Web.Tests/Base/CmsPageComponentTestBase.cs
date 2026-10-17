using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Web.Tests.Base;

/// <summary>
/// Base class for tests of the page and view components: a default provider the test can program, a navigation
/// manager that counts not-found signals and a logger factory that records failures.
/// </summary>
public abstract class CmsPageComponentTestBase : CmsWebComponentTestBase
{
    private readonly RecordingLoggerFactory _logs = new();

    protected CmsPageComponentTestBase()
    {
        Services.AddSingleton(Manager);
        Services.AddSingleton<NavigationManager>(Navigation);
        Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new Osirion.Blazor.Cms.Web.Options.SeoMetadataOptions()));
        _logs.Register(Services);
        Manager.GetDefaultProvider().Returns(Provider);
        Manager.GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>>([]));
        Provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>()).Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([]));
    }

    protected IContentProvider Provider { get; } = Substitute.For<IContentProvider>();

    protected IContentProviderManager Manager { get; } = Substitute.For<IContentProviderManager>();

    internal RecordingNavigationManager Navigation { get; } = new();

    protected static ContentItem Doc(string name, string content = "<p>Body</p>")
    {
        var item = ContentItem.Create($"id-{name}", name, content, $"docs/{name}.md", "test");
        item.SetUrl($"docs/{name}");
        return item;
    }
}
