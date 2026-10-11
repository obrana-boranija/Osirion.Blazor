using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Models;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Core.Services.Implementations;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Services;

/// <summary>
/// How each email provider reports a failed send, the payload addressing, the factory and the configuration binding.
/// </summary>
public class EmailServiceFailureTests
{
    private static ContactFormModel Form() => new()
    {
        Name = "Ann",
        Email = "ann@example.com",
        Subject = "Hi",
        Message = "A message that is long enough"
    };

    private static EmailOptions Options() => new()
    {
        ToEmail = "to@example.com",
        FromName = "Site",
        SubjectTemplate = "[Site] {subject}",
        SendGrid = { ApiKey = "sg-key", FromEmail = "from@example.com" },
        Infobip = { ApiKey = "ib-key", FromEmail = "from@example.com", BaseUrl = "https://example.api.infobip.com" },
        Smtp = { Host = "smtp.example.com", Port = 587, FromEmail = "from@example.com" }
    };

    [Fact]
    public async Task SendGrid_ApiRejects_ReturnsFailureWithStatusAndBody()
    {
        using var client = new HttpClient(new StubHandler(HttpStatusCode.BadRequest, "{\"errors\":[]}"));
        var service = new SendGridEmailService(Options(), NullLogger<SendGridEmailService>.Instance, client);

        var result = await service.SendEmailAsync(Form());

        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("SendGrid API error: BadRequest");
        result.Details.ShouldBe("{\"errors\":[]}");
    }

    [Fact]
    public async Task SendGrid_TransportFails_ReturnsFailureWithTheExceptionMessage()
    {
        using var client = new HttpClient(new StubHandler(new HttpRequestException("connection refused")));
        var service = new SendGridEmailService(Options(), NullLogger<SendGridEmailService>.Instance, client);

        var result = await service.SendEmailAsync(Form());

        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("SendGrid error: connection refused");
        result.Details.ShouldNotBeNull().ShouldContain(nameof(HttpRequestException));
    }

    [Fact]
    public async Task SendGrid_Payload_AddressesTheRecipientSenderAndReplyTo()
    {
        var handler = new StubHandler(HttpStatusCode.Accepted, string.Empty);
        using var client = new HttpClient(handler);
        var service = new SendGridEmailService(Options(), NullLogger<SendGridEmailService>.Instance, client);

        var result = await service.SendEmailAsync(Form());

        result.IsSuccess.ShouldBeTrue();
        result.Details.ShouldBe("Email sent via SendGrid");
        handler.Request!.RequestUri!.ToString().ShouldBe("https://api.sendgrid.com/v3/mail/send");
        using var payload = JsonDocument.Parse(handler.Body);
        var personalization = payload.RootElement.GetProperty("personalizations")[0];
        personalization.GetProperty("to")[0].GetProperty("email").GetString().ShouldBe("to@example.com");
        personalization.GetProperty("subject").GetString().ShouldBe("[Site] Hi");
        payload.RootElement.GetProperty("from").GetProperty("email").GetString().ShouldBe("from@example.com");
        payload.RootElement.GetProperty("from").GetProperty("name").GetString().ShouldBe("Site");
        payload.RootElement.GetProperty("reply_to").GetProperty("email").GetString().ShouldBe("ann@example.com");
        payload.RootElement.GetProperty("reply_to").GetProperty("name").GetString().ShouldBe("Ann");
    }

    [Fact]
    public async Task Infobip_ApiRejects_ReturnsFailureWithStatusAndBody()
    {
        using var client = new HttpClient(new StubHandler(HttpStatusCode.Unauthorized, "denied"));
        var service = new InfobipEmailService(Options(), NullLogger<InfobipEmailService>.Instance, client);

        var result = await service.SendEmailAsync(Form());

        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("Infobip API error: Unauthorized");
        result.Details.ShouldBe("denied");
    }

    [Fact]
    public async Task Infobip_TransportFails_ReturnsFailureWithTheExceptionMessage()
    {
        using var client = new HttpClient(new StubHandler(new HttpRequestException("timeout")));
        var service = new InfobipEmailService(Options(), NullLogger<InfobipEmailService>.Instance, client);

        var result = await service.SendEmailAsync(Form());

        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("Infobip error: timeout");
    }

    [Fact]
    public async Task Infobip_RelativeBaseUrl_ReturnsFailureWithoutSending()
    {
        var options = Options();
        options.Infobip.BaseUrl = "not-a-url";
        var handler = new StubHandler(HttpStatusCode.OK, string.Empty);
        using var client = new HttpClient(handler);
        var service = new InfobipEmailService(options, NullLogger<InfobipEmailService>.Instance, client);

        var result = await service.SendEmailAsync(Form());

        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNull().ShouldStartWith("Infobip error: ");
        handler.Request.ShouldBeNull();
    }

    [Fact]
    public async Task Infobip_Payload_AddressesTheRecipientSenderAndReplyTo()
    {
        var handler = new StubHandler(HttpStatusCode.OK, string.Empty);
        using var client = new HttpClient(handler);
        var service = new InfobipEmailService(Options(), NullLogger<InfobipEmailService>.Instance, client);

        var result = await service.SendEmailAsync(Form());

        result.Details.ShouldBe("Email sent via Infobip");
        using var payload = JsonDocument.Parse(handler.Body);
        var message = payload.RootElement.GetProperty("messages")[0];
        message.GetProperty("to")[0].GetProperty("email").GetString().ShouldBe("to@example.com");
        message.GetProperty("from").GetProperty("email").GetString().ShouldBe("from@example.com");
        message.GetProperty("from").GetProperty("name").GetString().ShouldBe("Site");
        message.GetProperty("replyTo")[0].GetProperty("email").GetString().ShouldBe("ann@example.com");
        message.GetProperty("subject").GetString().ShouldBe("[Site] Hi");
    }

    [Theory]
    [InlineData("", "to@example.com", 587)]
    [InlineData("from@example.com", "not an address", 587)]
    [InlineData("from@example.com", "to@example.com", 0)]
    public async Task Smtp_InvalidConfiguration_ReturnsFailureWithoutSending(string fromEmail, string toEmail, int port)
    {
        var options = Options();
        options.Smtp.FromEmail = fromEmail;
        options.Smtp.Port = port;
        options.ToEmail = toEmail;
        var service = new SmtpEmailService(Microsoft.Extensions.Options.Options.Create(options), NullLogger<SmtpEmailService>.Instance);

        var result = await service.SendEmailAsync(Form());

        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNull().ShouldStartWith("SMTP error: ");
        result.Details.ShouldNotBeNullOrEmpty();
    }

    // SmtpClient checks for a host before it opens a connection, so this reaches the send call without a network.
    [Fact]
    public async Task Smtp_NoHost_ReturnsFailureFromTheSendCall()
    {
        var options = Options();
        options.Smtp.Host = string.Empty;
        var service = new SmtpEmailService(Microsoft.Extensions.Options.Options.Create(options), NullLogger<SmtpEmailService>.Instance);

        var result = await service.SendEmailAsync(Form());

        result.IsSuccess.ShouldBeFalse();
        result.Details.ShouldNotBeNull().ShouldContain(nameof(InvalidOperationException));
    }

    // The reserved .invalid host never resolves, so no server can be reached whatever SmtpClient does first.
    [Fact]
    public async Task Smtp_CancelledBeforeSending_ReturnsFailure()
    {
        var options = Options();
        options.Smtp.Host = "smtp.invalid";
        var service = new SmtpEmailService(Microsoft.Extensions.Options.Options.Create(options), NullLogger<SmtpEmailService>.Instance);
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        var result = await service.SendEmailAsync(Form(), cancellation.Token);

        result.IsSuccess.ShouldBeFalse();
        result.Details.ShouldNotBeNull().ShouldContain("Canceled");
    }

    [Fact]
    public void EmailResult_Success_CarriesTheDetails()
    {
        var result = EmailResult.Success("sent");

        result.IsSuccess.ShouldBeTrue();
        result.ErrorMessage.ShouldBeNull();
        result.Details.ShouldBe("sent");
    }

    [Fact]
    public void EmailResult_Failure_CarriesTheErrorAndDetails()
    {
        var result = EmailResult.Failure("failed", "trace");

        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("failed");
        result.Details.ShouldBe("trace");
    }

    [Fact]
    public void EmailServiceFactory_UnsupportedProvider_Throws()
    {
        using var provider = new ServiceCollection().BuildServiceProvider();
        var factory = new EmailServiceFactory(provider, Microsoft.Extensions.Options.Options.Create(new EmailOptions { Provider = (EmailProviderType)99 }));

        Should.Throw<InvalidOperationException>(factory.CreateEmailService).Message.ShouldBe("Unsupported email provider: 99");
    }

    [Fact]
    public void AddOsirionEmailServices_Configuration_BindsTheEmailSection()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Osirion:Email:Provider"] = "Infobip",
                ["Osirion:Email:ToEmail"] = "inbox@example.com",
                ["Osirion:Email:Infobip:ApiKey"] = "key"
            })
            .Build();
        using var provider = new ServiceCollection()
            .AddLogging()
            .AddOsirionEmailServices(configuration)
            .BuildServiceProvider();

        provider.GetRequiredService<IOptions<EmailOptions>>().Value.ToEmail.ShouldBe("inbox@example.com");
        provider.GetRequiredService<IEmailService>().ShouldBeOfType<InfobipEmailService>();
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _status;
        private readonly string _responseBody = string.Empty;
        private readonly Exception? _exception;

        public StubHandler(HttpStatusCode status, string responseBody)
        {
            _status = status;
            _responseBody = responseBody;
        }

        public StubHandler(Exception exception)
        {
            _exception = exception;
        }

        public HttpRequestMessage? Request { get; private set; }

        public string Body { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_exception is not null)
            {
                throw _exception;
            }

            Request = request;
            Body = await request.Content!.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(_status) { Content = new StringContent(_responseBody) };
        }
    }
}
