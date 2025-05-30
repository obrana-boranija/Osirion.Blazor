using Osirion.Blazor.Cms.Infrastructure.Extensions;
using Osirion.Blazor.Example.Bootstrap.Components;
using Osirion.Blazor.Extensions;
using Osirion.Blazor.Cms.Web.Middleware;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOsirion(builder.Configuration);
builder.Services.AddGitHubWebhookAndPolling();
builder.Services.AddJSComponents();

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


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Osirion.Blazor.Cms.Admin._Imports).Assembly);

app.Run();
