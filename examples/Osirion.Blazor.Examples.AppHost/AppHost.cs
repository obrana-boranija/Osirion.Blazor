// Runs every example site side by side, so a library change can be checked against each CSS framework at once.
var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Osirion_Blazor_Example_Bootstrap>("bootstrap");
builder.AddProject<Projects.Osirion_Blazor_Example_Mud>("mudblazor");
builder.AddProject<Projects.Osirion_Blazor_Example_FluentUI>("fluentui");

builder.Build().Run();
