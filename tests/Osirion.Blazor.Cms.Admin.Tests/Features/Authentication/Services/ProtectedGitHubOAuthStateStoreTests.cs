using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging.Abstractions;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Authentication.Services;

public class ProtectedGitHubOAuthStateStoreTests
{
    private const string StoredItemKey = "sessionStorage:" + ProtectedGitHubOAuthStateStore.StorageKey;

    private readonly FakeBrowserStorageJSRuntime _jsRuntime = new();
    private readonly EphemeralDataProtectionProvider _dataProtectionProvider = new();
    private readonly GitHubOAuthRequest _request = new("state", "verifier", "https://admin.example/osirion/login", "/osirion");

    [Fact]
    public async Task TakeAsync_AfterSaveAsync_ReturnsRequestOnlyOnce()
    {
        // Proves the pending login is single use, so a replayed callback finds no state to match.
        var store = CreateStore();
        await store.SaveAsync(_request);

        var first = await store.TakeAsync();
        var second = await store.TakeAsync();

        first.ShouldBe(_request);
        second.ShouldBeNull();
        _jsRuntime.Items.ShouldBeEmpty();
    }

    [Fact]
    public async Task SaveAsync_KeepsRequestInSessionStorageEncrypted()
    {
        var store = CreateStore();

        await store.SaveAsync(_request);

        var storedValue = _jsRuntime.Items.ShouldHaveSingleItem();
        storedValue.Key.ShouldBe(StoredItemKey);
        storedValue.Value.ShouldNotContain("verifier");
    }

    [Fact]
    public async Task TakeAsync_ValueFromAnotherKeyRing_ReturnsNullAndRemovesIt()
    {
        await CreateStore().SaveAsync(_request);

        var restored = await CreateStore(new EphemeralDataProtectionProvider()).TakeAsync();

        restored.ShouldBeNull();
        _jsRuntime.Items.ShouldBeEmpty();
    }

    private ProtectedGitHubOAuthStateStore CreateStore(IDataProtectionProvider? dataProtectionProvider = null)
    {
        return new ProtectedGitHubOAuthStateStore(
            new ProtectedSessionStorage(_jsRuntime, dataProtectionProvider ?? _dataProtectionProvider),
            NullLogger<ProtectedGitHubOAuthStateStore>.Instance);
    }
}
