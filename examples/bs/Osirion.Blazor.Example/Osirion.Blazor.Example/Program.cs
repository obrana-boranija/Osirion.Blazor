using Osirion.Blazor.Cms;
using Osirion.Blazor.Example.Components;
using Osirion.Blazor.Extensions;
using Osirion.Blazor.Theming;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpContextAccessor();

builder.Services.AddOsirionCms(builder.Configuration);

// Add admin features if needed
//builder.Services.AddOsirionCmsAdmin(builder.Configuration);

// Add Osirion.Blazor services
builder.Services.AddOsirion(osirion =>
{
    osirion
        // Add content services with GitHub provider
        //.UseContent(content => content.AddGitHub(options =>
        //{
        //    options.Owner = "obrana-boranija";
        //    options.Repository = "hexavera-blog";
        //    options.ContentPath = "localized";
        //    options.Branch = "main";
        //    options.ApiToken = "";
        //    options.CacheDurationMinutes = 30;
        //    options.EnableLocalization = true;
        //    options.DefaultLocale = "en";
        //}))
        //Configure CMS Admin
        //.UseCmsAdmin(admin =>
        //{
        //    admin.Configure(options =>
        //    {
        //        options.Owner = "obrana-boranija";
        //        options.DefaultRepository = "hexavera-blog";
        //        options.DefaultBranch = "master";
        //    });

        //    admin.UseGitHubAuthentication(
        //        clientId: "your-github-oauth-client-id",
        //        clientSecret: "your-github-oauth-client-secret"
        //    );
        //})
        // Add navigation services
        .UseNavigation(navigation =>
        {
            navigation.UseEnhancedNavigation(options =>
            {
                options.Behavior = Osirion.Blazor.ScrollBehavior.Smooth;
                options.PreserveScrollForSamePageNavigation = false;
            });
            navigation.AddScrollToTop(options =>
            {
                options.Position = Osirion.Blazor.Position.BottomRight;
                options.Behavior = Osirion.Blazor.ScrollBehavior.Smooth;
                options.VisibilityThreshold = 100;
                //options.CssClass = "btn btn-danger";
            });
        })
        // Add analytics services
        .UseAnalytics(analytics =>
        {
            analytics.AddClarity(options =>
            {
                options.SiteId = "demo-clarity-id";
                options.Enabled = false; // Disable for demo
            });
            analytics.AddMatomo(options =>
            {
                options.SiteId = "demo-matomo-id";
                options.TrackerUrl = "https://your-matomo-url/";
                options.Enabled = false; // Disable for demo
            });
        })
        // Add theming services
        .UseTheming(theming =>
        {
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
