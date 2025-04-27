using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

public class GitHubTokenProvider : IGitHubTokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GitHubTokenProvider> _logger;

    public GitHubTokenProvider(HttpClient httpClient, ILogger<GitHubTokenProvider> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string?> ExchangeCodeForTokenAsync(string code, string clientId, string clientSecret)
    {
        try
        {
            // Create request data
            var requestData = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["code"] = code,
                ["redirect_uri"] = "https://localhost:5001/auth/callback", // This should come from config
                ["grant_type"] = "authorization_code"
            };

            // Create request content
            var content = new FormUrlEncodedContent(requestData);

            // Send token request
            var response = await _httpClient.PostAsync("https://github.com/login/oauth/access_token", content);
            response.EnsureSuccessStatusCode();

            // Parse response
            var responseContent = await response.Content.ReadAsStringAsync();

            // Parse the response which is in form-encoded format
            var responseParams = responseContent.Split('&')
                .Select(param => param.Split('='))
                .ToDictionary(param => param[0], param => param[1]);

            if (responseParams.TryGetValue("access_token", out var token))
            {
                return token;
            }

            _logger.LogWarning("GitHub OAuth response did not contain access_token");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exchanging GitHub OAuth code for token");
            return null;
        }
    }
}