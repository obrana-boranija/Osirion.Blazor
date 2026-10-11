using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Shouldly;
using CmsApplicationBuilderExtensions = Osirion.Blazor.Cms.Infrastructure.DependencyInjection.ApplicationBuilderExtensions;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.DependencyInjection;

public class ApplicationBuilderSyncInitializationTests
{
    private readonly IContentProviderInitializer _initializer = Substitute.For<IContentProviderInitializer>();
    private readonly ILogger<IContentProviderInitializer> _logger = Substitute.For<ILogger<IContentProviderInitializer>>();

    private IApplicationBuilder CreateApp()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_initializer);
        services.AddSingleton(_logger);
        return new ApplicationBuilder(services.BuildServiceProvider());
    }

    [Fact]
    public void UseOsirionCms_InitializesProviders()
    {
        var app = CreateApp();
        _initializer.InitializeProvidersAsync().Returns(Task.CompletedTask);

        app.UseOsirionCms().ShouldBeSameAs(app);

        _initializer.Received(1).InitializeProvidersAsync();
        _logger.ReceivedCalls().ShouldNotContain(call => call.GetMethodInfo().Name == nameof(ILogger.Log));
    }

    [Fact]
    public void UseOsirionCms_WhenInitializationFails_LogsTheErrorAndContinues()
    {
        var app = CreateApp();
        var failure = new InvalidOperationException("boom");
        _initializer.InitializeProvidersAsync().Returns(Task.FromException(failure));

        app.UseOsirionCms().ShouldBeSameAs(app);

        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<Arg.AnyType>(),
            failure,
            Arg.Any<Func<Arg.AnyType, Exception?, string>>());
    }

    [Fact]
    public async Task UseOsirionCmsAsync_WithNullApp_Throws()
    {
        await Should.ThrowAsync<ArgumentNullException>(() => CmsApplicationBuilderExtensions.UseOsirionCmsAsync(null!));
    }

    [Fact]
    public async Task UseOsirionCmsAsync_WhenCancelled_ThrowsInsteadOfLogging()
    {
        var app = CreateApp();
        _initializer.InitializeProvidersAsync().Returns(new TaskCompletionSource().Task);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(() => app.UseOsirionCmsAsync(cts.Token));

        _logger.ReceivedCalls().ShouldNotContain(call => call.GetMethodInfo().Name == nameof(ILogger.Log));
    }

    [Fact]
    public async Task UseOsirionCmsAsync_WhenInitializationFails_LogsTheError()
    {
        var app = CreateApp();
        var failure = new InvalidOperationException("boom");
        _initializer.InitializeProvidersAsync().Returns(Task.FromException(failure));

        (await app.UseOsirionCmsAsync()).ShouldBeSameAs(app);

        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<Arg.AnyType>(),
            failure,
            Arg.Any<Func<Arg.AnyType, Exception?, string>>());
    }
}
