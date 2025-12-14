using Osirion.Blazor.Cms.Infrastructure.Extensions;
using Osirion.Blazor.Example.Bootstrap.Components;
using Osirion.Blazor.Extensions;
using Osirion.Blazor.Cms.Web.Middleware;
using Osirion.Blazor.Cms.Web.DependencyInjection;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.ResponseCompression;
using Osirion.Blazor.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// SEO metadata is automatically loaded from appsettings.json (Osirion:Seo section)
builder.Services.AddOsirion(builder.Configuration);
builder.Services.AddGitHubWebhookAndPolling();
builder.Services.AddJSComponents();
builder.Services.AddOsirionCookieConsent();

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

var rewriteOptions = new RewriteOptions()
    .AddRedirectToHttpsPermanent() // never include if proxy has redirection
    .AddRedirectToNonWwwPermanent();
app.UseRewriter(rewriteOptions);

app.UseGitHubWebhook();
app.UseOsirionRobotsGenerator();
app.UseResponseCompression();
app.MapOsirionCookieConsent();


app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Osirion.Blazor.Cms.Admin._Imports).Assembly);

app.Run();
