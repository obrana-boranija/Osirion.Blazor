using Osirion.Blazor.Example.Components;
using Osirion.Blazor.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

builder.Services.AddGitHubCms(options =>
{
    options.Owner = "obrana-boranija";
    options.Repository = "hexavera-blog";
    options.ContentPath = "content"; // Root directory for content
    options.Branch = "master";
    //options.ApiToken = ""; // Optional
    options.CacheDurationMinutes = 30;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>();

app.Run();
