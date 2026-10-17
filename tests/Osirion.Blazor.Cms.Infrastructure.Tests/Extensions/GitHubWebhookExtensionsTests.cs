using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Microsoft.Extensions.Logging.Abstractions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Extensions;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Extensions;

public class GitHubWebhookExtensionsTests
{
    [Fact]
    public void AddGitHubWebhookAndPolling_RegistersScopedHandlerAndPollingService()
    {
        var services = new ServiceCollection();

        var returned = services.AddGitHubWebhookAndPolling();

        returned.ShouldBeSameAs(services);
        var handler = services.Where(d => d.ServiceType == typeof(IGitHubWebhookHandler)).ShouldHaveSingleItem();
        handler.ImplementationType.ShouldBe(typeof(GitHubWebhookHandler));
        handler.Lifetime.ShouldBe(ServiceLifetime.Scoped);
        services.ShouldContain(d => d.ServiceType == typeof(IHostedService) && d.ImplementationType == typeof(GitHubPollingService));
    }

    [Fact]
    public async Task UseGitHubWebhook_SendsWebhookPostsToTheHandler()
    {
        var webhookHandler = Substitute.For<IGitHubWebhookHandler>();
        webhookHandler.HandleWebhookAsync(Arg.Any<HttpRequest>()).Returns(true);
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton(webhookHandler)
            .AddSingleton<IGitHubApiClientFactory>(GitHubApiClientFactoryWith(new Dictionary<string, string?>
            {
                ["Osirion:Cms:Web:GitHub:Blog:Owner"] = "o",
                ["Osirion:Cms:Web:GitHub:Blog:Repository"] = "r"
            }))
            .BuildServiceProvider();
        var app = new ApplicationBuilder(services);

        var returned = app.UseGitHubWebhook();
        var pipeline = app.Build();
        var context = new DefaultHttpContext { RequestServices = services };
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/api/github/webhook";
        context.Response.Body = new MemoryStream();
        await pipeline(context);

        returned.ShouldBeSameAs(app);
        context.Response.StatusCode.ShouldBe(StatusCodes.Status202Accepted);
        await webhookHandler.Received(1).HandleWebhookAsync(context.Request);
    }

    [Fact]
    public void AddGitHubWebhookAndPolling_WithoutAGitHubProvider_StillBuildsAValidatedServiceProvider()
    {
        // Fails before the fix: IGitHubWebhookHandler needs IGitHubApiClientFactory, which only a GitHub provider
        // registered, so a host with scope validation (Development) failed to start with an unrelated error.
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton(Substitute.For<IContentProviderManager>())
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddGitHubWebhookAndPolling();

        var act = () => services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        Should.NotThrow(act).Dispose();
    }

    [Fact]
    public void UseGitHubWebhook_WithoutAGitHubProvider_FailsAtStartupNamingTheMissingProvider()
    {
        // Fails before the fix: the middleware was added anyway and every request to the app returned 500
        // because the webhook handler could not be resolved.
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton(Substitute.For<IContentProviderManager>())
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddGitHubWebhookAndPolling();
        var app = new ApplicationBuilder(services.BuildServiceProvider());

        var act = () => app.UseGitHubWebhook();

        var error = act.ShouldThrow<InvalidOperationException>();
        error.Message.ShouldContain("GitHub provider");
        error.Message.ShouldContain("Osirion:Cms:Web:GitHub");
    }

    [Fact]
    public void UseGitHubWebhook_WithNoFactoryRegistered_FailsAtStartupNamingTheMissingProvider()
    {
        var app = new ApplicationBuilder(new ServiceCollection().BuildServiceProvider());

        var act = () => app.UseGitHubWebhook();

        act.ShouldThrow<InvalidOperationException>().Message.ShouldContain("GitHub provider");
    }

    private static GitHubApiClientFactory GitHubApiClientFactoryWith(Dictionary<string, string?> values)
    {
        return new GitHubApiClientFactory(
            new ServiceCollection().BuildServiceProvider(),
            new ConfigurationBuilder().AddInMemoryCollection(values).Build(),
            NullLogger<GitHubApiClientFactory>.Instance);
    }
}
