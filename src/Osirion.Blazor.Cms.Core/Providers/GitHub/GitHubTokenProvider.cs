using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Core.Providers.GitHub;

/// <summary>
/// Implementation of IGitHubTokenProvider that exchanges GitHub OAuth code for an access token
/// </summary>
public class GitHubTokenProvider : IGitHubTokenProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GitHubTokenProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the GitHubTokenProvider class
    /// </summary>
    public GitHubTokenProvider(IHttpClientFactory httpClientFactory, ILogger<GitHubTokenProvider> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<string?> ExchangeCodeForTokenAsync(string code, string clientId, string clientSecret)
    {
        if (string.IsNullOrEmpty(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        if (string.IsNullOrEmpty(clientId))
            throw new ArgumentException("Client ID cannot be empty", nameof(clientId));

        if (string.IsNullOrEmpty(clientSecret))
            throw new ArgumentException("Client secret cannot be empty", nameof(clientSecret));

        try
        {
            // Exchange code for access token
            var tokenRequest = new
            {
                client_id = clientId,
                client_secret = clientSecret,
                code
            };

            var content = new StringContent(
                JsonSerializer.Serialize(tokenRequest),
                Encoding.UTF8,
                "application/json"
            );

            // GitHub's token endpoint
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.PostAsync(
                "https://github.com/login/oauth/access_token",
                content
            );

            response.EnsureSuccessStatusCode();

            // Parse response to get access token
            var responseContent = await response.Content.ReadAsStringAsync();
            string? accessToken = null;

            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                var tokenResponse = JsonSerializer.Deserialize<GitHubTokenResponse>(responseContent);
                accessToken = tokenResponse?.AccessToken;
            }
            else
            {
                // Parse as form url encoded
                var formValues = responseContent.Split('&')
                    .Select(v => v.Split('='))
                    .ToDictionary(pair => pair[0], pair => pair.Length > 1 ? pair[1] : string.Empty);

                accessToken = formValues.ContainsKey("access_token") ? formValues["access_token"] : null;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogError("Failed to get access token from GitHub");
                return null;
            }

            return accessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exchanging code for token");
            return null;
        }
    }

    private class GitHubTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }
    }
}
