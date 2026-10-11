using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>Exchanges GitHub OAuth authorization codes for access tokens.</summary>
public class GitHubTokenProvider : IGitHubTokenProvider
{
    private const string TokenEndpoint = "https://github.com/login/oauth/access_token";

    private readonly HttpClient _httpClient;
    private readonly ILogger<GitHubTokenProvider> _logger;

    /// <summary>Initializes a new instance of the <see cref="GitHubTokenProvider"/> class.</summary>
    /// <param name="httpClient">The HTTP client used to call GitHub</param>
    /// <param name="logger">The logger instance</param>
    public GitHubTokenProvider(HttpClient httpClient, ILogger<GitHubTokenProvider> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Task<string?> ExchangeCodeForTokenAsync(string code, string clientId, string clientSecret)
    {
        return ExchangeCodeForTokenAsync(code, clientId, clientSecret, redirectUri: null, codeVerifier: null, CancellationToken.None);
    }

    /// <inheritdoc/>
    public async Task<string?> ExchangeCodeForTokenAsync(
        string code,
        string clientId,
        string clientSecret,
        string? redirectUri,
        string? codeVerifier,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateTokenRequest(code, clientId, clientSecret, redirectUri, codeVerifier);
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GitHub OAuth token request returned status code {StatusCode}", (int)response.StatusCode);
                return null;
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(body);

            // GitHub reports a rejected code, verifier or redirect URI as 200 OK with an error field
            if (!string.IsNullOrWhiteSpace(tokenResponse?.Error))
            {
                _logger.LogWarning("GitHub OAuth token request was rejected with error {Error}", tokenResponse.Error);
                return null;
            }

            if (string.IsNullOrWhiteSpace(tokenResponse?.AccessToken))
            {
                _logger.LogWarning("GitHub OAuth response did not contain access_token");
                return null;
            }

            return tokenResponse.AccessToken;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exchanging GitHub OAuth code for token");
            return null;
        }
    }

    private static HttpRequestMessage CreateTokenRequest(
        string code,
        string clientId,
        string clientSecret,
        string? redirectUri,
        string? codeVerifier)
    {
        var form = new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["code"] = code,
            ["grant_type"] = "authorization_code"
        };

        if (!string.IsNullOrWhiteSpace(redirectUri))
        {
            form["redirect_uri"] = redirectUri;
        }

        if (!string.IsNullOrWhiteSpace(codeVerifier))
        {
            form["code_verifier"] = codeVerifier;
        }

        var request = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(form)
        };

        // Without this header GitHub answers with a form-encoded body
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return request;
    }

    /// <summary>
    /// The JSON body of GitHub's access token response
    /// </summary>
    private sealed class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
