using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Osirion.Blazor.Cms.Infrastructure.Directory;

/// <summary>
/// Implementation of IDirectoryMetadataProcessor for processing directory metadata
/// </summary>
public class DirectoryMetadataProcessor : IDirectoryMetadataProcessor
{
    private readonly IFrontMatterExtractor _frontMatterExtractor;
    private readonly IMarkdownProcessor _markdownProcessor;
    private readonly ILogger<DirectoryMetadataProcessor> _logger;

    public DirectoryMetadataProcessor(
        IFrontMatterExtractor frontMatterExtractor,
        IMarkdownProcessor markdownProcessor,
        ILogger<DirectoryMetadataProcessor> logger)
    {
        _frontMatterExtractor = frontMatterExtractor;
        _markdownProcessor = markdownProcessor;
        _logger = logger;
    }

    /// <inheritdoc/>
    public DirectoryItem ProcessMetadata(DirectoryItem directory, string metadataContent)
    {
        if (directory is null)
            throw new ArgumentNullException(nameof(directory));

        if (string.IsNullOrWhiteSpace(metadataContent))
            return directory;

        try
        {
            var frontmatter = _markdownProcessor.ExtractFrontMatterAndContent(metadataContent).FrontMatter;

            directory.SetMetadata(frontmatter);
            directory.SetName(frontmatter?.Title ?? directory.Name);
            directory.SetDescription(frontmatter?.Description ?? directory.Description);
            directory.SetOrder(frontmatter?.Order ?? directory.Order);
            directory.SetLocale(frontmatter?.Lang ?? directory.Locale);
            directory.SetUrl(frontmatter?.Slug ?? directory.Url);
            directory.SetFeaturedImage(frontmatter?.FeaturedImage ?? directory.FeaturedImageUrl);
            return directory;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing directory metadata for {DirectoryId}", directory.Id);
            return directory;
        }
    }

    /// <inheritdoc/>
    public string GenerateMetadataContent(DirectoryItem directory)
    {
        if (directory is null)
            throw new ArgumentNullException(nameof(directory));

        var frontMatter = new StringBuilder();
        frontMatter.AppendLine("---");

        // Add metadata
        frontMatter.AppendLine($"title: \"{EscapeYamlString(directory.Name)}\"");

        if (!string.IsNullOrWhiteSpace(directory.Description))
            frontMatter.AppendLine($"description: \"{EscapeYamlString(directory.Description)}\"");

        if (directory.Order != 0)
            frontMatter.AppendLine($"order: {directory.Order}");

        if (!string.IsNullOrWhiteSpace(directory.Locale))
            frontMatter.AppendLine($"locale: \"{directory.Locale}\"");

        if (!string.IsNullOrWhiteSpace(directory.Url))
            frontMatter.AppendLine($"url: \"{directory.Url}\"");

        // Add custom metadata
        //foreach (var meta in directory.Metadata)
        //{
        //    if (meta.Value is string strValue)
        //        frontMatter.AppendLine($"{meta.Key}: \"{EscapeYamlString(strValue)}\"");
        //    else if (meta.Value is bool boolValue)
        //        frontMatter.AppendLine($"{meta.Key}: {boolValue.ToString().ToLowerInvariant()}");
        //    else if (meta.Value is int intValue)
        //        frontMatter.AppendLine($"{meta.Key}: {intValue}");
        //    else if (meta.Value is double doubleValue)
        //        frontMatter.AppendLine($"{meta.Key}: {doubleValue}");
        //    else
        //        frontMatter.AppendLine($"{meta.Key}: \"{meta.Value}\"");
        //}

        frontMatter.AppendLine("---");
        frontMatter.AppendLine();

        frontMatter.AppendLine($"# {directory.Name}");
        frontMatter.AppendLine();

        if (!string.IsNullOrWhiteSpace(directory.Description))
        {
            frontMatter.AppendLine(directory.Description);
        }

        return frontMatter.ToString();
    }

    /// <summary>
    /// Escapes special characters in YAML strings
    /// </summary>
    private string EscapeYamlString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}