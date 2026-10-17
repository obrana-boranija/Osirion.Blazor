using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Models;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Sections;

public class OsirionContactInfoSectionTests : OsirionComponentTestBase
{
    private IRenderedComponent<OsirionContactInfoSection> Render(ContactInformation info) =>
        RenderStatic<OsirionContactInfoSection>(parameters => parameters.Add(p => p.ContactInfo, info));

    [Fact]
    public void ContactInfoSection_Renders()
    {
        var cut = Render(new ContactInformation { Email = "a@b.example" });

        cut.Find("a[href='mailto:a@b.example']").TextContent.ShouldBe("a@b.example");
    }

    // Fails before the fix: Class was written twice on the root, once by CombineCssClasses and once by the markup.
    [Fact]
    public void ContactInfoSection_Class_IsWrittenOnceOnTheRoot()
    {
        var cut = RenderStatic<OsirionContactInfoSection>(parameters => parameters
            .Add(p => p.ContactInfo, new ContactInformation { Email = "a@b.example" })
            .Add(p => p.Class, "extra"));

        cut.Find("div.osirion-contact-info-section").GetAttribute("class")!.Trim().ShouldBe("osirion-contact-info-section extra");
    }

    // Fails before the fix: StartsWith made a lookalike host count as the current site.
    [Fact]
    public void ContactInfoSection_LookalikeHostOfTheCurrentSite_IsAnExternalLink()
    {
        var baseUri = Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>().BaseUri.TrimEnd('/');

        var cut = Render(new ContactInformation { Website = baseUri + ".evil.net" });

        var link = cut.Find("a.osirion-contact-link");
        link.GetAttribute("href").ShouldBe(baseUri + ".evil.net");
        link.GetAttribute("target").ShouldBe("_blank");
    }

    // Fails before the fix: every same-site website was rewritten to "/".
    [Fact]
    public void ContactInfoSection_SameSiteWebsiteWithPath_KeepsThePath()
    {
        var baseUri = Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>().BaseUri;

        var cut = Render(new ContactInformation { Website = baseUri + "about?x=1" });

        var link = cut.Find("a.osirion-contact-link");
        link.GetAttribute("href").ShouldBe("/about?x=1");
        link.GetAttribute("target").ShouldBe("_self");
    }

    // Fails before the fix: GetWebsiteDisplayText indexed [0] of an empty array and threw during render.
    [Theory]
    [InlineData("https://")]
    [InlineData("http:///")]
    [InlineData("   ,  ")]
    public void ContactInfoSection_WebsiteWithoutHost_DoesNotThrowAndRendersNoLink(string website)
    {
        var cut = Render(new ContactInformation { Website = website });

        cut.FindAll("a.osirion-contact-link").ShouldBeEmpty();
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("data:text/html,<script>alert(1)</script>")]
    [InlineData("vbscript:msgbox(1)")]
    public void ContactInfoSection_NonHttpWebsite_RendersNoLink(string website)
    {
        var cut = Render(new ContactInformation { Website = website });

        cut.FindAll("a.osirion-contact-link").ShouldBeEmpty();
        cut.Markup.ShouldNotContain("javascript:");
    }

    [Fact]
    public void ContactInfoSection_SchemelessWebsite_IsLinkedAsHttps()
    {
        var cut = Render(new ContactInformation { Website = "example.org" });

        var link = cut.Find("a.osirion-contact-link");
        link.GetAttribute("href").ShouldBe("https://example.org/");
        link.TextContent.ShouldBe("example.org");
        link.GetAttribute("target").ShouldBe("_blank");
    }

    // Fails before the fix: phone, email and website text were written as raw markup.
    [Fact]
    public void ContactInfoSection_PhoneEmailAndWebsiteText_IsEncoded()
    {
        var cut = Render(new ContactInformation
        {
            Phone = "<img src=x onerror=alert(1)>",
            Email = "<img src=y onerror=alert(2)>"
        });

        cut.FindAll("img").ShouldBeEmpty();
        cut.FindAll("a.osirion-contact-link").Count.ShouldBe(2);
    }

    // Address and message may carry intentional markup (a consumer passes a paragraph), but never script.
    [Fact]
    public void ContactInfoSection_AddressMarkup_IsSanitized()
    {
        var cut = Render(new ContactInformation
        {
            Address = "<p>Main street 1</p><script>alert(1)</script><img src=x onerror=alert(2)>",
            Message = "Hello <b>there</b><script>alert(3)</script>"
        });

        cut.FindAll("script").ShouldBeEmpty();
        cut.Markup.ShouldNotContain("onerror");
        cut.Find(".osirion-contact-info-value").InnerHtml.ShouldContain("<p>Main street 1</p>");
        cut.Find(".osirion-contact-info-description").InnerHtml.ShouldContain("<b>there</b>");
    }
}
