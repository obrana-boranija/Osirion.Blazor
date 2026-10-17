using Osirion.Blazor.Cms.Domain.Options;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// A GitHub provider registered under <c>Osirion:Cms:Web:GitHub:&lt;Name&gt;</c> or
/// <c>Osirion:Cms:Admin:GitHub:&lt;Name&gt;</c>, with the options that decide how <see cref="GitHubPollingService"/> polls it.
/// </summary>
/// <param name="ProviderName">The name of the provider section, which the API client factory knows the provider by.</param>
/// <param name="Options">The options bound from that provider's own section.</param>
public sealed record GitHubPollingTarget(string ProviderName, GitHubOptions Options);
