using Osirion.Blazor.Cms.Domain.Options;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Derives the provider id a <see cref="Providers.GitHubContentProvider"/> reports for its options.
/// </summary>
/// <remarks>
/// The webhook handler uses the same rule to find the cache updater for a push, so both must
/// stay in step.
/// </remarks>
internal static class GitHubProviderId
{
    /// <summary>
    /// Returns the configured <see cref="ContentProviderOptions.ProviderId"/>, or
    /// <c>github-{Owner}-{Repository}</c> when none is configured.
    /// </summary>
    /// <param name="options">The GitHub provider options.</param>
    /// <returns>The effective provider id.</returns>
    internal static string For(GitHubOptions options)
    {
        return options.ProviderId ?? $"github-{options.Owner}-{options.Repository}";
    }
}
