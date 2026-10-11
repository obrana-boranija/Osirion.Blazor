using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging.Abstractions;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Authentication.Services;

public class ProtectedGitHubTokenStoreTests
{
    private const string Token = "ghp_user_token_value";
    private const string StoredItemKey = "localStorage:" + ProtectedGitHubTokenStore.StorageKey;

    private readonly FakeBrowserStorageJSRuntime _jsRuntime = new();
    private readonly EphemeralDataProtectionProvider _dataProtectionProvider = new();
    private readonly TestAuthenticationStateProvider _authenticationStateProvider =
        new(TestAuthenticationStateProvider.CreateUser("alice-id", "alice"));

    [Fact]
    public async Task SaveTokenAsync_ThenGetTokenAsync_ForSameUser_ReturnsToken()
    {
        var store = CreateStore();

        var saved = await store.SaveTokenAsync(Token);
        var restored = await store.GetTokenAsync();

        saved.ShouldBeTrue();
        restored.ShouldBe(Token);
    }

    [Fact]
    public async Task SaveTokenAsync_WritesOnlyEncryptedValueToBrowser()
    {
        // Proves the browser never holds the token in plain text.
        var store = CreateStore();

        await store.SaveTokenAsync(Token);

        var storedValue = _jsRuntime.Items.ShouldHaveSingleItem();
        storedValue.Key.ShouldBe(StoredItemKey);
        storedValue.Value.ShouldNotContain(Token);
    }

    [Fact]
    public async Task GetTokenAsync_ValueStoredForAnotherUser_ReturnsNullAndRemovesIt()
    {
        // Proves the purpose is bound to the host user: a value written for alice cannot sign in bob.
        var store = CreateStore();
        await store.SaveTokenAsync(Token);
        _authenticationStateProvider.User = TestAuthenticationStateProvider.CreateUser("bob-id", "bob");

        var restored = await store.GetTokenAsync();

        restored.ShouldBeNull();
        _jsRuntime.Items.ShouldNotContainKey(StoredItemKey);
    }

    [Fact]
    public async Task GetTokenAsync_AfterKeyRingChange_ReturnsNullAndRemovesIt()
    {
        // Proves a value that cannot be decrypted any more is treated as signed out.
        await CreateStore().SaveTokenAsync(Token);
        var storeWithNewKeyRing = CreateStore(new EphemeralDataProtectionProvider());

        var restored = await storeWithNewKeyRing.GetTokenAsync();

        restored.ShouldBeNull();
        _jsRuntime.Items.ShouldNotContainKey(StoredItemKey);
    }

    [Fact]
    public async Task GetTokenAsync_WithTamperedValue_ReturnsNull()
    {
        var store = CreateStore();
        _jsRuntime.Items[StoredItemKey] = "not-a-protected-value";

        var restored = await store.GetTokenAsync();

        restored.ShouldBeNull();
    }

    [Fact]
    public async Task SaveTokenAsync_AnonymousUser_StoresNothing()
    {
        _authenticationStateProvider.User = TestAuthenticationStateProvider.CreateAnonymousUser();
        var store = CreateStore();

        var saved = await store.SaveTokenAsync(Token);

        saved.ShouldBeFalse();
        _jsRuntime.Items.ShouldBeEmpty();
    }

    [Fact]
    public async Task SaveTokenAsync_UserWithoutIdentifierOrName_StoresNothing()
    {
        _authenticationStateProvider.User = TestAuthenticationStateProvider.CreateUser(nameIdentifier: null, name: null);
        var store = CreateStore();

        var saved = await store.SaveTokenAsync(Token);

        saved.ShouldBeFalse();
        _jsRuntime.Items.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetTokenAsync_AnonymousUser_ReturnsNull()
    {
        var store = CreateStore();
        await store.SaveTokenAsync(Token);
        _authenticationStateProvider.User = TestAuthenticationStateProvider.CreateAnonymousUser();

        var restored = await store.GetTokenAsync();

        restored.ShouldBeNull();
    }

    [Fact]
    public async Task RemoveTokenAsync_RemovesStoredValue()
    {
        var store = CreateStore();
        await store.SaveTokenAsync(Token);

        await store.RemoveTokenAsync();

        _jsRuntime.Items.ShouldBeEmpty();
    }

    [Fact]
    public void GetUserId_PrefersNameIdentifierOverName()
    {
        var userId = ProtectedGitHubTokenStore.GetUserId(TestAuthenticationStateProvider.CreateUser("alice-id", "alice"));

        userId.ShouldBe("alice-id");
    }

    [Fact]
    public void GetUserId_WithoutNameIdentifier_FallsBackToName()
    {
        var userId = ProtectedGitHubTokenStore.GetUserId(TestAuthenticationStateProvider.CreateUser(nameIdentifier: null, name: "alice"));

        userId.ShouldBe("alice");
    }

    private ProtectedGitHubTokenStore CreateStore(IDataProtectionProvider? dataProtectionProvider = null)
    {
        return new ProtectedGitHubTokenStore(
            new ProtectedLocalStorage(_jsRuntime, dataProtectionProvider ?? _dataProtectionProvider),
            _authenticationStateProvider,
            NullLogger<ProtectedGitHubTokenStore>.Instance);
    }
}
