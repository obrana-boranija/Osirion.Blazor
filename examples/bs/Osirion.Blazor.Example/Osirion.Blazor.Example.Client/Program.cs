using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Osirion.Blazor.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddOsirion();

await builder.Build().RunAsync();
