using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Models;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Core.Services.Implementations;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Services;

public class EmailServicesTests
{
    private static ContactFormModel Form() => new()
    {
        Name = "<b>Ann</b>",
        Email = "ann@example.com",
        Subject = "Hi",
        Message = "line1\n<script>x</script>",
        SubscribeToUpdates = true
    };

    private static EmailOptions Options() => new()
    {
        ToEmail = "to@example.com",
        SendGrid = { ApiKey = "sg-key", FromEmail = "from@example.com" },
        Infobip = { ApiKey = "ib-key", FromEmail = "from@example.com", BaseUrl = "https://example.api.infobip.com" }
    };

    [Fact]
    public async Task SendGrid_SendEmailAsync_SetsAuthorizationPerRequestAndLeavesClientDefaultsAlone()
    {
        var handler = new CapturingHandler();
        using var client = new HttpClient(handler);
        var service = new SendGridEmailService(Options(), NullLogger<SendGridEmailService>.Instance, client);

        var result = await service.SendEmailAsync(Form(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        client.DefaultRequestHeaders.Authorization.ShouldBeNull();
        handler.Request!.Headers.Authorization!.ToString().ShouldBe("Bearer sg-key");
        handler.Body.ShouldContain("&lt;b&gt;Ann&lt;/b&gt;");
        handler.Body.ShouldNotContain("<script>");
    }

    [Fact]
    public async Task Infobip_SendEmailAsync_SetsAuthorizationAndTargetPerRequestAndLeavesClientDefaultsAlone()
    {
        var handler = new CapturingHandler();
        using var client = new HttpClient(handler);
        var service = new InfobipEmailService(Options(), NullLogger<InfobipEmailService>.Instance, client);

        var result = await service.SendEmailAsync(Form(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        client.DefaultRequestHeaders.Authorization.ShouldBeNull();
        client.BaseAddress.ShouldBeNull();
        handler.Request!.Headers.Authorization!.ToString().ShouldBe("App ib-key");
        handler.Request.RequestUri!.ToString().ShouldBe("https://example.api.infobip.com/email/3/send");
        handler.Body.ShouldContain("&lt;b&gt;Ann&lt;/b&gt;");
    }

    [Fact]
    public async Task Providers_ShareTheSameBodyLayout()
    {
        var sendGrid = new CapturingHandler();
        var infobip = new CapturingHandler();
        using var sendGridClient = new HttpClient(sendGrid);
        using var infobipClient = new HttpClient(infobip);

        await new SendGridEmailService(Options(), NullLogger<SendGridEmailService>.Instance, sendGridClient)
            .SendEmailAsync(Form(), CancellationToken.None);
        await new InfobipEmailService(Options(), NullLogger<InfobipEmailService>.Instance, infobipClient)
            .SendEmailAsync(Form(), CancellationToken.None);

        foreach (var body in new[] { sendGrid.Body, infobip.Body })
        {
            body.ShouldContain("New Contact Form Submission");
            body.ShouldContain("requested to receive updates");
        }
    }

    [Theory]
    [InlineData("Osirion.Email.SendGrid")]
    [InlineData("Osirion.Email.Infobip")]
    public void AddOsirionEmailServices_ProviderHttpClientsHaveAFiniteTimeout(string clientName)
    {
        using var provider = new ServiceCollection().AddOsirionEmailServices(o => o.ToEmail = "a@b.c").BuildServiceProvider();

        var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient(clientName);

        client.Timeout.ShouldBe(TimeSpan.FromSeconds(30));
    }

    [Theory]
    [InlineData(EmailProviderType.SendGrid, typeof(SendGridEmailService))]
    [InlineData(EmailProviderType.Infobip, typeof(InfobipEmailService))]
    [InlineData(EmailProviderType.Smtp, typeof(SmtpEmailService))]
    public void AddOsirionEmailServices_ResolvesConfiguredProvider(EmailProviderType type, Type expected)
    {
        using var provider = new ServiceCollection()
            .AddLogging()
            .AddOsirionEmailServices(o => { o.Provider = type; o.SendGrid.ApiKey = "k"; })
            .BuildServiceProvider();

        provider.GetRequiredService<IEmailService>().ShouldBeOfType(expected);
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        public string Body { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            var json = await request.Content!.ReadAsStringAsync(cancellationToken);
            Body = FindHtml(JsonDocument.Parse(json).RootElement) ?? json;
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private static string? FindHtml(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        if (property.Name is "html" or "value" && property.Value.ValueKind == JsonValueKind.String)
                        {
                            return property.Value.GetString();
                        }

                        if (FindHtml(property.Value) is { } nested)
                        {
                            return nested;
                        }
                    }

                    return null;
                case JsonValueKind.Array:
                    return element.EnumerateArray().Select(FindHtml).FirstOrDefault(found => found is not null);
                default:
                    return null;
            }
        }
    }
}
