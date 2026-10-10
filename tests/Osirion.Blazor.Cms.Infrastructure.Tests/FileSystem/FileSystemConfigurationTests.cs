using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.FileSystem;

/// <summary>
/// AddOsirion(configuration) enables the FileSystem provider when Osirion:Cms:Web:FileSystem exists, but the options
/// were bound only from Osirion:Cms:FileSystem:Web, so settings under the key that enabled the provider were ignored.
/// </summary>
public class FileSystemConfigurationTests
{
    [Fact]
    public void AddFileSystemContentProvider_SettingsUnderWebSection_AreBound()
    {
        var options = Resolve(new Dictionary<string, string?> { ["Osirion:Cms:Web:FileSystem:BasePath"] = "content" });

        options.BasePath.ShouldBe("content");
    }

    [Fact]
    public void AddFileSystemContentProvider_SettingsUnderLegacySection_AreStillBound()
    {
        var options = Resolve(new Dictionary<string, string?> { ["Osirion:Cms:FileSystem:Web:BasePath"] = "legacy" });

        options.BasePath.ShouldBe("legacy");
    }

    [Fact]
    public void AddFileSystemContentProvider_BothSections_WebSectionWins()
    {
        var options = Resolve(new Dictionary<string, string?>
        {
            ["Osirion:Cms:Web:FileSystem:BasePath"] = "content",
            ["Osirion:Cms:FileSystem:Web:BasePath"] = "legacy"
        });

        options.BasePath.ShouldBe("content");
    }

    [Fact]
    public void AddCmsWithFileSystemProvider_ProviderGraph_PassesScopeValidation()
    {
        // The FileSystem provider and repositories were scoped while the provider registry that holds every
        // IContentProvider is a singleton, so a Development host failed at startup with a captive dependency.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Osirion:Cms:Web:FileSystem:BasePath"] = "content" })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddCms(configuration);
        services.AddFileSystemContentProvider(configuration);

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        Should.NotThrow(() => provider.GetRequiredService<Osirion.Blazor.Cms.Domain.Services.IContentProviderRegistry>());
    }

    private static FileSystemOptions Resolve(Dictionary<string, string?> settings)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddFileSystemContentProvider(configuration);

        using var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IOptions<FileSystemOptions>>().Value;
    }
}
