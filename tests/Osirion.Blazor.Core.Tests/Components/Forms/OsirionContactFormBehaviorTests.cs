using System.Net;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Models;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Core.Services.Implementations;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Forms;

/// <summary>
/// The contact form layout options, the status message read back from the query string after a submission, and
/// the result each submission path navigates with.
/// </summary>
public class OsirionContactFormBehaviorTests : OsirionComponentTestBase
{
    private BunitNavigationManager Navigation => Services.GetRequiredService<BunitNavigationManager>();

    [Fact]
    public void ContactForm_Defaults_RendersTheSideBySideFormWithRequiredMarkers()
    {
        var cut = RenderStatic<OsirionContactForm>(parameters => { });

        cut.Find(".osirion-contact-form-container").ClassName.ShouldBe("osirion-contact-form-container osirion-contact-form-side-by-side");
        cut.Find("h2.osirion-contact-form-title").TextContent.ShouldBe("Write us");
        cut.FindAll(".osirion-contact-form-description").ShouldBeEmpty();
        cut.FindAll(".osirion-required").Count.ShouldBe(5);
        cut.FindAll(".osirion-form-checkbox-group").Count.ShouldBe(2);
        cut.FindAll("button[type=reset]").ShouldBeEmpty();
        cut.Find("button[type=submit]").TextContent.Trim().ShouldBe("Send Message");
        cut.FindAll(".osirion-status-icon").ShouldBeEmpty();
    }

    [Fact]
    public void ContactForm_StackedDarkLayout_AddsTheLayoutClasses()
    {
        var cut = RenderStatic<OsirionContactForm>(parameters => parameters
            .Add(p => p.SideBySideLayout, false)
            .Add(p => p.UseDarkTheme, true)
            .Add(p => p.Class, "my-form"));

        cut.Find(".osirion-contact-form-container").ClassName
            .ShouldBe("osirion-contact-form-container osirion-contact-form-stacked osirion-contact-form-dark my-form");
    }

    [Fact]
    public void ContactForm_ContactInfo_RendersTheContactInformationSection()
    {
        var cut = RenderStatic<OsirionContactForm>(parameters => parameters
            .Add(p => p.ContactInfo, new ContactInformation { Email = "hello@example.com" })
            .Add(p => p.ContactInfoTitle, "Reach us"));

        cut.Find(".osirion-contact-form-container").ClassList.ShouldContain("osirion-contact-form-with-info");
        cut.Markup.ShouldContain("Reach us");
        cut.Markup.ShouldContain("hello@example.com");
    }

    [Fact]
    public void ContactForm_ContactInfoHidden_RendersNoContactInformation()
    {
        var cut = RenderStatic<OsirionContactForm>(parameters => parameters
            .Add(p => p.ContactInfo, new ContactInformation { Email = "hello@example.com" })
            .Add(p => p.ShowContactInformation, false));

        cut.Find(".osirion-contact-form-container").ClassList.ShouldNotContain("osirion-contact-form-with-info");
        cut.Markup.ShouldNotContain("hello@example.com");
    }

    [Fact]
    public void ContactForm_CustomTexts_RendersThem()
    {
        var cut = RenderStatic<OsirionContactForm>(parameters => parameters
            .Add(p => p.Id, "f")
            .Add(p => p.FormTitle, "Talk to us")
            .Add(p => p.FormDescription, "We answer within a day.")
            .Add(p => p.NamePlaceholder, "Name here")
            .Add(p => p.EmailPlaceholder, "Mail here")
            .Add(p => p.SubjectPlaceholder, "Topic")
            .Add(p => p.MessagePlaceholder, "Text")
            .Add(p => p.MessageRows, 3)
            .Add(p => p.SubmitButtonText, "Send")
            .Add(p => p.SubscribeText, "Keep me posted"));

        cut.Find("h2.osirion-contact-form-title").TextContent.ShouldBe("Talk to us");
        cut.Find(".osirion-contact-form-description").TextContent.ShouldBe("We answer within a day.");
        cut.Find("#contact-name-f").GetAttribute("placeholder").ShouldBe("Name here");
        cut.Find("#contact-email-f").GetAttribute("placeholder").ShouldBe("Mail here");
        cut.Find("#contact-subject-f").GetAttribute("placeholder").ShouldBe("Topic");
        var message = cut.Find("#contact-message-f");
        message.GetAttribute("placeholder").ShouldBe("Text");
        message.GetAttribute("rows").ShouldBe("3");
        cut.Find("button[type=submit]").TextContent.Trim().ShouldBe("Send");
        cut.Markup.ShouldContain("Keep me posted");
    }

    [Fact]
    public void ContactForm_OptionalPartsToggled_RendersOnlyWhatIsEnabled()
    {
        var cut = RenderStatic<OsirionContactForm>(parameters => parameters
            .Add(p => p.FormTitle, " ")
            .Add(p => p.ShowRequiredIndicators, false)
            .Add(p => p.PrivacyPolicyText, (string?)null)
            .Add(p => p.ShowSubscribeOption, false)
            .Add(p => p.ShowResetButton, true)
            .Add(p => p.ResetButtonText, "Clear"));

        cut.FindAll(".osirion-contact-form-title").ShouldBeEmpty();
        cut.FindAll(".osirion-required").ShouldBeEmpty();
        cut.FindAll(".osirion-form-checkbox-group").ShouldBeEmpty();
        cut.Find("button[type=reset]").TextContent.Trim().ShouldBe("Clear");
    }

    [Theory]
    [InlineData(ContactFormSubmissionResult.Success, "osirion-alert-success", "bi-check-circle", "Thank you! Your message has been sent successfully. We'll get back to you soon.")]
    [InlineData(ContactFormSubmissionResult.ValidationError, "osirion-alert-error", "bi-exclamation-circle", "Please correct the errors below and try again.")]
    [InlineData(ContactFormSubmissionResult.ServerError, "osirion-alert-error", "bi-exclamation-circle", "Sorry, there was an error sending your message. Please try again later.")]
    [InlineData(ContactFormSubmissionResult.SpamDetected, "osirion-alert-warning", "bi-exclamation-triangle", "Your submission appears to be spam and has been blocked.")]
    public void ContactForm_SubmissionResultInQuery_RendersTheMatchingStatus(
        ContactFormSubmissionResult result, string alertClass, string iconClass, string message)
    {
        Navigation.NavigateTo($"http://localhost/contact?SubmissionResultValue={(int)result}");

        var cut = RenderStatic<OsirionContactForm>(parameters => { });

        var status = cut.Find($".{alertClass}");
        status.QuerySelector("svg")!.ClassList.ShouldContain(iconClass);
        status.QuerySelector("span")!.TextContent.ShouldBe(message);
    }

    [Theory]
    [InlineData(ContactFormSubmissionResult.Success, "Sent!")]
    [InlineData(ContactFormSubmissionResult.ServerError, "Failed!")]
    [InlineData(ContactFormSubmissionResult.SpamDetected, "Blocked!")]
    public void ContactForm_CustomStatusMessages_AreShownForTheirResult(ContactFormSubmissionResult result, string expected)
    {
        Navigation.NavigateTo($"http://localhost/contact?SubmissionResultValue={(int)result}");

        var cut = RenderStatic<OsirionContactForm>(parameters => parameters
            .Add(p => p.SuccessMessage, "Sent!")
            .Add(p => p.ErrorMessage, "Failed!")
            .Add(p => p.SpamMessage, "Blocked!"));

        cut.Find(".osirion-alert span").TextContent.ShouldBe(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(99)]
    [InlineData(-1)]
    public void ContactForm_NoFinalResultInQuery_RendersNoStatus(int value)
    {
        Navigation.NavigateTo($"http://localhost/contact?SubmissionResultValue={value}");

        var cut = RenderStatic<OsirionContactForm>(parameters => { });

        cut.FindAll(".osirion-alert").ShouldBeEmpty();
    }

    [Fact]
    public void ContactForm_ValidSubmit_InvokesOnSubmitAndReportsSuccess()
    {
        ContactFormModel? submitted = null;
        var cut = RenderStatic<OsirionContactForm>(parameters => parameters
            .Add(p => p.OnSubmit, (ContactFormModel model) => submitted = model));
        Fill(cut);

        cut.Find("form").Submit();

        submitted.ShouldNotBeNull().Email.ShouldBe("ann@example.com");
        Navigation.Uri.ShouldContain("SubmissionResultValue=1");
    }

    // Fails before the fix: with nothing to deliver the message the form told the visitor it was sent, and it was lost.
    [Fact]
    public void ContactForm_NoHandlerAndNoEmailService_ReportsServerErrorInsteadOfSuccess()
    {
        var cut = RenderStatic<OsirionContactForm>(parameters => { });
        Fill(cut);

        cut.Find("form").Submit();

        Navigation.Uri.ShouldContain("SubmissionResultValue=3");
    }

    [Fact]
    public void ContactForm_BuiltInEmailServiceNotRegistered_ReportsServerError()
    {
        var submitted = false;
        var cut = RenderStatic<OsirionContactForm>(parameters => parameters
            .Add(p => p.UseBuiltInEmailService, true)
            .Add(p => p.OnSubmit, (ContactFormModel _) => submitted = true));
        Fill(cut);

        cut.Find("form").Submit();

        submitted.ShouldBeFalse();
        Navigation.Uri.ShouldContain("SubmissionResultValue=3");
    }

    [Fact]
    public void ContactForm_OnSubmitThrows_ReportsServerError()
    {
        var cut = RenderStatic<OsirionContactForm>(parameters => parameters
            .Add(p => p.OnSubmit, (ContactFormModel _) => throw new InvalidOperationException("handler failed")));
        Fill(cut);

        cut.Find("form").Submit();

        Navigation.Uri.ShouldContain("SubmissionResultValue=3");
    }

    [Fact]
    public void ContactForm_InvalidSubmit_ShowsValidationMessagesAndDoesNotNavigate()
    {
        var submitted = false;
        var cut = RenderStatic<OsirionContactForm>(parameters => parameters
            .Add(p => p.OnSubmit, (ContactFormModel _) => submitted = true));
        var before = Navigation.Uri;

        cut.Find("form").Submit();

        submitted.ShouldBeFalse();
        Navigation.Uri.ShouldBe(before);
        cut.Find("#name-error-contact-form").TextContent.ShouldBe("Name is required");
    }

    [Fact]
    public void ContactForm_BuiltInEmailServiceSucceeds_InvokesOnSubmitAndReportsSuccess()
    {
        RegisterSendGrid(HttpStatusCode.Accepted);
        var submitted = false;
        var cut = RenderStatic<OsirionContactForm>(parameters => parameters
            .Add(p => p.UseBuiltInEmailService, true)
            .Add(p => p.OnSubmit, (ContactFormModel _) => submitted = true));
        Fill(cut);

        cut.Find("form").Submit();

        submitted.ShouldBeTrue();
        Navigation.Uri.ShouldContain("SubmissionResultValue=1");
    }

    [Fact]
    public void ContactForm_BuiltInEmailServiceFails_ReportsServerErrorWithoutOnSubmit()
    {
        RegisterSendGrid(HttpStatusCode.InternalServerError);
        var submitted = false;
        var cut = RenderStatic<OsirionContactForm>(parameters => parameters
            .Add(p => p.UseBuiltInEmailService, true)
            .Add(p => p.OnSubmit, (ContactFormModel _) => submitted = true));
        Fill(cut);

        cut.Find("form").Submit();

        submitted.ShouldBeFalse();
        Navigation.Uri.ShouldContain("SubmissionResultValue=3");
    }

    private void RegisterSendGrid(HttpStatusCode status)
    {
        var options = new EmailOptions { Provider = EmailProviderType.SendGrid, ToEmail = "to@example.com" };
        Services.AddSingleton(Options.Create(options));
        Services.AddSingleton(new SendGridEmailService(
            options, NullLogger<SendGridEmailService>.Instance, new HttpClient(new StatusHandler(status))));
        Services.AddTransient<EmailServiceFactory>();
    }

    private static void Fill(IRenderedComponent<OsirionContactForm> cut)
    {
        cut.Find("#contact-name-contact-form").Change("Ann");
        cut.Find("#contact-email-contact-form").Change("ann@example.com");
        cut.Find("#contact-subject-contact-form").Change("Hello");
        cut.Find("#contact-message-contact-form").Change("A message that is long enough");
        cut.Find(".osirion-form-checkbox-group input[type=checkbox]").Change(true);
    }

    private sealed class StatusHandler(HttpStatusCode status) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent("body") });
    }
}
