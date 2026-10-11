using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.DependencyInjection;

public class ApplicationBuilderExtensionsTests
{
    private static (IApplicationBuilder App, IContentProviderInitializer Initializer) CreateApp()
    {
        var initializer = Substitute.For<IContentProviderInitializer>();
        var services = new ServiceCollection();
        services.AddSingleton(initializer);
        services.AddSingleton(typeof(Microsoft.Extensions.Logging.ILogger<>), typeof(NullLogger<>));
        return (new ApplicationBuilder(services.BuildServiceProvider()), initializer);
    }

    [Fact]
    public async Task UseOsirionCmsAsync_AwaitsProviderInitialization()
    {
        var (app, initializer) = CreateApp();
        var release = new TaskCompletionSource();
        initializer.InitializeProvidersAsync().Returns(release.Task);

        var pending = app.UseOsirionCmsAsync();
        await Task.Delay(30);
        pending.IsCompleted.ShouldBeFalse();
        release.SetResult();

        (await pending).ShouldBeSameAs(app);
    }

    [Fact]
    public async Task UseOsirionCmsAsync_WhenInitializationFails_DoesNotCrashStartup()
    {
        var (app, initializer) = CreateApp();
        initializer.InitializeProvidersAsync().Returns(Task.FromException(new InvalidOperationException("boom")));

        (await app.UseOsirionCmsAsync()).ShouldBeSameAs(app);
    }
}
