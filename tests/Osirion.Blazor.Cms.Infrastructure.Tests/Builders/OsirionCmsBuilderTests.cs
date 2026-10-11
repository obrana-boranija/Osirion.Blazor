using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Providers;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Builders;

public class OsirionCmsBuilderTests
{
    /// <summary>A provider type the container can hand out without constructing a real provider.</summary>
    public interface ITestContentProvider : IContentProvider;

    private static IConfiguration Configuration(params (string Key, string Value)[] values) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(values.ToDictionary(pair => pair.Key, pair => (string?)pair.Value))
            .Build();

    private static (OsirionCmsBuilder Builder, ServiceCollection Services) CreateBuilder(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        return (new OsirionCmsBuilder(services, configuration), services);
    }

    private static bool HasKeyedProvider(IServiceCollection services, string providerName) =>
        services.Any(d => d.ServiceType == typeof(GitHubContentProvider) && Equals(d.ServiceKey, $"GitHubContentProvider_{providerName}"));

    private static bool HasLegacyGitHubProvider(IServiceCollection services) =>
        services.Any(d => d.ServiceType == typeof(GitHubContentProvider) && !d.IsKeyedService);

    [Fact]
    public void Constructor_WithNullArguments_Throws()
    {
        Should.Throw<ArgumentNullException>(() => new OsirionCmsBuilder(null!, Configuration()));
        Should.Throw<ArgumentNullException>(() => new OsirionCmsBuilder(new ServiceCollection(), null!));
    }

    [Fact]
    public void Constructor_ExposesServicesAndConfiguration()
    {
        var configuration = Configuration();
        var services = new ServiceCollection();

        var builder = new OsirionCmsBuilder(services, configuration);

        builder.Services.ShouldBeSameAs(services);
        builder.Configuration.ShouldBeSameAs(configuration);
    }

    [Fact]
    public void AddGitHub_WithProviderName_BindsTheWebSectionThenAppliesTheDelegate()
    {
        var (builder, services) = CreateBuilder(Configuration(
            ("Osirion:Cms:Web:GitHub:blog:Owner", "web-owner"),
            ("Osirion:Cms:Web:GitHub:blog:Repository", "web-repo")));

        builder.AddGitHub("blog", options => options.Repository = "override").ShouldBeSameAs(builder);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<GitHubOptions>>().Get("GitHub_blog");
        options.Owner.ShouldBe("web-owner");
        options.Repository.ShouldBe("override");
    }

    [Fact]
    public void AddGitHub_WithProviderNameOnlyInAdminSection_BindsTheAdminSection()
    {
        var (builder, services) = CreateBuilder(Configuration(("Osirion:Cms:Admin:GitHub:docs:Owner", "admin-owner")));

        builder.AddGitHub("docs");

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptionsMonitor<GitHubOptions>>().Get("GitHub_docs").Owner.ShouldBe("admin-owner");
    }

    [Fact]
    public void AddGitHub_WithUnknownProviderName_LeavesDefaultOptions()
    {
        var (builder, services) = CreateBuilder(Configuration(("Osirion:Cms:GitHub:Owner", "legacy-owner")));

        builder.AddGitHub("unknown");

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptionsMonitor<GitHubOptions>>().Get("GitHub_unknown").Owner.ShouldBe(new GitHubOptions().Owner);
    }

    [Fact]
    public void AddGitHub_WithoutProviderNameAndWithDelegate_BindsTheLegacySectionThenAppliesTheDelegate()
    {
        var (builder, services) = CreateBuilder(Configuration(
            ("Osirion:Cms:GitHub:Owner", "legacy-owner"),
            ("Osirion:Cms:GitHub:Repository", "legacy-repo")));

        builder.AddGitHub(null, options => options.Repository = "custom-repo");

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<GitHubOptions>>().Value;
        options.Owner.ShouldBe("legacy-owner");
        options.Repository.ShouldBe("custom-repo");
        var setter = provider.GetServices<IDefaultProviderSetter>().OfType<DefaultProviderSetter>().Single();
        setter.ProviderId.ShouldBe("github-legacy-owner-custom-repo");
    }

    [Fact]
    public void AddGitHub_WithConfiguredProviderId_UsesItForTheDefaultSetter()
    {
        var (builder, services) = CreateBuilder(Configuration(
            ("Osirion:Cms:GitHub:ProviderId", "configured-id"),
            ("Osirion:Cms:GitHub:IsDefault", "true")));

        builder.AddGitHub();

        using var provider = services.BuildServiceProvider();
        var setter = provider.GetServices<IDefaultProviderSetter>().OfType<DefaultProviderSetter>().Single();
        setter.ProviderId.ShouldBe("configured-id");
        setter.IsDefault.ShouldBeTrue();
        HasLegacyGitHubProvider(services).ShouldBeTrue();
    }

    [Fact]
    public void AddGitHubProviders_RegistersEveryConfiguredWebProvider()
    {
        var (builder, services) = CreateBuilder(Configuration(
            ("Osirion:Cms:Web:GitHub:blog:Owner", "o"),
            ("Osirion:Cms:Web:GitHub:docs:Owner", "o")));

        builder.AddGitHubProviders().ShouldBeSameAs(builder);

        HasKeyedProvider(services, "blog").ShouldBeTrue();
        HasKeyedProvider(services, "docs").ShouldBeTrue();
    }

    [Fact]
    public void AddFileSystem_WithDelegate_AppliesItAfterConfiguration()
    {
        var (builder, services) = CreateBuilder(Configuration(
            ("Osirion:Cms:Web:FileSystem:BasePath", "configured"),
            ("Osirion:Cms:Web:FileSystem:IsDefault", "true")));

        builder.AddFileSystem(options => options.ProviderId = "files").ShouldBeSameAs(builder);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<FileSystemOptions>>().Value;
        options.BasePath.ShouldBe("configured");
        options.ProviderId.ShouldBe("files");
        var setter = provider.GetServices<IDefaultProviderSetter>().OfType<DefaultProviderSetter>().Single();
        setter.ProviderId.ShouldBe("files");
        setter.IsDefault.ShouldBeTrue();
        services.ShouldContain(d => d.ServiceType == typeof(FileSystemContentProvider));
    }

    [Fact]
    public void AddFileSystem_WithExistingSection_RegistersTheProvider()
    {
        var (builder, services) = CreateBuilder(Configuration(("Osirion:Cms:FileSystem:Web:BasePath", "content")));

        builder.AddFileSystem(FileSystemOptions.Section).ShouldBeSameAs(builder);

        services.ShouldContain(d => d.ServiceType == typeof(FileSystemContentProvider));
        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<FileSystemOptions>>().Value.BasePath.ShouldBe("content");
    }

    [Fact]
    public void AddFileSystem_WithMissingSection_RegistersNothing()
    {
        var (builder, services) = CreateBuilder(Configuration());

        builder.AddFileSystem("Osirion:Cms:Missing");

        services.ShouldNotContain(d => d.ServiceType == typeof(FileSystemContentProvider));
        services.ShouldNotContain(d => d.ServiceType == typeof(IContentProvider));
    }

    [Fact]
    public void AddProvider_RegistersTheProviderAndAppliesTheDelegate()
    {
        var (builder, services) = CreateBuilder(Configuration());
        var custom = Substitute.For<ITestContentProvider>();
        custom.ProviderId.Returns("custom");
        services.AddSingleton(custom);
        ITestContentProvider? configured = null;

        builder.AddProvider<ITestContentProvider>(p => configured = p, isDefault: true).ShouldBeSameAs(builder);

        using var provider = services.BuildServiceProvider();
        provider.GetServices<IContentProvider>().ShouldContain(custom);
        configured.ShouldBeSameAs(custom);
        var setter = provider.GetServices<IDefaultProviderSetter>().OfType<DefaultProviderSetter>().Single();
        setter.ProviderId.ShouldBe("custom");
        setter.IsDefault.ShouldBeTrue();
    }

    [Fact]
    public void AddProvider_WhenNotDefault_RegistersNoDefaultSetter()
    {
        var (builder, services) = CreateBuilder(Configuration());
        services.AddSingleton(Substitute.For<ITestContentProvider>());

        builder.AddProvider<ITestContentProvider>();

        services.ShouldNotContain(d => d.ServiceType == typeof(IDefaultProviderSetter));
        using var provider = services.BuildServiceProvider();
        provider.GetServices<IContentProvider>().Count().ShouldBe(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void SetDefaultProvider_WithBlankId_Throws(string providerId)
    {
        var (builder, _) = CreateBuilder(Configuration());

        Should.Throw<ArgumentException>(() => builder.SetDefaultProvider(providerId));
    }

    [Fact]
    public void SetDefaultProvider_RegistersADefaultSetter()
    {
        var (builder, services) = CreateBuilder(Configuration());

        builder.SetDefaultProvider("chosen").ShouldBeSameAs(builder);

        using var provider = services.BuildServiceProvider();
        var setter = provider.GetRequiredService<IDefaultProviderSetter>().ShouldBeOfType<DefaultProviderSetter>();
        setter.ProviderId.ShouldBe("chosen");
        setter.IsDefault.ShouldBeTrue();
    }

    [Fact]
    public void ConfigureCache_WithNullDelegate_Throws()
    {
        var (builder, _) = CreateBuilder(Configuration());

        Should.Throw<ArgumentNullException>(() => builder.ConfigureCache(null!));
    }

    [Fact]
    public void ConfigureCache_AppliesTheDelegate()
    {
        var (builder, services) = CreateBuilder(Configuration());

        builder.ConfigureCache(options => options.MaxAgeMinutes = 7).ShouldBeSameAs(builder);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<CacheOptions>>().Value.MaxAgeMinutes.ShouldBe(7);
    }

    [Fact]
    public void AddAllProvidersFromConfiguration_WithWebProviders_UsesMultiProviderRegistration()
    {
        var (builder, services) = CreateBuilder(Configuration(
            ("Osirion:Cms:Web:GitHub:blog:Owner", "o"),
            ("Osirion:Cms:GitHub:Owner", "legacy")));

        builder.AddAllProvidersFromConfiguration().ShouldBeSameAs(builder);

        HasKeyedProvider(services, "blog").ShouldBeTrue();
        HasLegacyGitHubProvider(services).ShouldBeFalse();
    }

    [Fact]
    public void AddAllProvidersFromConfiguration_WithAdminProvidersOnly_UsesMultiProviderRegistration()
    {
        var (builder, services) = CreateBuilder(Configuration(("Osirion:Cms:Admin:GitHub:docs:Owner", "o")));

        builder.AddAllProvidersFromConfiguration();

        HasKeyedProvider(services, "docs").ShouldBeTrue();
        HasLegacyGitHubProvider(services).ShouldBeFalse();
    }

    [Fact]
    public void AddAllProvidersFromConfiguration_WithLegacySectionOnly_RegistersTheLegacyProvider()
    {
        var (builder, services) = CreateBuilder(Configuration(("Osirion:Cms:GitHub:Owner", "legacy")));

        builder.AddAllProvidersFromConfiguration();

        HasLegacyGitHubProvider(services).ShouldBeTrue();
        services.ShouldNotContain(d => d.IsKeyedService && d.ServiceType == typeof(GitHubContentProvider));
    }

    [Fact]
    public void AddAllProvidersFromConfiguration_WithoutGitHubConfiguration_RegistersNoGitHubProvider()
    {
        var (builder, services) = CreateBuilder(Configuration(("Osirion:Cms:FileSystem:Web:BasePath", "content")));

        builder.AddAllProvidersFromConfiguration();

        services.ShouldNotContain(d => d.ServiceType == typeof(GitHubContentProvider));
        services.ShouldNotContain(d => d.ServiceType == typeof(FileSystemContentProvider));
        services.ShouldNotContain(d => d.ServiceType == typeof(FileSystemContentRepository));
    }
}
