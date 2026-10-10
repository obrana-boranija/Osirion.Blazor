using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.DependencyInjection;

public class MarkdownExtensionsTests
{
    [Fact]
    public void AddMarkdownServices_ResolvedProcessor_SanitizesRenderedHtml()
    {
        var services = new ServiceCollection();

        services.AddMarkdownServices();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IMarkdownProcessor>()
            .RenderToHtml("<p onclick=\"alert(1)\">text</p>")
            .Trim()
            .ShouldBe("<p>text</p>");
    }

    [Fact]
    public void AddMarkdownServices_RegistersHtmlContentSanitizer()
    {
        var services = new ServiceCollection();

        services.AddMarkdownServices();
        using var provider = services.BuildServiceProvider();

        provider.GetService<IHtmlContentSanitizer>().ShouldNotBeNull();
    }

    [Fact]
    public void AddMarkdownServices_HostAllowList_ReachesResolvedProcessor()
    {
        var services = new ServiceCollection();

        services.AddOsirionHtmlSanitizer(options => options.AllowElement("chart", "type"));
        services.AddMarkdownServices();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IMarkdownProcessor>()
            .RenderToHtml("<chart type=\"line\"></chart>")
            .ShouldContain("<chart type=\"line\"></chart>");
    }
}
