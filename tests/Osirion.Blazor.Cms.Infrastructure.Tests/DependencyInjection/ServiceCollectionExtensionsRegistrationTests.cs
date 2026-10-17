using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Providers;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;
using CmsServiceCollectionExtensions = Osirion.Blazor.Cms.Infrastructure.DependencyInjection.ServiceCollectionExtensions;

#pragma warning disable CS0618 // The options under test are obsolete and keep working until they are removed.

namespace Osirion.Blazor.Cms.Infrastructure.Tests.DependencyInjection;

public class ServiceCollectionExtensionsRegistrationTests
{
    private readonly ServiceCollection _services = new();

    public ServiceCollectionExtensionsRegistrationTests()
    {
        _services.AddLogging();
    }

    private static IConfiguration Configuration(params (string Key, string Value)[] values) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(values.ToDictionary(pair => pair.Key, pair => (string?)pair.Value))
            .Build();

    private static DefaultProviderSetter SingleSetter(IServiceProvider provider) =>
        provider.GetServices<IDefaultProviderSetter>().OfType<DefaultProviderSetter>().Single();

    [Fact]
    public void AddCms_WithNullArguments_Throws()
    {
        Should.Throw<ArgumentNullException>(() => CmsServiceCollectionExtensions.AddCms(null!, Configuration()));
        Should.Throw<ArgumentNullException>(() => _services.AddCms(null!));
    }

    [Fact]
    public void AddCms_WithBuilderDelegate_PassesABuilderOverTheSameServices()
    {
        var configuration = Configuration(("Osirion:Cms:GitHub:Owner", "ignored"));
        OsirionCmsBuilder? received = null;

        _services.AddCms(configuration, builder => received = builder).ShouldBeSameAs(_services);

        received.ShouldNotBeNull();
        received.Services.ShouldBeSameAs(_services);
        received.Configuration.ShouldBeSameAs(configuration);
        _services.ShouldNotContain(d => d.ServiceType == typeof(GitHubContentProvider));
    }

    [Fact]
    public void AddCms_WithoutBuilderDelegate_RegistersProvidersFromConfiguration()
    {
        _services.AddCms(Configuration(("Osirion:Cms:GitHub:Owner", "owner")));

        _services.ShouldContain(d => d.ServiceType == typeof(GitHubContentProvider));
        _services.ShouldNotContain(d => d.ServiceType == typeof(FileSystemContentProvider));
    }

    [Fact]
    public void AddCms_BindsCacheOptions()
    {
        _services.AddCms(Configuration(("Osirion:Cms:Cache:MaxAgeMinutes", "9")), _ => { });

        using var provider = _services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<CacheOptions>>().Value.MaxAgeMinutes.ShouldBe(9);
    }

    [Fact]
    public void AddGitHubContentProvider_WithNullArguments_Throws()
    {
        Should.Throw<ArgumentNullException>(() => CmsServiceCollectionExtensions.AddGitHubContentProvider(null!, Configuration()));
        Should.Throw<ArgumentNullException>(() => _services.AddGitHubContentProvider(null!));
    }

    [Fact]
    public void AddGitHubContentProvider_WithNamedProviderInWebSection_BindsItAndAppliesTheDelegate()
    {
        var configuration = Configuration(
            ("Osirion:Cms:Web:GitHub:blog:Owner", "web-owner"),
            ("Osirion:Cms:Admin:GitHub:blog:Owner", "admin-owner"));

        _services.AddGitHubContentProvider(configuration, "blog", options => options.Branch = "dev");

        using var provider = _services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<GitHubOptions>>().Get("GitHub_blog");
        options.Owner.ShouldBe("web-owner");
        options.Branch.ShouldBe("dev");
    }

    [Fact]
    public void AddGitHubContentProvider_WithLegacyDelegate_AppliesItAfterTheSection()
    {
        var configuration = Configuration(
            ("Osirion:Cms:GitHub:Owner", "owner"),
            ("Osirion:Cms:GitHub:Repository", "repo"));

        _services.AddGitHubContentProvider(configuration, configureOptions: options => options.IsDefault = true);

        using var provider = _services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<GitHubOptions>>().Value.Owner.ShouldBe("owner");
        var setter = SingleSetter(provider);
        setter.ProviderId.ShouldBe("github-owner-repo");
        setter.IsDefault.ShouldBeTrue();
    }

    [Fact]
    public void AddGitHubContentProvider_CalledTwice_KeepsOneProviderRegistration()
    {
        var configuration = Configuration(("Osirion:Cms:GitHub:Owner", "owner"));

        _services.AddGitHubContentProvider(configuration);
        _services.AddGitHubContentProvider(configuration);

        _services.Count(d => d.ServiceType == typeof(IContentProvider)).ShouldBe(1);
        _services.Count(d => d.ServiceType == typeof(IDefaultProviderSetter)).ShouldBe(1);
        _services.Single(d => d.ServiceType == typeof(IAuthenticationService)).Lifetime.ShouldBe(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddFileSystemContentProvider_WithDelegate_AppliesItAfterTheSection()
    {
        var configuration = Configuration(("Osirion:Cms:Web:FileSystem:BasePath", "content"));

        _services.AddFileSystemContentProvider(configuration, options =>
        {
            options.ProviderId = "docs";
            options.IsDefault = true;
        }).ShouldBeSameAs(_services);

        using var provider = _services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<FileSystemOptions>>().Value.BasePath.ShouldBe("content");
        var setter = SingleSetter(provider);
        setter.ProviderId.ShouldBe("docs");
        setter.IsDefault.ShouldBeTrue();
        provider.GetRequiredService<System.IO.Abstractions.IFileSystem>().ShouldBeOfType<System.IO.Abstractions.FileSystem>();
    }

    [Fact]
    public void AddFileSystemContentProvider_KeepsAHostFileSystem()
    {
        var hostFileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem();
        _services.AddSingleton<System.IO.Abstractions.IFileSystem>(hostFileSystem);

        _services.AddFileSystemContentProvider(Configuration());

        using var provider = _services.BuildServiceProvider();
        provider.GetRequiredService<System.IO.Abstractions.IFileSystem>().ShouldBeSameAs(hostFileSystem);
    }

    [Fact]
    public void AddCustomContentProvider_WithNullServices_Throws()
    {
        Should.Throw<ArgumentNullException>(() => CmsServiceCollectionExtensions.AddCustomContentProvider<NamedContentProvider>(null!));
    }

    [Fact]
    public void AddCustomContentProvider_AsDefault_ConfiguresTheProviderAndRegistersADefaultSetter()
    {
        _services.AddCustomContentProvider<NamedContentProvider>(p => p.ConfiguredBy = "custom", isDefault: true)
            .ShouldBeSameAs(_services);

        using var provider = _services.BuildServiceProvider();
        provider.GetServices<IContentProvider>().Single().ShouldBeOfType<NamedContentProvider>().ConfiguredBy.ShouldBe("custom");
        var setter = SingleSetter(provider);
        setter.ProviderId.ShouldBe(NamedContentProvider.Id);
        setter.IsDefault.ShouldBeTrue();
    }

    [Fact]
    public void AddCustomContentProvider_NotDefault_RegistersNoDefaultSetter()
    {
        _services.AddCustomContentProvider<NamedContentProvider>();

        using var provider = _services.BuildServiceProvider();
        provider.GetServices<IContentProvider>().Single().ShouldBeOfType<NamedContentProvider>().ConfiguredBy.ShouldBeNull();
        _services.ShouldNotContain(d => d.ServiceType == typeof(IDefaultProviderSetter));
    }

    [Fact]
    public void AddOsirionCmsWithProviders_WithNullArguments_Throws()
    {
        Should.Throw<ArgumentNullException>(() => CmsServiceCollectionExtensions.AddOsirionCmsWithProviders(null!, Configuration()));
        Should.Throw<ArgumentNullException>(() => _services.AddOsirionCmsWithProviders(null!));
    }

    [Fact]
    public void AddOsirionCmsWithProviders_WithoutProviderSections_RegistersCoreServicesOnly()
    {
        _services.AddOsirionCmsWithProviders(Configuration()).ShouldBeSameAs(_services);

        _services.ShouldContain(d => d.ServiceType == typeof(IContentProviderManager));
        _services.ShouldNotContain(d => d.ServiceType == typeof(IContentProvider));
    }
}
