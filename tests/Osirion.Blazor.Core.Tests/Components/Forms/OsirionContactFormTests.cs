using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Forms;

public class OsirionContactFormTests : OsirionComponentTestBase
{
    // Fails before the fix: a placeholder New York address, phone and email were rendered for every form without ContactInfo.
    [Fact]
    public void ContactForm_WithoutContactInfo_RendersNoContactInformationSection()
    {
        var cut = RenderStatic<OsirionContactForm>(parameters => { });

        cut.FindAll(".osirion-contact-info-section").ShouldBeEmpty();
        cut.Markup.ShouldNotContain("New York");
        cut.Markup.ShouldNotContain("yoursite.com");
    }

    // Fails before the fix: the result started as SpamDetected, so a missing email service flagged a genuine visitor as spam.
    [Fact]
    public void ContactForm_BuiltInEmailServiceNotRegistered_ReportsServerErrorNotSpam()
    {
        var cut = RenderStatic<OsirionContactForm>(parameters => parameters
            .Add(p => p.UseBuiltInEmailService, true));
        Fill(cut);

        cut.Find("form").Submit();

        var navigation = Services.GetRequiredService<BunitNavigationManager>();
        navigation.Uri.ShouldContain("SubmissionResultValue=3");
    }

    [Fact]
    public void ContactForm_FilledHoneypot_ReportsSpam()
    {
        var cut = RenderStatic<OsirionContactForm>(parameters => { });
        Fill(cut);
        cut.Find("input.osirion-honeypot, .osirion-honeypot input").Change("http://spam.example");

        cut.Find("form").Submit();

        Services.GetRequiredService<BunitNavigationManager>().Uri.ShouldContain("SubmissionResultValue=4");
    }

    private static void Fill(IRenderedComponent<OsirionContactForm> cut)
    {
        cut.Find("#contact-name-contact-form").Change("Ann");
        cut.Find("#contact-email-contact-form").Change("ann@example.com");
        cut.Find("#contact-subject-contact-form").Change("Hello");
        cut.Find("#contact-message-contact-form").Change("A message that is long enough");
        cut.Find(".osirion-form-checkbox-group input[type=checkbox]").Change(true);
    }
}
