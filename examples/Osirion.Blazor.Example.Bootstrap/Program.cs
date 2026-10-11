using Osirion.Blazor.Cms.Infrastructure.Extensions;
using Osirion.Blazor.Example.Bootstrap.Components;
using Osirion.Blazor.Extensions;
using Osirion.Blazor.Cms.Web.Middleware;
using Osirion.Blazor.Cms.Web.DependencyInjection;
using Osirion.Blazor.Cms.Web.Feeds;
using Osirion.Blazor.Cms.Web.Redirects;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.ResponseCompression;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Example.Bootstrap.Components.Demos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// SEO metadata is automatically loaded from appsettings.json (Osirion:Seo section)
builder.Services.AddOsirion(builder.Configuration);
builder.Services.AddGitHubWebhookAndPolling();
builder.Services.AddJSComponents();
builder.Services.AddOsirionCookieConsent();

// Live demos that Markdown content embeds with a fenced "demo" block
builder.Services.AddOsirionContentDemos(demos => demos
    .Add<InstallTabsDemo>("install-tabs", title: "Install command tabs", source: ContentDemoRegistry.ReadEmbeddedSource(
        typeof(InstallTabsDemo).Assembly, "Osirion.Blazor.Example.Bootstrap.Components.Demos.InstallTabsDemo.razor")));

// Optional: Override SEO metadata configuration programmatically
// Uncomment below to override values from appsettings.json
/*
builder.Services.AddOsirionSeoMetadata(seo =>
{
    seo.SiteName = "Osirion Blazor";
    seo.SiteDescription = "A modular, high-performance component library for Blazor applications";
    seo.OrganizationName = "Osirion";
    seo.TwitterSite = "@osirionblazor";
    seo.DefaultImageUrl = "/images/og-default.png";
    seo.SiteLogoUrl = "/images/logo.png";
    seo.AllowAiDiscovery = true;
    seo.AllowAiTraining = true;
    seo.EnableGeoOptimization = true;
    seo.EnableAeoOptimization = true;
});
*/

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // Enable for HTTPS (be aware of BREACH attacks)
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json", "application/xml", "text/csv" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Canonical host and scheme redirects belong to deployed sites; local runs (and tools that browse the dev server
// by another host name) work over plain HTTP.
if (!app.Environment.IsDevelopment())
{
    var rewriteOptions = new RewriteOptions()
        .AddRedirectToHttpsPermanent() // never include if proxy has redirection
        .AddRedirectToNonWwwPermanent();
    app.UseRewriter(rewriteOptions);
}

app.UseGitHubWebhook();
app.UseOsirionRobotsGenerator();
app.UseResponseCompression();
app.MapOsirionCookieConsent();
app.MapOsirionAnnouncements();


app.UseAntiforgery();
app.MapStaticAssets();

// Old URLs listed in redirect_from front matter answer with a 301 to the page's current address.
app.UseOsirionRedirects();

// /sitemap.xml, /feed.xml (RSS) and /atom.xml from the published content. A deployed site sets SiteUrl so the
// URLs do not depend on the request's Host header.
app.MapOsirionFeeds(feeds =>
{
    feeds.FeedDirectory = "blog";
    feeds.AdditionalSitemapUrls = ["/", "/blog", "/showcase"];
    // home.md is served at "/", not at its own slug
    feeds.ContentUrlFormatter = item => item.Url.Trim('/') == "homepage" ? "/" : "/" + item.Url.TrimStart('/');
});

// The CMS admin pages (/osirion/...) require the CMS admin authorization policy. This example registers no
// authentication scheme, so requests to them end in an error instead of rendering. A real host adds its own
// sign-in first; see "Securing the Admin" in the Osirion.Blazor.Cms.Admin README.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Osirion.Blazor.Cms.Admin._Imports).Assembly);

app.Run();
