using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Turns off cookie handling and shortens the default timeout of every <see cref="IHttpClientFactory"/> client that
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
        if (!AppliesTo(name))
        {
            return;
        }

        options.HttpClientActions.Add(ApplyDefaultTimeout);

        // Reading UseCookies throws PlatformNotSupportedException in the browser
        if (OperatingSystem.IsBrowser())
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

    /// <summary>Timeout applied to GitHub clients whose timeout the host left at the framework default.</summary>
    internal static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(30);

    // The framework default is 100 seconds, far too long for a refresh that makes many requests in sequence.
    // A timeout the host set itself is kept.
    private static void ApplyDefaultTimeout(HttpClient client)
    {
        if (client.Timeout == TimeSpan.FromSeconds(100))
        {
            client.Timeout = DefaultRequestTimeout;
        }
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
