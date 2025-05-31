using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;

public partial class AdvancedMetadataForm
{
    [Parameter]
    public FrontMatter Metadata { get; set; } = FrontMatter.Create("New Post");

    [Parameter]
    public EventCallback<FrontMatter> MetadataChanged { get; set; }

    [Parameter]
    public SeoMetadata SeoMetadata { get; set; } = SeoMetadata.Create("", "");

    [Parameter]
    public EventCallback<SeoMetadata> SeoMetadataChanged { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = null!;

    private List<CustomField> CustomFieldsList { get; set; } = new();

    protected override void OnParametersSet()
    {
        Metadata ??= FrontMatter.Create("New Post");
        SeoMetadata ??= SeoMetadata.Create("", "");

        // Convert dictionary to list for easier manipulation
        CustomFieldsList = Metadata.CustomFields
            .Select(kvp => new CustomField { Key = kvp.Key, Value = kvp.Value?.ToString() ?? "" })
            .ToList();
    }

    private async Task NotifyMetadataChanged()
    {
        if (MetadataChanged.HasDelegate)
        {
            await MetadataChanged.InvokeAsync(Metadata);
        }
    }

    private async Task GenerateSlug()
    {
        if (!string.IsNullOrWhiteSpace(Metadata.Title))
        {
            var slug = Metadata.Title.ToLower()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace("&", "and");

            // Remove all non-alphanumeric characters except hyphens
            slug = Regex.Replace(slug, @"[^a-z0-9-]", "");

            // Remove multiple consecutive hyphens
            slug = Regex.Replace(slug, @"-+", "-");

            // Trim hyphens from start and end
            slug = slug.Trim('-');

            Metadata = Metadata.WithSlug(slug);
            await NotifyMetadataChanged();
        }
    }

    private async Task UpdateCustomFields()
    {
        var customFields = new Dictionary<string, object>();

        foreach (var field in CustomFieldsList.Where(f => !string.IsNullOrWhiteSpace(f.Key)))
        {
            customFields[field.Key] = field.Value ?? "";
        }

        Metadata = Metadata.WithCustomFields(customFields);
        await NotifyMetadataChanged();
    }

    private void AddCustomField()
    {
        CustomFieldsList.Add(new CustomField());
    }

    private async Task RemoveCustomField(CustomField field)
    {
        CustomFieldsList.Remove(field);
        await UpdateCustomFields();
    }

    private async Task ExportAsYaml()
    {
        var yaml = Metadata.ToYaml();
        await DownloadFile("frontmatter.yaml", yaml, "text/yaml");
    }

    private async Task ExportAsJson()
    {
        var json = JsonSerializer.Serialize(new
        {
            frontMatter = Metadata,
            seoMetadata = SeoMetadata
        }, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await DownloadFile("metadata.json", json, "application/json");
    }

    private async Task DownloadFile(string filename, string content, string mimeType)
    {
        // Since we're in server interactive mode, we can use JSInterop
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var base64 = Convert.ToBase64String(bytes);

        await JSRuntime.InvokeVoidAsync("downloadFile", filename, base64, mimeType);
    }

    private class CustomField
    {
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
    }
}