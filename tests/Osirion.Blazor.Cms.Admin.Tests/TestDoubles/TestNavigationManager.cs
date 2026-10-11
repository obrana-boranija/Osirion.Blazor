using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Admin.Tests.TestDoubles;

/// <summary>
/// A navigation manager for <c>https://admin.example/</c> that records every navigation instead of performing it.
/// </summary>
internal sealed class TestNavigationManager : NavigationManager
{
    public const string SiteBaseUri = "https://admin.example/";

    public TestNavigationManager(string relativeUri = "osirion/login")
    {
        Initialize(SiteBaseUri, SiteBaseUri + relativeUri);
    }

    public List<(string Uri, bool ForceLoad)> Navigations { get; } = [];

    protected override void NavigateToCore(string uri, NavigationOptions options)
    {
        Navigations.Add((uri, options.ForceLoad));
        Uri = ToAbsoluteUri(uri).ToString();
    }
}
