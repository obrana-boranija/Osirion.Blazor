using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Authentication.Services;

public class GitHubOAuthTests
{
    [Fact]
    public void CreateRandomValue_ReturnsDistinct43CharacterBase64UrlValues()
    {
        var first = GitHubOAuth.CreateRandomValue();
        var second = GitHubOAuth.CreateRandomValue();

        first.Length.ShouldBe(43);
        first.ShouldMatch("^[A-Za-z0-9_-]{43}$");
        second.ShouldNotBe(first);
    }

    [Fact]
    public void CreateCodeChallenge_WithRfc7636Example_ReturnsS256Challenge()
    {
        // The verifier and challenge of RFC 7636 appendix B
        var challenge = GitHubOAuth.CreateCodeChallenge("dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk");

        challenge.ShouldBe("E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM");
    }

    [Theory]
    [InlineData("state-value", "state-value", true)]
    [InlineData("state-value", "state-valuE", false)]
    [InlineData("state-value", "state-value-longer", false)]
    [InlineData("state-value", "", false)]
    [InlineData("state-value", null, false)]
    [InlineData("", "", false)]
    public void StatesMatch_ComparesExactValues(string expected, string? actual, bool matches)
    {
        GitHubOAuth.StatesMatch(expected, actual).ShouldBe(matches);
    }

    [Fact]
    public void BuildAuthorizeUrl_EscapesValuesAndKeepsRepoScope()
    {
        var request = new GitHubOAuthRequest("st&ate", "verifier", "https://cms.example/osirion/login?x=1", "/osirion");

        var url = GitHubOAuth.BuildAuthorizeUrl("client id", request);

        url.ShouldBe(
            "https://github.com/login/oauth/authorize" +
            "?client_id=client%20id" +
            "&redirect_uri=https%3A%2F%2Fcms.example%2Fosirion%2Flogin%3Fx%3D1" +
            "&scope=repo" +
            "&state=st%26ate" +
            $"&code_challenge={GitHubOAuth.CreateCodeChallenge("verifier")}" +
            "&code_challenge_method=S256");
    }
}
