using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public class ContentProviderRegistryArgumentTests
{
    [Fact]
    public void Constructor_WithNullArguments_Throws()
    {
        var logger = NullLogger<ContentProviderRegistry>.Instance;

        Should.Throw<ArgumentNullException>(() => new ContentProviderRegistry(null!, logger));
        Should.Throw<ArgumentNullException>(() => new ContentProviderRegistry([], null!));
        Should.Throw<ArgumentNullException>(() => new ContentProviderRegistry([], logger, null!));
    }

    [Fact]
    public void GetDefaultProvider_WithoutProviders_ReturnsNull()
    {
        var registry = new ContentProviderRegistry([], NullLogger<ContentProviderRegistry>.Instance);

        registry.GetDefaultProvider().ShouldBeNull();
        registry.GetAllProviders().ShouldBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void GetProviderAndSetDefaultProvider_WithBlankId_Throw(string providerId)
    {
        var provider = Substitute.For<IContentProvider>();
        provider.ProviderId.Returns("p");
        var registry = new ContentProviderRegistry([provider], NullLogger<ContentProviderRegistry>.Instance);

        Should.Throw<ArgumentException>(() => registry.GetProvider(providerId));
        Should.Throw<ArgumentException>(() => registry.SetDefaultProvider(providerId));
        registry.GetDefaultProvider().ShouldBeSameAs(provider);
    }
}
