using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Web.Tests.Base;

/// <summary>A navigation manager that counts <see cref="NavigationManager.NotFound"/> calls.</summary>
internal sealed class RecordingNavigationManager : NavigationManager
{
    public RecordingNavigationManager()
    {
        Initialize("http://localhost/", "http://localhost/blog/page");
        OnNotFound += (_, _) => NotFoundCalls++;
    }

    public int NotFoundCalls { get; private set; }

    protected override void NavigateToCore(string uri, NavigationOptions options) => Uri = ToAbsoluteUri(uri).ToString();
}
