using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

/// <summary>
/// The configured default provider was applied only by ContentProviderInitializer, which runs only when the host calls
/// UseOsirionCms, so without it the first registered provider was the default whatever IsDefault said.
/// </summary>
public class ContentProviderRegistryDefaultTests
{
    [Fact]
    public void ContentProviderRegistry_SecondProviderConfiguredAsDefault_IsTheDefaultWithoutInitializer()
    {
        var registry = new ContentProviderRegistry(
            [CreateProvider("github"), CreateProvider("local")],
            NullLogger<ContentProviderRegistry>.Instance,
            [new DefaultProviderSetter("github", false), new DefaultProviderSetter("local", true)]);

        registry.GetDefaultProvider()!.ProviderId.ShouldBe("local");
    }

    [Fact]
    public void ContentProviderRegistry_DefaultForUnknownProvider_KeepsTheFirstProvider()
    {
        var registry = new ContentProviderRegistry(
            [CreateProvider("github")],
            NullLogger<ContentProviderRegistry>.Instance,
            [new DefaultProviderSetter("missing", true)]);

        registry.GetDefaultProvider()!.ProviderId.ShouldBe("github");
    }

    private static IContentProvider CreateProvider(string id)
    {
        var provider = Substitute.For<IContentProvider>();
        provider.ProviderId.Returns(id);
        return provider;
    }
}
