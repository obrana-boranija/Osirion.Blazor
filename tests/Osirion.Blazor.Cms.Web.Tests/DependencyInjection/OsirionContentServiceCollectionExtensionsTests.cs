using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Providers;
using Osirion.Blazor.Cms.Web.DependencyInjection;
using Osirion.Blazor.Cms.Web.Options;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.DependencyInjection;

public class OsirionContentServiceCollectionExtensionsTests
{
    [Fact]
    public void AddOsirionContent_WithBuilder_RunsTheBuilderAndRegistersTheCmsServices()
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IConfiguration>(Configuration());
        IContentBuilder? received = null;

        var result = services.AddOsirionContent(builder => received = builder);

        result.ShouldBeSameAs(services);
        received.ShouldNotBeNull();
        received.Services.ShouldBeSameAs(services);
        services.ShouldContain(d => d.ServiceType == typeof(IContentProviderManager));
    }

    [Fact]
    public void AddOsirionContent_WithBuilder_RejectsNullArguments()
    {
        Should.Throw<ArgumentNullException>(() => ((IServiceCollection)null!).AddOsirionContent(_ => { }))
            .ParamName.ShouldBe("services");
        Should.Throw<ArgumentNullException>(() => new ServiceCollection().AddOsirionContent((Action<IContentBuilder>)null!))
            .ParamName.ShouldBe("configure");
    }

    [Fact]
    public void AddOsirionContent_ConfigurationWithoutProviders_RegistersNoContentProvider()
    {
        var services = new ServiceCollection();

        var result = services.AddOsirionContent(Configuration());

        result.ShouldBeSameAs(services);
        services.ShouldContain(d => d.ServiceType == typeof(IContentProviderManager));
        services.ShouldNotContain(d => d.ServiceType == typeof(FileSystemContentProvider));
        services.ShouldNotContain(d => d.IsKeyedService && Equals(d.ServiceKey, "GitHubContentProvider_Main"));
    }

    [Fact]
    public void AddOsirionContent_WebGitHubSection_RegistersAProviderPerEntry()
    {
        var services = new ServiceCollection();

        services.AddOsirionContent(Configuration(new()
        {
            ["Osirion:Cms:Web:GitHub:Main:Owner"] = "owner",
            ["Osirion:Cms:Web:GitHub:Main:Repository"] = "repo"
        }));

        services.ShouldContain(d => d.IsKeyedService && Equals(d.ServiceKey, "GitHubContentProvider_Main"));
    }

    [Theory]
    [InlineData("Osirion:Cms:Web:FileSystem:BasePath")]
    [InlineData("Osirion:Cms:FileSystem:Web:BasePath")]
    public void AddOsirionContent_FileSystemSection_RegistersTheFileSystemProvider(string key)
    {
        var services = new ServiceCollection();

        services.AddOsirionContent(Configuration(new() { [key] = "content" }));

        services.ShouldContain(d => d.ServiceType == typeof(FileSystemContentProvider));
    }

    [Fact]
    public void AddOsirionContent_WithConfiguration_RejectsNullArguments()
    {
        Should.Throw<ArgumentNullException>(() => ((IServiceCollection)null!).AddOsirionContent(Configuration()))
            .ParamName.ShouldBe("services");
        Should.Throw<ArgumentNullException>(() => new ServiceCollection().AddOsirionContent((IConfiguration)null!))
            .ParamName.ShouldBe("configuration");
    }

    [Fact]
    public void AddOsirionSeoMetadata_WithDelegate_ConfiguresTheOptions()
    {
        var services = new ServiceCollection();

        var result = services.AddOsirionSeoMetadata(options => options.SiteName = "Osirion");

        result.ShouldBeSameAs(services);
        services.BuildServiceProvider().GetRequiredService<IOptions<SeoMetadataOptions>>().Value.SiteName.ShouldBe("Osirion");
    }

    [Fact]
    public void AddOsirionSeoMetadata_WithConfiguration_BindsTheOptions()
    {
        var services = new ServiceCollection();

        var result = services.AddOsirionSeoMetadata(Configuration(new() { ["SiteName"] = "Bound", ["TwitterSite"] = "@osirion" }));

        result.ShouldBeSameAs(services);
        var options = services.BuildServiceProvider().GetRequiredService<IOptions<SeoMetadataOptions>>().Value;
        options.SiteName.ShouldBe("Bound");
        options.TwitterSite.ShouldBe("@osirion");
    }

    [Fact]
    public void AddOsirionSeoMetadata_RejectsNullArguments()
    {
        Should.Throw<ArgumentNullException>(() => ((IServiceCollection)null!).AddOsirionSeoMetadata(_ => { }))
            .ParamName.ShouldBe("services");
        Should.Throw<ArgumentNullException>(() => new ServiceCollection().AddOsirionSeoMetadata((Action<SeoMetadataOptions>)null!))
            .ParamName.ShouldBe("configure");
        Should.Throw<ArgumentNullException>(() => ((IServiceCollection)null!).AddOsirionSeoMetadata(Configuration()))
            .ParamName.ShouldBe("services");
        Should.Throw<ArgumentNullException>(() => new ServiceCollection().AddOsirionSeoMetadata((IConfiguration)null!))
            .ParamName.ShouldBe("configuration");
    }

    private static IConfiguration Configuration(Dictionary<string, string?>? values = null) =>
        new ConfigurationBuilder().AddInMemoryCollection(values ?? []).Build();
}
