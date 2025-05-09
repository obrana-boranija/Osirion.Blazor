using Osirion.Blazor.Cms.Admin.DependencyInjection;
using Osirion.Blazor.Example.Components;
using Osirion.Blazor.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpContextAccessor();

builder.Services.AddOsirion(builder.Configuration);

// In Program.cs
builder.Services.AddOsirionCmsAdmin(options =>
{
    // Configure GitHub provider
    options.UseGitHubProvider(github =>
    {
        github.Owner = "obrana-boranija";
        github.Repository = "Osirion.Blazor";
        github.DefaultBranch = "master"; // Optional, defaults to "main"
        github.ContentPath = ""; // Optional subdirectory in the repository
        github.CommitterName = "Your Name"; // Optional
        github.CommitterEmail = "your.email@example.com"; // Optional
    });

    // Configure authentication
    options.ConfigureAuthentication(auth =>
    {
        auth.EnableGitHubOAuth = true;
        auth.GitHubClientId = "Ov23lid1Y6zZLKbwaaih";
        auth.GitHubClientSecret = "fef6f7e1dac44e185df867fb9ddd80daf58705b8";
        auth.GitHubRedirectUri = "https://your-site.com/osirion/login";
        auth.AllowPersonalAccessTokens = true; // Allow PAT authentication
        auth.PersonalAccessToken = "";
    });

    // Configure UI theme
    options.ConfigureTheme(theme =>
    {
        theme.DefaultMode = "light"; // "light" or "dark"
        theme.PrimaryColor = "#2563eb"; // Primary brand color
        theme.UseDarkMode = false; // Enable dark mode by default
        theme.RespectSystemPreferences = true; // Follow system preferences
    });

    // Configure content rules
    options.ConfigureContentRules(rules =>
    {
        rules.RequireApproval = false; // Require content approval
        rules.MaximumDraftAge = 30; // Auto-delete drafts after 30 days
        rules.EnforceFrontMatterValidation = true; // Validate front matter
        rules.RequiredFrontMatterFields = new List<string> { "title", "date" };
        rules.AutoGenerateSlugs = true; // Auto-generate slugs from titles
        rules.AllowedFileExtensions = new List<string> { ".md", ".markdown" };
        rules.AllowFileDeletion = true; // Allow deleting files
    });

    // Configure localization (optional)
    options.UseLocalization(localization =>
    {
        localization.AddSupportedCultures("en-US", "fr-FR", "de-DE");
        localization.SetDefaultCulture("en-US");
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

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies([ typeof(Osirion.Blazor.Example.Client._Imports).Assembly, typeof(Osirion.Blazor.Cms.Admin.OsirionCmsAdminExtensions).Assembly ]);

app.Run();
