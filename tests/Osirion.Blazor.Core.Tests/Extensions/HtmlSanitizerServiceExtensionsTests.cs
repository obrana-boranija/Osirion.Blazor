using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Core.Services.Implementations;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Extensions;

public class HtmlSanitizerServiceExtensionsTests
{
    [Fact]
    public void AddOsirionHtmlSanitizer_Default_RegistersOneSharedSanitizer()
    {
        var services = new ServiceCollection();

        services.AddOsirionHtmlSanitizer();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IHtmlContentSanitizer>()
            .ShouldBeSameAs(provider.GetRequiredService<IHtmlContentSanitizer>());
    }

    [Fact]
    public void AddOsirionHtmlSanitizer_CalledTwice_RegistersOnceAndAppliesBothConfigurations()
    {
        var services = new ServiceCollection();

        services.AddOsirionHtmlSanitizer(options => options.AllowElement("chart", "type"));
        services.AddOsirionHtmlSanitizer(options => options.AllowElement("chart", "data"));
        using var provider = services.BuildServiceProvider();

        services.Count(descriptor => descriptor.ServiceType == typeof(IHtmlContentSanitizer)).ShouldBe(1);
        provider.GetRequiredService<IHtmlContentSanitizer>()
            .Sanitize("<chart type=\"bar\" data=\"[[1, 2]]\" onclick=\"alert(1)\"></chart>")
            .MarkupMatches("<chart type=\"bar\" data=\"[[1, 2]]\"></chart>");
    }

    [Fact]
    public void GetHtmlContentSanitizer_NothingRegistered_ReturnsDefaultSanitizer()
    {
        using var provider = new ServiceCollection().BuildServiceProvider();

        provider.GetHtmlContentSanitizer().ShouldBeSameAs(HtmlContentSanitizer.Default);
    }

    [Fact]
    public void GetHtmlContentSanitizer_SanitizerRegistered_ReturnsRegisteredSanitizer()
    {
        var registered = Substitute.For<IHtmlContentSanitizer>();
        using var provider = new ServiceCollection().AddSingleton(registered).BuildServiceProvider();

        provider.GetHtmlContentSanitizer().ShouldBeSameAs(registered);
    }
}
