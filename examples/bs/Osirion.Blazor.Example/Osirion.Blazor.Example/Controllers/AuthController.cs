using Microsoft.AspNetCore.Mvc;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Example.Controllers;

/// <summary>
/// Controller for handling GitHub OAuth callbacks
/// </summary>
[ApiController]
[Route("admin/auth")]
public class AuthController : ControllerBase
{
    private readonly IGitHubTokenProvider _tokenProvider;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IGitHubTokenProvider tokenProvider,
        ILogger<AuthController> logger)
    {
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GitHub OAuth callback
    /// </summary>
    [HttpGet("callback")]
    public IActionResult Callback([FromQuery] string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            _logger.LogWarning("OAuth callback received with no code");
            return BadRequest("No authorization code provided");
        }

        // Redirect to the login page with the code
        // The Blazor app will extract the code and use it to complete the auth process
        return Redirect($"/admin/login?code={code}");
    }
}