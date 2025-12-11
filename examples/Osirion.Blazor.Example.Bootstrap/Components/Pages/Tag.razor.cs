using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using System.Globalization;
using System.Linq;

namespace Osirion.Blazor.Example.Bootstrap.Components.Pages;

public partial class Tag
{
    [Inject]
    private IContentProviderManager ContentProviderManager { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public string? Locale { get; set; } = "en";

    [Parameter]
    public string? TagSlug { get; set; }

    protected string? ResolvedTagName { get; private set; }

    protected bool TagExists { get; private set; }

    private bool UseLocalePrefix { get; set; }

    protected string TagDisplayName => ResolvedTagName ?? FormatSlug(TagSlug);

    protected override async Task OnParametersSetAsync()
    {
        UseLocalePrefix = ShouldUseLocalePrefix();

        if (string.IsNullOrWhiteSpace(TagSlug))
        {
            ResolvedTagName = null;
            TagExists = false;
            return;
        }

        await ResolveTagAsync(TagSlug);
    }

    private async Task ResolveTagAsync(string slug)
    {
        ResolvedTagName = null;
        TagExists = false;

        try
        {
            var provider = ContentProviderManager.GetDefaultProvider();
            if (provider is null)
            {
                return;
            }

            var tags = await provider.GetTagsAsync();
            var match = tags.FirstOrDefault(t => t.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

            if (match is not null)
            {
                ResolvedTagName = match.Name;
                TagExists = true;
            }
        }
        catch
        {
            // Intentionally swallow exceptions to avoid breaking the page when tag lookups fail.
        }
    }

    private string FormatTagUrl(ContentTag tag)
    {
        if (UseLocalePrefix && !string.IsNullOrWhiteSpace(Locale))
        {
            return $"/{Locale}/tag/{tag.Slug}";
        }

        return $"/tag/{tag.Slug}";
    }

    private static string FormatSlug(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return string.Empty;
        }

        var value = slug.Replace('-', ' ').Replace('_', ' ');
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
    }

    private bool ShouldUseLocalePrefix()
    {
        if (string.IsNullOrWhiteSpace(Locale))
        {
            return false;
        }

        var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        return relativePath.StartsWith($"{Locale}/", StringComparison.OrdinalIgnoreCase);
    }
}
