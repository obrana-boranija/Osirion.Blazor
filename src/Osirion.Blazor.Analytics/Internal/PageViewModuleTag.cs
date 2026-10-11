namespace Osirion.Blazor.Analytics.Internal;

/// <summary>
/// Builds the external script reference for the enhanced-navigation page view module. The module is a
/// static file, so no inline script is needed and a strict Content-Security-Policy keeps working.
/// </summary>
internal static class PageViewModuleTag
{
    internal const string ModulePath = "_content/Osirion.Blazor.Analytics/js/osirion-analytics.js";

    /// <param name="provider">The provider key the module understands (<c>matomo</c> or <c>yandex</c>).</param>
    /// <param name="counterId">The provider specific identifier the module needs, when it needs one.</param>
    public static string Build(string provider, string? counterId = null)
    {
        var counterAttribute = counterId is null
            ? string.Empty
            : $" data-counter=\"{ScriptLiteral.Attribute(counterId)}\"";

        return $"<script src=\"{ModulePath}\" defer data-osirion-analytics=\"{provider}\"{counterAttribute}></script>";
    }
}
