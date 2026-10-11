using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Builders;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Providers;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Builders;

public class ContentBuilderTests
{
    private readonly ServiceCollection _services = new();

    private ContentBuilder CreateBuilder(params (string Key, string Value)[] values)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values.ToDictionary(pair => pair.Key, pair => (string?)pair.Value))
            .Build();
        _services.AddLogging();
        return new ContentBuilder(_services, configuration, NullLogger<ContentBuilder>.Instance);
    }

    private static DefaultProviderSetter SingleSetter(IServiceProvider provider) =>
        provider.GetServices<IDefaultProviderSetter>().OfType<DefaultProviderSetter>().Single();

    [Fact]
    public void Constructor_WithNullArguments_Throws()
    {
        var configuration = new ConfigurationBuilder().Build();
        var logger = NullLogger<ContentBuilder>.Instance;

        Should.Throw<ArgumentNullException>(() => new ContentBuilder(null!, configuration, logger));
        Should.Throw<ArgumentNullException>(() => new ContentBuilder(_services, null!, logger));
        Should.Throw<ArgumentNullException>(() => new ContentBuilder(_services, configuration, null!));
    }

    [Fact]
    public void Constructor_ExposesTheServiceCollection()
    {
        var builder = CreateBuilder();

        builder.Services.ShouldBeSameAs(_services);
    }

    [Fact]
    public void AddGitHub_WithoutDelegate_BindsTheGitHubSection()
    {
        var builder = CreateBuilder(
            ("Osirion:Cms:GitHub:Owner", "owner"),
            ("Osirion:Cms:GitHub:Repository", "repo"));

        builder.AddGitHub().ShouldBeSameAs(builder);

        using var provider = _services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<GitHubOptions>>().Value.Owner.ShouldBe("owner");
        var setter = SingleSetter(provider);
        setter.ProviderId.ShouldBe("github-owner-repo");
        setter.IsDefault.ShouldBeFalse();
        _services.ShouldContain(d => d.ServiceType == typeof(GitHubContentProvider));
        _services.ShouldContain(d => d.ServiceType == typeof(GitHubContentRepository));
        _services.ShouldContain(d => d.ServiceType == typeof(GitHubDirectoryRepository));
        _services.ShouldContain(d => d.ServiceType == typeof(IContentProvider));
    }

    [Fact]
    public void AddGitHub_WithDelegate_AppliesItAfterTheGitHubSection()
    {
        var builder = CreateBuilder(
            ("Osirion:Cms:GitHub:Owner", "owner"),
            ("Osirion:Cms:GitHub:Repository", "repo"));

        builder.AddGitHub(options =>
        {
            options.ProviderId = "custom-id";
            options.IsDefault = true;
        });

        using var provider = _services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<GitHubOptions>>().Value;
        options.Owner.ShouldBe("owner");
        var setter = SingleSetter(provider);
        setter.ProviderId.ShouldBe("custom-id");
        setter.IsDefault.ShouldBeTrue();
    }

    [Fact]
    public void AddFileSystem_WithoutDelegate_BindsTheFileSystemSection()
    {
        var builder = CreateBuilder(
            ("Osirion:Cms:Web:FileSystem:BasePath", "content"),
            ("Osirion:Cms:Web:FileSystem:ProviderId", "files"));

        builder.AddFileSystem().ShouldBeSameAs(builder);

        using var provider = _services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<FileSystemOptions>>().Value.BasePath.ShouldBe("content");
        SingleSetter(provider).ProviderId.ShouldBe("files");
        _services.ShouldContain(d => d.ServiceType == typeof(FileSystemContentProvider));
        _services.ShouldContain(d => d.ServiceType == typeof(FileSystemContentRepository));
        _services.ShouldContain(d => d.ServiceType == typeof(FileSystemDirectoryRepository));
    }

    [Fact]
    public void AddFileSystem_WithDelegate_AppliesItAfterTheFileSystemSection()
    {
        var builder = CreateBuilder(("Osirion:Cms:FileSystem:Web:BasePath", "legacy-content"));

        builder.AddFileSystem(options =>
        {
            options.ProviderId = "docs";
            options.IsDefault = true;
        });

        using var provider = _services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<FileSystemOptions>>().Value.BasePath.ShouldBe("legacy-content");
        var setter = SingleSetter(provider);
        setter.ProviderId.ShouldBe("docs");
        setter.IsDefault.ShouldBeTrue();
    }

    [Fact]
    public void AddProvider_WithDelegate_ConfiguresTheResolvedProvider()
    {
        var builder = CreateBuilder();

        builder.AddProvider<NamedContentProvider>(p => p.ConfiguredBy = "delegate").ShouldBeSameAs(builder);

        using var provider = _services.BuildServiceProvider();
        var contentProvider = provider.GetServices<IContentProvider>().Single().ShouldBeOfType<NamedContentProvider>();
        contentProvider.ConfiguredBy.ShouldBe("delegate");
        contentProvider.ShouldBeSameAs(provider.GetRequiredService<NamedContentProvider>());
    }

    [Fact]
    public void AddProvider_WithoutDelegate_RegistersTheProviderAsASingleton()
    {
        var builder = CreateBuilder();

        builder.AddProvider<NamedContentProvider>();

        using var provider = _services.BuildServiceProvider();
        var contentProvider = provider.GetServices<IContentProvider>().Single().ShouldBeOfType<NamedContentProvider>();
        contentProvider.ConfiguredBy.ShouldBeNull();
        _services.Single(d => d.ServiceType == typeof(NamedContentProvider)).Lifetime.ShouldBe(ServiceLifetime.Singleton);
    }
}
