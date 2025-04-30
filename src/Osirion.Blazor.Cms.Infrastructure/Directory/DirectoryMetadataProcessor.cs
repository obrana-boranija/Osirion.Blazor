using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using System.Text;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Infrastructure.Directory;

/// <summary>
/// Implementation of IDirectoryMetadataProcessor for processing directory metadata
/// </summary>
public class DirectoryMetadataProcessor : IDirectoryMetadataProcessor
{
    private readonly IFrontMatterExtractor _frontMatterExtractor;
    private readonly ILogger<DirectoryMetadataProcessor> _logger;

    public DirectoryMetadataProcessor(
        IFrontMatterExtractor frontMatterExtractor,
        ILogger<DirectoryMetadataProcessor> logger)
    {
        _frontMatterExtractor = frontMatterExtractor ?? throw new ArgumentNullException(nameof(frontMatterExtractor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public DirectoryItem ProcessMetadata(DirectoryItem directory, string metadataContent)
    {
        if (directory == null)
            throw new ArgumentNullException(nameof(directory));

        if (string.IsNullOrWhiteSpace(metadataContent))
            return directory;

        try
        {
            // Extract front matter
            var frontMatter = _frontMatterExtractor.ExtractFrontMatter(metadataContent);

            // Apply metadata to directory
            foreach (var kvp in frontMatter)
            {
                var key = kvp.Key.ToLowerInvariant();
                var value = kvp.Value;

                switch (key)
                {
                    case "title":
                        directory.SetName(value);
                        break;
                    case "description":
                        directory.SetDescription(value);
                        break;
                    case "order":
                        if (int.TryParse(value, out var order))
                            directory.SetOrder(order);
                        break;
                    case "locale":
                        directory.SetLocale(value);
                        break;
                    case "url":
                        directory.SetUrl(value);
                        break;
                    default:
                        // Add as custom metadata
                        if (bool.TryParse(value, out var boolVal))
                            directory.SetMetadata(key, boolVal);
                        else if (int.TryParse(value, out var intVal))
                            directory.SetMetadata(key, intVal);
                        else if (double.TryParse(value, out var doubleVal))
                            directory.SetMetadata(key, doubleVal);
                        else
                            directory.SetMetadata(key, value);
                        break;
                }
            }

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
        if (directory == null)
            throw new ArgumentNullException(nameof(directory));

        var frontMatter = new StringBuilder();
        frontMatter.AppendLine("---");

        // Add metadata
        frontMatter.AppendLine($"title: \"{EscapeYamlString(directory.Name)}\"");

        if (!string.IsNullOrEmpty(directory.Description))
            frontMatter.AppendLine($"description: \"{EscapeYamlString(directory.Description)}\"");

        if (directory.Order != 0)
            frontMatter.AppendLine($"order: {directory.Order}");

        if (!string.IsNullOrEmpty(directory.Locale))
            frontMatter.AppendLine($"locale: \"{directory.Locale}\"");

        if (!string.IsNullOrEmpty(directory.Url))
            frontMatter.AppendLine($"url: \"{directory.Url}\"");

        // Add custom metadata
        foreach (var meta in directory.Metadata)
        {
            if (meta.Value is string strValue)
                frontMatter.AppendLine($"{meta.Key}: \"{EscapeYamlString(strValue)}\"");
            else if (meta.Value is bool boolValue)
                frontMatter.AppendLine($"{meta.Key}: {boolValue.ToString().ToLowerInvariant()}");
            else if (meta.Value is int intValue)
                frontMatter.AppendLine($"{meta.Key}: {intValue}");
            else if (meta.Value is double doubleValue)
                frontMatter.AppendLine($"{meta.Key}: {doubleValue}");
            else
                frontMatter.AppendLine($"{meta.Key}: \"{meta.Value}\"");
        }

        frontMatter.AppendLine("---");
        frontMatter.AppendLine();
        frontMatter.AppendLine($"# {directory.Name}");
        frontMatter.AppendLine();

        if (!string.IsNullOrEmpty(directory.Description))
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