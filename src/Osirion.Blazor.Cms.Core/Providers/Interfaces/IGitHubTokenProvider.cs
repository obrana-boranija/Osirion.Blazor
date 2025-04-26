namespace Osirion.Blazor.Cms.Core.Providers.Interfaces;

/// <summary>
/// Provides functionality to exchange GitHub OAuth code for an access token
/// </summary>
public interface IGitHubTokenProvider
{
    /// <summary>
    /// Exchanges an OAuth code for an access token
    /// </summary>
    /// <param name="code">The OAuth code to exchange</param>
    /// <param name="clientId">The GitHub client ID</param>
    /// <param name="clientSecret">The GitHub client secret</param>
    /// <returns>The access token, or null if the exchange fails</returns>
    Task<string?> ExchangeCodeForTokenAsync(string code, string clientId, string clientSecret);
}