using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Admin.Builders;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Providers;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.DependencyInjection;

public class OsirionCmsAdminBuilderTests
{
    private readonly ServiceCollection _services = new();
    private readonly RecordingLogger<OsirionCmsAdminBuilder> _logger = new();
    private readonly OsirionCmsAdminBuilder _builder;

    public OsirionCmsAdminBuilderTests()
    {
        _builder = new OsirionCmsAdminBuilder(_services, new ConfigurationBuilder().Build(), _logger);
    }

    private CmsAdminOptions ResolveOptions() =>
        _services.BuildServiceProvider().GetRequiredService<IOptions<CmsAdminOptions>>().Value;

    [Fact]
    public void Constructor_NullArguments_Throw()
    {
        var configuration = new ConfigurationBuilder().Build();
        var logger = NullLogger<OsirionCmsAdminBuilder>.Instance;

        Should.Throw<ArgumentNullException>(() => new OsirionCmsAdminBuilder(null!, configuration, logger)).ParamName.ShouldBe("services");
        Should.Throw<ArgumentNullException>(() => new OsirionCmsAdminBuilder(_services, null!, logger)).ParamName.ShouldBe("configuration");
        Should.Throw<ArgumentNullException>(() => new OsirionCmsAdminBuilder(_services, configuration, null!)).ParamName.ShouldBe("logger");
    }

    [Fact]
    public void Services_IsTheCollectionPassedIn()
    {
        _builder.Services.ShouldBeSameAs(_services);
    }

    [Fact]
    public void UseGitHubProvider_WithoutConfigure_RegistersTheGitHubAdminService()
    {
        var result = _builder.UseGitHubProvider();

        result.ShouldBeSameAs(_builder);
        _services.ShouldContain(descriptor => descriptor.ServiceType == typeof(IGitHubAdminService));
        _logger.HasEntry(LogLevel.Information, "GitHub provider").ShouldBeTrue();
    }

    [Fact]
    public void UseGitHubProvider_WithConfigure_MakesGitHubTheDefaultAndAppliesTheOptions()
    {
        _builder.UseGitHubProvider(options => options.Owner = "octo");

        var options = ResolveOptions();
        options.DefaultContentProvider.ShouldBe("GitHub");
        options.GitHub.Owner.ShouldBe("octo");
    }

    [Fact]
    public void UseFileSystemProvider_WithoutConfigure_KeepsTheDefaultProvider()
    {
        var result = _builder.UseFileSystemProvider();

        result.ShouldBeSameAs(_builder);
        ResolveOptions().DefaultContentProvider.ShouldBe("GitHub");
    }

    [Fact]
    public void UseFileSystemProvider_WithConfigure_MakesFileSystemTheDefaultAndAppliesTheOptions()
    {
        _builder.UseFileSystemProvider(options => options.RootPath = "/content");

        var options = ResolveOptions();
        options.DefaultContentProvider.ShouldBe("FileSystem");
        options.FileSystem.RootPath.ShouldBe("/content");
    }

    [Fact]
    public void ConfigureAuthentication_AppliesTheOptions()
    {
        _builder.ConfigureAuthentication(options => options.EnableGitHubOAuth = true).ShouldBeSameAs(_builder);

        ResolveOptions().Authentication.EnableGitHubOAuth.ShouldBeTrue();
    }

    [Fact]
    public void ConfigureTheme_AppliesTheOptions()
    {
        _builder.ConfigureTheme(options => options.PrimaryColor = "#ff0000").ShouldBeSameAs(_builder);

        ResolveOptions().Theme.PrimaryColor.ShouldBe("#ff0000");
    }

    [Fact]
    public void ConfigureContentRules_AppliesTheOptions()
    {
        _builder.ConfigureContentRules(options => options.RequireApproval = true).ShouldBeSameAs(_builder);

        ResolveOptions().ContentRules.RequireApproval.ShouldBeTrue();
    }

    [Fact]
    public void UseLocalization_AppliesTheOptions()
    {
        _builder.UseLocalization(options => options.DefaultCulture = "sr-RS").ShouldBeSameAs(_builder);

        ResolveOptions().Localization.DefaultCulture.ShouldBe("sr-RS");
    }

    [Fact]
    public void ConfigureMethods_NullAction_Throw()
    {
        Should.Throw<ArgumentNullException>(() => _builder.ConfigureAuthentication(null!));
        Should.Throw<ArgumentNullException>(() => _builder.ConfigureTheme(null!));
        Should.Throw<ArgumentNullException>(() => _builder.ConfigureContentRules(null!));
        Should.Throw<ArgumentNullException>(() => _builder.UseLocalization(null!));
    }

    [Fact]
    public void AddProvider_RegistersTheProviderAsScopedContentProvider()
    {
        _builder.AddProvider<FileSystemContentProvider>().ShouldBeSameAs(_builder);

        _services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IContentProvider)
            && descriptor.ImplementationType == typeof(FileSystemContentProvider)
            && descriptor.Lifetime == ServiceLifetime.Scoped);
    }
}
