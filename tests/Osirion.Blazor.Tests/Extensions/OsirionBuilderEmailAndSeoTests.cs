using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Web.Options;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Extensions;
using Shouldly;

namespace Osirion.Blazor.Tests.Extensions;

public class OsirionBuilderEmailAndSeoTests
{
    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values) =>
        new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    [Fact]
    public void UseEmailServices_WithAction_RegistersTheEmailServiceAndOptions()
    {
        var services = new ServiceCollection();

        services.AddOsirion(osirion => osirion.UseEmailServices(options => options.ToEmail = "team@example.com"));

        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(IEmailService));
        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<EmailOptions>>().Value.ToEmail.ShouldBe("team@example.com");
    }

    [Fact]
    public void UseEmailServices_WithConfiguration_BindsTheEmailSection()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(new() { [$"{EmailOptions.Section}:ToEmail"] = "config@example.com" });

        services.AddOsirion(osirion => osirion.UseEmailServices(configuration));

        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(IEmailService));
        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<EmailOptions>>().Value.ToEmail.ShouldBe("config@example.com");
    }

    [Fact]
    public void UseSeoMetadata_WithAction_ConfiguresTheOptions()
    {
        var services = new ServiceCollection();

        services.AddOsirion(osirion => osirion.UseSeoMetadata(options => options.SiteName = "Osirion"));

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<SeoMetadataOptions>>().Value.SiteName.ShouldBe("Osirion");
    }

    [Fact]
    public void UseSeoMetadata_WithConfiguration_BindsTheGivenSection()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(new() { ["SiteName"] = "From config", ["AllowAiTraining"] = "false" });

        services.AddOsirion(osirion => osirion.UseSeoMetadata(configuration));

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SeoMetadataOptions>>().Value;
        options.SiteName.ShouldBe("From config");
        options.AllowAiTraining.ShouldBeFalse();
    }

    [Fact]
    public void AddOsirion_WithEmailAndSeoSections_ConfiguresBoth()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(new()
        {
            [$"{EmailOptions.Section}:ToEmail"] = "section@example.com",
            ["Osirion:Seo:SiteName"] = "Section site",
        });

        services.AddOsirion(configuration);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<EmailOptions>>().Value.ToEmail.ShouldBe("section@example.com");
        provider.GetRequiredService<IOptions<SeoMetadataOptions>>().Value.SiteName.ShouldBe("Section site");
    }

    [Fact]
    public void UseCmsAdmin_WithHostConfigurationRegistered_AppliesTheActionAndKeepsTheHostConfiguration()
    {
        var services = new ServiceCollection();
        var hostConfiguration = BuildConfiguration(new() { ["Osirion:Cms:Admin:Owner"] = "ignored" });
        services.AddSingleton(hostConfiguration);

        services.AddOsirion(osirion => osirion.UseCmsAdmin(admin => admin.Configure(options => options.Owner = "owner")));

        services.Where(descriptor => descriptor.ServiceType == typeof(IConfiguration))
            .ShouldHaveSingleItem().ImplementationInstance.ShouldBeSameAs(hostConfiguration);
        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<GitHubOptions>>().Value.Owner.ShouldBe("owner");
    }

    [Fact]
    public void UseCmsAdmin_WithHostConfigurationFactory_StillAppliesTheAction()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(_ => BuildConfiguration([]));

        services.AddOsirion(osirion => osirion.UseCmsAdmin(admin => admin.Configure(options => options.Repository = "repo")));

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<GitHubOptions>>().Value.Repository.ShouldBe("repo");
    }

    [Fact]
    public void UseCmsAdmin_NullAction_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();

        var exception = Should.Throw<ArgumentNullException>(() =>
            services.AddOsirion(osirion => osirion.UseCmsAdmin((Action<Osirion.Blazor.Cms.Admin.Interfaces.ICmsAdminBuilder>)null!)));

        exception.ParamName.ShouldBe("configure");
    }
}
