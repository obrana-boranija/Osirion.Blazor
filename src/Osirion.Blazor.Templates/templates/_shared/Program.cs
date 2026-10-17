using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Web.Feeds;
using Osirion.Blazor.Extensions;
using OsirionApp_CSharp.Components;

var builder = WebApplication.CreateBuilder(args);

// Static server-side rendering: every page arrives as complete HTML, readable by people, crawlers and feed readers.
builder.Services.AddRazorComponents();

// Content, theming, SEO and the site shell (menu and footer), all from the Osirion section of appsettings.json.
builder.Services.AddOsirion(builder.Configuration);

// The Markdown files live in the content folder; resolve it against the project, whatever folder the app starts from.
builder.Services.PostConfigure<FileSystemOptions>(options =>
    options.BasePath = Path.GetFullPath(options.BasePath, builder.Environment.ContentRootPath));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    app.UseHsts();
}

// An address no page claims, or a missing file, gets the not-found page with status 404.
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

// /sitemap.xml, and /feed.xml and /atom.xml unless the Feeds section turns them off, built from the same Markdown files
// as the pages.
app.MapOsirionFeeds(feeds => app.Configuration.GetSection("Feeds").Bind(feeds));

app.MapRazorComponents<App>();

await app.RunAsync();
