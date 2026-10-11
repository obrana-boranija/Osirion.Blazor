using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;

namespace Osirion.Blazor.Cms.Admin.Tests.TestDoubles;

/// <summary>
/// Registers a real <see cref="LoginViewModel"/> over substituted authentication collaborators for the login
/// components. GitHub OAuth is configured; a token equal to <see cref="UserToken"/> is accepted.
/// </summary>
public abstract class LoginComponentTestContext : AdminComponentTestContext
{
    protected const string UserToken = "user-token";

    protected LoginComponentTestContext()
    {
        AuthService.SetAccessTokenAsync(UserToken).Returns(true);
        TokenStore.SaveTokenAsync(Arg.Any<string>()).Returns(true);
        var stateStorage = Substitute.For<IStateStorageService>();
        stateStorage.IsInitialized.Returns(true);

        Services.AddSingleton(AuthService);
        Services.AddSingleton(TokenProvider);
        Services.AddSingleton(TokenStore);
        Services.AddSingleton<IGitHubOAuthStateStore>(OAuthStateStore);
        Services.AddSingleton(stateStorage);
        Services.AddSingleton(Substitute.For<IContentRepositoryAdapter>());
        Services.AddSingleton(new CmsEventMediator(NullLogger<CmsEventMediator>.Instance));
        Services.AddSingleton(Options.Create(AdminOptions));
        Services.AddSingleton<ILogger<LoginViewModel>>(NullLogger<LoginViewModel>.Instance);
        Services.AddScoped<LoginViewModel>();
    }

    protected IAuthenticationService AuthService { get; } = Substitute.For<IAuthenticationService>();

    protected IGitHubTokenProvider TokenProvider { get; } = Substitute.For<IGitHubTokenProvider>();

    protected IGitHubTokenStore TokenStore { get; } = Substitute.For<IGitHubTokenStore>();

    internal InMemoryGitHubOAuthStateStore OAuthStateStore { get; } = new();

    protected CmsAdminOptions AdminOptions { get; } = new()
    {
        Authentication = new AuthenticationOptions { GitHubClientId = "client-id", GitHubClientSecret = "client-secret" }
    };
}
