namespace Osirion.Blazor.Cms.Infrastructure.Markdown
{
    public interface IMarkdownProcessor
    {
        Task<string> ConvertHtmlToMarkdownAsync(string html);
        Dictionary<string, string> ExtractFrontMatter(string content);
        string RenderToHtml(string markdown, bool sanitizeHtml = true);
        Task<string> RenderToHtmlAsync(string markdown, bool sanitizeHtml = true);
        string SanitizeMarkdown(string markdown);
    }
}