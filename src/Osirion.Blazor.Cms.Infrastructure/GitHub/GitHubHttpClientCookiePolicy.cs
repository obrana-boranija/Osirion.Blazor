using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Turns off cookie handling on the primary handler of every <see cref="IHttpClientFactory"/> client that
/// talks to GitHub.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="IHttpClientFactory"/> pools handlers per client name, and a pooled handler shares one cookie
/// container across every request it serves. Without this policy a cookie GitHub sets during one user's
/// request would be replayed on another user's request.
/// </para>
/// <para>
/// The policy runs as a post-configuration, after every handler configuration of the host, whether the host
/// registered it before or after the Osirion services. It changes the primary handler that is in place at that
/// point, so a primary handler configured by the host (for example a proxy) is kept, with cookies turned off.
/// In the browser the fetch API owns cookies and the handler has no cookie container, so the policy does nothing
/// there.
/// </para>
/// </remarks>
internal sealed class GitHubHttpClientCookiePolicy : IPostConfigureOptions<HttpClientFactoryOptions>
{
    private static readonly HashSet<string> ClientNames = new(StringComparer.Ordinal)
    {
        nameof(IGitHubApiClient),
        nameof(IGitHubTokenProvider),
        nameof(IAuthenticationService)
    };

    /// <inheritdoc />
    public void PostConfigure(string? name, HttpClientFactoryOptions options)
    {
        // Reading UseCookies throws PlatformNotSupportedException in the browser
        if (OperatingSystem.IsBrowser() || !AppliesTo(name))
        {
            return;
        }

        options.HttpMessageHandlerBuilderActions.Add(builder => DisableCookies(builder.PrimaryHandler));
    }

    /// <summary>
    /// Returns whether the named client calls GitHub: the per-provider clients created by
    /// <see cref="GitHubApiClientFactory"/>, and the typed or named clients of the GitHub services.
    /// </summary>
    internal static bool AppliesTo(string? name)
    {
        return !string.IsNullOrEmpty(name)
            && (name.StartsWith(GitHubApiClientFactory.HttpClientNamePrefix, StringComparison.Ordinal)
                || ClientNames.Contains(name));
    }

    private static void DisableCookies(HttpMessageHandler primaryHandler)
    {
        switch (primaryHandler)
        {
            case HttpClientHandler { UseCookies: true } httpClientHandler:
                httpClientHandler.UseCookies = false;
                break;
            case SocketsHttpHandler { UseCookies: true } socketsHandler:
                socketsHandler.UseCookies = false;
                break;
        }
    }
}
