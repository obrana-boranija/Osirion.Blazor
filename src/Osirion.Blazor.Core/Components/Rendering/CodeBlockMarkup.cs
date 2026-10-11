namespace Osirion.Blazor.Components;

/// <summary>
/// Builds the header and copy button markup around a code block, shared by <see cref="OsirionHtmlRenderer"/> and
/// <see cref="OsirionContentDemo"/>.
/// </summary>
internal static class CodeBlockMarkup
{
    /// <summary>Wraps <paramref name="codeBlock"/>, which must already be safe markup, with a language label and a
    /// copy button.</summary>
    public static string Wrap(string codeBlock, string language, string languageLabel, string buttonClass) => $@"
            <div class=""osirion-code-wrapper"" data-language=""{language}"">
                <div class=""osirion-code-header"">
                    <span class=""osirion-language-label"">{languageLabel}</span>
                    <button class=""{buttonClass} osirion-copy-button"" aria-label=""Copy {languageLabel} code to clipboard"" onclick=""osirionCopyCode(this)"">
                        <span class=""osirion-copy-text"">Copy</span>
                    </button>
                </div>
                {codeBlock}
            </div>";
}
