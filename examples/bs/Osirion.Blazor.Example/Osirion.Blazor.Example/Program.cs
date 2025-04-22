using Osirion.Blazor.Example.Client.Pages;
using Osirion.Blazor.Example.Components;
using Osirion.Blazor.Extensions;
using Osirion.Blazor.Theming;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add Osirion.Blazor services
builder.Services.AddOsirion(osirion => {
    osirion
        // Add content services with GitHub provider
        .UseContent(content => content.AddGitHub(options => {
            options.Owner = "obrana-boranija";
            options.Repository = "hexavera-blog";
            options.ContentPath = "";
            options.Branch = "main";
            options.ApiToken = "";
            options.CacheDurationMinutes = 30;
        }))
        // Add navigation services
        .UseNavigation(navigation => {
            navigation.UseEnhancedNavigation(options => {
                options.Behavior = Osirion.Blazor.ScrollBehavior.Smooth;
                options.PreserveScrollForSamePageNavigation = false;
            });
            navigation.AddScrollToTop(options => {
                options.Position = Osirion.Blazor.Position.BottomRight;
                options.Behavior = Osirion.Blazor.ScrollBehavior.Smooth;
                options.VisibilityThreshold = 100;
                options.CssClass = "btn btn-danger";
            });
        })
        // Add analytics services
        .UseAnalytics(analytics => {
            analytics.AddClarity(options => {
                options.SiteId = "demo-clarity-id";
                options.Enabled = false; // Disable for demo
            });
            analytics.AddMatomo(options => {
                options.SiteId = "demo-matomo-id";
                options.TrackerUrl = "https://your-matomo-url/";
                options.Enabled = false; // Disable for demo
            });
        })
        // Add theming services
        .UseTheming(theming => {
            theming.UseFramework(CssFramework.Bootstrap);
            theming.EnableDarkMode(true);
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Osirion.Blazor.Example.Client._Imports).Assembly);

app.Run();
