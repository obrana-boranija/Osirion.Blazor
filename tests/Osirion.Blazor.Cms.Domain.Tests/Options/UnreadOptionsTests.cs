using System.Reflection;
using Osirion.Blazor.Cms.Domain.Options;

#pragma warning disable CS0618 // The options under test are obsolete and keep working until they are removed.

namespace Osirion.Blazor.Cms.Domain.Tests.Domain.Options;

/// <summary>
/// Options that configuration accepts and no code reads must say so, with what to use instead.
/// </summary>
public class UnreadOptionsTests
{
    public static TheoryData<Type, string, string> ObsoleteMembers => new()
    {
        { typeof(FileSystemOptions), nameof(FileSystemOptions.PollingIntervalMs), "WatchForChanges" },
        { typeof(FileSystemOptions), nameof(FileSystemOptions.IncludePatterns), "SupportedExtensions" },
        { typeof(GitHubOptions), nameof(GitHubOptions.ExcludePatterns), "SupportedExtensions" }
    };

    [Theory]
    [MemberData(nameof(ObsoleteMembers))]
    public void UnreadOption_IsObsoleteAndNamesWhatToUseInstead(Type optionsType, string propertyName, string replacement)
    {
        // Fails before the fix: the property carried no Obsolete attribute, so configuring it looked supported.
        var attribute = optionsType.GetProperty(propertyName)!.GetCustomAttribute<ObsoleteAttribute>();

        Assert.NotNull(attribute);
        Assert.Contains(replacement, attribute.Message);
    }

    [Fact]
    public void CacheOptions_IsObsoleteAndSaysTheProvidersCacheThemselves()
    {
        // Fails before the fix: nothing told a host that Osirion:Cms:Cache does not govern the providers' caches.
        var attribute = typeof(CacheOptions).GetCustomAttribute<ObsoleteAttribute>();

        Assert.NotNull(attribute);
        Assert.Contains("Osirion:Cms:Cache", attribute.Message);
    }
}
