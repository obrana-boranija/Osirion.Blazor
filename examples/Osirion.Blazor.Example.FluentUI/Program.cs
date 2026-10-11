using Microsoft.AspNetCore.Rewrite;
using Microsoft.FluentUI.AspNetCore.Components;
using Osirion.Blazor.Cms.Web.Middleware;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Example.FluentUI.Components;
using Osirion.Blazor.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluentUIComponents();

// Content, theming (Framework: FluentUI) and SEO settings come from the Osirion section of appsettings.json.
builder.Services.AddOsirion(builder.Configuration);
builder.Services.AddJSComponents();
builder.Services.AddOsirionCookieConsent();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();

    // Canonical scheme redirects belong to deployed sites; local runs work over plain HTTP.
    app.UseRewriter(new RewriteOptions().AddRedirectToHttpsPermanent());
}

app.UseOsirionRobotsGenerator();
app.MapOsirionCookieConsent();

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
