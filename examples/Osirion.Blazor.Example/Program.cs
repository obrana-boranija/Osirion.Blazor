using Osirion.Blazor.Example.Components;
using Osirion.Blazor.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

builder.Services.AddOsirionBlazor(osirion => {
    osirion.AddAllServices(builder.Configuration);
});

//builder.Services.AddScrollToTop(builder.Configuration);

//builder.Services.AddScrollToTop(options =>
//{
//    options.Position = ButtonPosition.BottomLeft;
//    options.Title = "Back to Top";
//    options.Text = "Top";
//    options.Behavior = ScrollBehavior.Smooth;
//    options.VisibilityThreshold = 400;
//    options.UseStyles = true;
//    options.CustomVariables = "--osirion-scroll-background: #007bff;";
//});

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
