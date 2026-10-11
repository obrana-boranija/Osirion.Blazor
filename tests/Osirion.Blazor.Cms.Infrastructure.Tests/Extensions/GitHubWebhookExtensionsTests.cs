using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
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
}
