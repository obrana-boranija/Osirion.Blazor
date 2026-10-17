using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Models;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Core.Services.Implementations;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Cards;

public class OsirionSubscriptionCardTests : OsirionComponentTestBase
{
    private const string Email = "ann@example.com";

    [Fact]
    public void SubscriptionCard_Defaults_RendersTitleDescriptionEmailAndPrivacyCheckbox()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => { });

        cut.Find("h3.osirion-subscription-card-title").TextContent.ShouldBe("Stay Updated");
        cut.Find("p.osirion-subscription-card-description").TextContent.ShouldContain("latest updates");
        cut.Find("form").ClassList.ShouldContain("osirion-subscription-form");
        cut.Find("#subscription-email-subscription-card").GetAttribute("type").ShouldBe("email");
        cut.FindAll("#subscription-name-subscription-card").ShouldBeEmpty();
        cut.Find("a.osirion-subscribtion-link").GetAttribute("href").ShouldBe("/privacy-policy");
        cut.FindAll(".osirion-required").Count.ShouldBe(2);
        cut.Find("button[type=submit]").TextContent.Trim().ShouldBe("Subscribe");
        cut.FindAll(".osirion-subscription-categories").ShouldBeEmpty();
        cut.FindAll(".osirion-subscription-card-footer").ShouldBeEmpty();
    }

    [Fact]
    public void SubscriptionCard_Defaults_HasBorderAndShadowClasses()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => { });

        var classes = cut.Find("div.osirion-subscription-card").ClassList;
        classes.ShouldContain("osirion-subscription-card-bordered");
        classes.ShouldContain("osirion-subscription-card-shadow");
        classes.ShouldNotContain("osirion-subscription-card-compact");
        classes.ShouldNotContain("osirion-subscription-card-dark");
    }

    [Fact]
    public void SubscriptionCard_LayoutFlagsInverted_AddsCompactAndDarkAndDropsBorderAndShadow()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.CompactLayout, true)
            .Add(p => p.UseDarkTheme, true)
            .Add(p => p.ShowBorder, false)
            .Add(p => p.ShowShadow, false)
            .Add(p => p.Class, "custom"));

        var classes = cut.Find("div.osirion-subscription-card").ClassList;
        classes.ShouldContain("osirion-subscription-card-compact");
        classes.ShouldContain("osirion-subscription-card-dark");
        classes.ShouldContain("custom");
        classes.ShouldNotContain("osirion-subscription-card-bordered");
        classes.ShouldNotContain("osirion-subscription-card-shadow");
    }

    [Fact]
    public void SubscriptionCard_EmptyTitleDescriptionAndPrivacyText_OmitsThoseElements()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.Title, " ")
            .Add(p => p.Description, string.Empty)
            .Add(p => p.PrivacyPolicyText, null)
            .Add(p => p.ShowRequiredIndicators, false));

        cut.FindAll("h3").ShouldBeEmpty();
        cut.FindAll(".osirion-subscription-card-description").ShouldBeEmpty();
        cut.FindAll("input[type=checkbox]").ShouldBeEmpty();
        cut.FindAll(".osirion-required").ShouldBeEmpty();
    }

    [Fact]
    public void SubscriptionCard_ShowNameFieldAndCustomTexts_RendersThemWithTheCardId()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.Id, "news")
            .Add(p => p.ShowNameField, true)
            .Add(p => p.NamePlaceholder, "Who are you")
            .Add(p => p.EmailPlaceholder, "Where to")
            .Add(p => p.SubscribeButtonText, "Join"));

        cut.Find("#subscription-name-news").GetAttribute("placeholder").ShouldBe("Who are you");
        cut.Find("#subscription-email-news").GetAttribute("placeholder").ShouldBe("Where to");
        cut.Find("label[for=subscription-name-news]").TextContent.Trim().ShouldBe("Name");
        cut.Find("button[type=submit]").TextContent.Trim().ShouldBe("Join");
    }

    [Fact]
    public void SubscriptionCard_ChildContent_RendersFooter()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "No spam, ever."))));

        cut.Find(".osirion-subscription-card-footer").TextContent.Trim().ShouldBe("No spam, ever.");
    }

    [Fact]
    public void SubscriptionCard_ShowCategoriesWithoutList_RendersDefaultCategoriesWithDefaultsChecked()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.ShowSubscriptionCategories, true));

        var boxes = cut.FindAll("fieldset input[type=checkbox]");
        boxes.Select(b => b.GetAttribute("value")).ShouldBe(["newsletter", "updates", "tips"]);
        boxes.Select(b => b.HasAttribute("checked")).ShouldBe([true, true, false]);
        boxes.ShouldAllBe(b => !b.HasAttribute("disabled"));
        cut.Find("fieldset legend").TextContent.ShouldBe("What would you like to receive?");
        cut.FindAll(".osirion-checkbox-description").Count.ShouldBe(3);
    }

    [Fact]
    public void SubscriptionCard_CustomCategoriesInline_MarksRequiredCategoriesCheckedAndDisabled()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.Id, "c")
            .Add(p => p.ShowSubscriptionCategories, true)
            .Add(p => p.SubscriptionCategories, Categories()));

        var required = cut.Find("#subscription-category-legal-c");
        required.HasAttribute("checked").ShouldBeTrue();
        required.HasAttribute("disabled").ShouldBeTrue();
        var optional = cut.Find("#subscription-category-promo-c");
        optional.HasAttribute("checked").ShouldBeFalse();
        optional.HasAttribute("disabled").ShouldBeFalse();
        cut.FindAll(".osirion-subscription-category-item .osirion-required").Count.ShouldBe(1);
        cut.FindAll(".osirion-checkbox-description").Count.ShouldBe(1);
    }

    [Fact]
    public void SubscriptionCard_CategoriesAsDropdown_RendersDetailsPanelWithRequiredBadge()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.Id, "c")
            .Add(p => p.ShowSubscriptionCategories, true)
            .Add(p => p.ShowCategoriesAsDropdown, true)
            .Add(p => p.CategoriesDropdownText, "Pick")
            .Add(p => p.CategoriesText, "Topics")
            .Add(p => p.SubscriptionCategories, Categories()));

        cut.FindAll("fieldset").ShouldBeEmpty();
        cut.Find("details.osirion-custom-dropdown .osirion-dropdown-text").TextContent.ShouldBe("Pick");
        cut.Find(".osirion-subscription-categories-dropdown label").TextContent.ShouldBe("Topics");
        cut.FindAll(".osirion-dropdown-option").Count.ShouldBe(2);
        cut.FindAll(".osirion-required-badge").Count.ShouldBe(1);
        cut.FindAll(".osirion-option-description").Count.ShouldBe(1);
        cut.Find("#subscription-category-legal-c").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void SubscriptionCard_ShowCategoriesWithEmptyList_RendersNoCategorySection()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.ShowSubscriptionCategories, true)
            .Add(p => p.SubscriptionCategories, Array.Empty<SubscriptionCategory>()));

        cut.FindAll(".osirion-subscription-categories").ShouldBeEmpty();
    }

    [Fact]
    public void SubscriptionCard_CategoriesProvidedButHidden_RendersNoCategorySection()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.SubscriptionCategories, Categories()));

        cut.FindAll(".osirion-subscription-categories").ShouldBeEmpty();
    }

    [Theory]
    [InlineData(1, "osirion-alert-success", "bi-check-circle", "Thank you for subscribing!")]
    [InlineData(2, "osirion-alert-error", "bi-exclamation-circle", "Please correct the errors below and try again.")]
    [InlineData(3, "osirion-alert-error", "bi-exclamation-circle", "Sorry, there was an error")]
    [InlineData(4, "osirion-alert-warning", "bi-exclamation-triangle", "appears to be spam")]
    [InlineData(6, "osirion-alert-info", "bi-info-circle", "already subscribed")]
    public void SubscriptionCard_SubmissionResultInQuery_RendersMatchingStatus(int result, string alertClass, string iconClass, string message)
    {
        NavigateTo($"http://localhost/?SubmissionResultValue={result}");

        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => { });

        var status = cut.Find($"div.{alertClass}");
        status.QuerySelector($"svg.osirion-status-icon.{iconClass}").ShouldNotBeNull();
        status.QuerySelector("span")!.TextContent.ShouldContain(message);
    }

    [Theory]
    [InlineData(1, "Welcome aboard")]
    [InlineData(3, "Try later")]
    [InlineData(4, "Blocked")]
    [InlineData(6, "Known address")]
    public void SubscriptionCard_CustomMessages_AreShownForTheirResult(int result, string message)
    {
        NavigateTo($"http://localhost/?SubmissionResultValue={result}");

        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.SuccessMessage, "Welcome aboard")
            .Add(p => p.ErrorMessage, "Try later")
            .Add(p => p.SpamMessage, "Blocked")
            .Add(p => p.AlreadySubscribedMessage, "Known address"));

        cut.Find("div.osirion-alert span").TextContent.ShouldBe(message);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("5")]
    [InlineData("42")]
    [InlineData("-1")]
    public void SubscriptionCard_NoneProcessingOrUnknownResult_RendersNoStatus(string value)
    {
        NavigateTo($"http://localhost/?SubmissionResultValue={value}");

        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => { });

        cut.FindAll(".osirion-status-icon").ShouldBeEmpty();
        cut.FindAll("div.osirion-alert").ShouldBeEmpty();
    }

    [Fact]
    public void SubscriptionCard_ValidSubmit_InvokesOnSubscribeAndNavigatesWithSuccess()
    {
        SubscriptionModel? received = null;
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.ShowNameField, true)
            .Add(p => p.OnSubscribe, (SubscriptionModel model) => received = model));
        cut.Find("#subscription-name-subscription-card").Change("Ann");
        Fill(cut);

        cut.Find("form").Submit();

        received.ShouldNotBeNull();
        received.Email.ShouldBe(Email);
        received.Name.ShouldBe("Ann");
        received.AgreeToPrivacyPolicy.ShouldBeTrue();
        CurrentUri().ShouldContain("SubmissionResultValue=1");
    }

    [Fact]
    public void SubscriptionCard_ValidSubmit_ShowsSuccessStatusAfterNavigation()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.OnSubscribe, (SubscriptionModel _) => { }));
        Fill(cut);

        cut.Find("form").Submit();

        cut.Find("div.osirion-alert-success span").TextContent.ShouldStartWith("Thank you for subscribing!");
    }

    [Fact]
    public void SubscriptionCard_InvalidEmail_DoesNotSubmitAndShowsValidationMessage()
    {
        var invoked = false;
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.OnSubscribe, (SubscriptionModel _) => invoked = true));
        cut.Find("#subscription-email-subscription-card").Change("not-an-email");
        cut.Find("div.osirion-form-group.osirion-form-checkbox-group input[type=checkbox]").Change(true);

        cut.Find("form").Submit();

        invoked.ShouldBeFalse();
        CurrentUri().ShouldNotContain("SubmissionResultValue");
        cut.Find("#email-error-subscription-card").TextContent.ShouldBe("Please enter a valid email address");
    }

    [Fact]
    public void SubscriptionCard_FilledHoneypot_ReportsSpamWithoutInvokingOnSubscribe()
    {
        var invoked = false;
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.OnSubscribe, (SubscriptionModel _) => invoked = true));
        Fill(cut);
        cut.Find(".osirion-honeypot input").Change("http://spam.example");

        cut.Find("form").Submit();

        invoked.ShouldBeFalse();
        CurrentUri().ShouldContain("SubmissionResultValue=4");
    }

    [Fact]
    public void SubscriptionCard_OnSubscribeThrows_ReportsServerError()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.OnSubscribe, (SubscriptionModel _) => throw new InvalidOperationException("boom")));
        Fill(cut);

        cut.Find("form").Submit();

        CurrentUri().ShouldContain("SubmissionResultValue=3");
    }

    // Fails before the fix: with the built-in email service registered the card reported success, and thanked the
    // visitor, without sending or storing anything.
    [Fact]
    public void SubscriptionCard_BuiltInEmailServiceRegistered_ReportsServerErrorBecauseNothingStoresTheAddress()
    {
        Services.AddSingleton(Options.Create(new EmailOptions { Provider = EmailProviderType.Smtp }));
        Services.AddSingleton<SmtpEmailService>();
        Services.AddSingleton<EmailServiceFactory>();
        var invoked = false;
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.UseBuiltInEmailService, true)
            .Add(p => p.OnSubscribe, (SubscriptionModel _) => invoked = true));
        Fill(cut);

        cut.Find("form").Submit();

        invoked.ShouldBeFalse();
        CurrentUri().ShouldContain("SubmissionResultValue=3");
    }

    // Fails before the fix: without an OnSubscribe handler the card said "Thank you for subscribing!" and the address
    // went nowhere.
    [Fact]
    public void SubscriptionCard_NoOnSubscribeHandler_ReportsServerError()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => { });
        Fill(cut);

        cut.Find("form").Submit();

        CurrentUri().ShouldContain("SubmissionResultValue=3");
        cut.Find("div.osirion-alert-error span").TextContent.ShouldStartWith("Sorry, there was an error");
    }

    // Fails before the fix: the result started as SpamDetected, so a missing email service flagged a genuine visitor as spam.
    [Fact]
    public void SubscriptionCard_BuiltInEmailServiceNotRegistered_ReportsServerErrorNotSpam()
    {
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.UseBuiltInEmailService, true));
        Fill(cut);

        cut.Find("form").Submit();

        CurrentUri().ShouldContain("SubmissionResultValue=3");
    }

    // Known defect, pinned as current behavior: ticked categories are read only from a static form post, so an interactive submit drops the visitor's choices and keeps the required ones. Update this expectation when it is fixed.
    [Fact]
    public void SubscriptionCard_SubmitWithoutPostedCategories_KeepsRequiredCategories()
    {
        SubscriptionModel? received = null;
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.ShowSubscriptionCategories, true)
            .Add(p => p.SubscriptionCategories, Categories())
            .Add(p => p.OnSubscribe, (SubscriptionModel model) => received = model));
        Fill(cut);

        cut.Find("form").Submit();

        received.ShouldNotBeNull();
        received.SubscriptionCategories.ShouldBe(["legal"]);
    }

    [Fact]
    public void SubscriptionCard_SubmitWithoutCategories_LeavesCategoriesEmpty()
    {
        SubscriptionModel? received = null;
        var cut = RenderStatic<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.SubscriptionCategories, Categories())
            .Add(p => p.OnSubscribe, (SubscriptionModel model) => received = model));
        Fill(cut);

        cut.Find("form").Submit();

        received.ShouldNotBeNull();
        received.SubscriptionCategories.ShouldBeEmpty();
    }

    [Fact]
    public void SubscriptionCard_InteractiveSubmit_InvokesOnSubscribeAndReportsSuccess()
    {
        SetRendererInfo(new RendererInfo("Server", true));
        SubscriptionModel? received = null;
        var cut = Render<OsirionSubscriptionCard>(parameters => parameters
            .Add(p => p.Id, "live")
            .Add(p => p.OnSubscribe, (SubscriptionModel model) => received = model));
        Fill(cut);

        cut.Find("form").Submit();

        received.ShouldNotBeNull().Email.ShouldBe(Email);
        CurrentUri().ShouldContain("SubmissionResultValue=1");
    }

    private static SubscriptionCategory[] Categories() =>
    [
        new() { Id = "legal", Name = "Account notices", Description = "Required notices", IsRequired = true },
        new() { Id = "promo", Name = "Promotions" },
    ];

    private void NavigateTo(string uri) => Services.GetRequiredService<BunitNavigationManager>().NavigateTo(uri);

    private string CurrentUri() => Services.GetRequiredService<BunitNavigationManager>().Uri;

    private static void Fill(IRenderedComponent<OsirionSubscriptionCard> cut)
    {
        cut.Find("input[type=email]").Change(Email);
        cut.Find("div.osirion-form-group.osirion-form-checkbox-group input[type=checkbox]").Change(true);
    }
}
