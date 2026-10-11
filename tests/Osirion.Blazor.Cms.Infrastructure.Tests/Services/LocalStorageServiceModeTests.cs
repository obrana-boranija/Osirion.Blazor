using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public sealed class LocalStorageServiceModeTests : IDisposable
{
    private readonly ILocalStorageService _localStorage = Substitute.For<ILocalStorageService>();
    private readonly LocalStorageService _service;

    public LocalStorageServiceModeTests()
    {
        _service = new LocalStorageService(_localStorage, new TestNavigationManager(), NullLogger<LocalStorageService>.Instance);
    }

    public void Dispose() => _service.Dispose();

    private void BrowserStorageUnavailable() =>
        _localStorage.SetItemAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("JavaScript interop is not available while prerendering"));

    [Fact]
    public void Constructor_WithNullDependencies_Throws()
    {
        var navigation = new TestNavigationManager();
        var logger = NullLogger<LocalStorageService>.Instance;

        Should.Throw<ArgumentNullException>(() => new LocalStorageService(null!, navigation, logger));
        Should.Throw<ArgumentNullException>(() => new LocalStorageService(_localStorage, null!, logger));
        Should.Throw<ArgumentNullException>(() => new LocalStorageService(_localStorage, navigation, null!));
    }

    [Fact]
    public async Task InitializeAsync_RunsTheBrowserProbeOnce()
    {
        await _service.InitializeAsync();
        await _service.InitializeAsync();

        await _localStorage.Received(1).SetItemAsync("__osirion_storage_test", true, Arg.Any<CancellationToken>());
        await _localStorage.Received(1).RemoveItemAsync("__osirion_storage_test", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InitializeAsync_WithoutBrowserStorage_UsesMemoryOnly()
    {
        BrowserStorageUnavailable();

        await _service.InitializeAsync();
        await _service.SaveStateAsync("key", 42);
        var value = await _service.GetStateAsync<int>("key");
        var missing = await _service.GetStateAsync<string>("missing");
        await _service.RemoveStateAsync("key");
        var removed = await _service.GetStateAsync<int?>("key");

        _service.IsInitialized.ShouldBeTrue();
        value.ShouldBe(42);
        missing.ShouldBeNull();
        removed.ShouldBeNull();
        await _localStorage.DidNotReceiveWithAnyArgs().SetItemAsStringAsync(default!, default!, default);
        await _localStorage.DidNotReceiveWithAnyArgs().ContainKeyAsync(default!, default);
        await _localStorage.DidNotReceive().RemoveItemAsync("key", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InitializeAsync_InBrowser_PreloadsOnlyNonBlankAuthenticationKeys()
    {
        _localStorage.ContainKeyAsync("github_username", Arg.Any<CancellationToken>()).Returns(true);
        _localStorage.GetItemAsStringAsync("github_username", Arg.Any<CancellationToken>()).Returns("\"octocat\"");
        _localStorage.ContainKeyAsync("auth_status", Arg.Any<CancellationToken>()).Returns(true);
        _localStorage.GetItemAsStringAsync("auth_status", Arg.Any<CancellationToken>()).Returns(" ");
        _localStorage.ContainKeyAsync("last_login_method", Arg.Any<CancellationToken>()).Returns(false);

        await _service.InitializeAsync();
        _localStorage.ClearReceivedCalls();
        var username = await _service.GetStateAsync<string>("github_username");

        username.ShouldBe("octocat");
        await _localStorage.DidNotReceiveWithAnyArgs().GetItemAsStringAsync(default!, default);
        await _localStorage.DidNotReceive().GetItemAsStringAsync("last_login_method", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InitializeAsync_WhenPreloadingFails_StaysInBrowserMode()
    {
        _localStorage.ContainKeyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Throws(new InvalidOperationException("boom"));

        await _service.InitializeAsync();
        await _service.SaveStateAsync("key", "value");

        _service.IsInitialized.ShouldBeTrue();
        await _localStorage.Received(1).SetItemAsStringAsync("key", "\"value\"", Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Operations_WithBlankKey_DoNothing(string key)
    {
        await _service.InitializeAsync();
        _localStorage.ClearReceivedCalls();

        await _service.SaveStateAsync(key, "value");
        var value = await _service.GetStateAsync<string>(key);
        await _service.RemoveStateAsync(key);

        value.ShouldBeNull();
        _localStorage.ReceivedCalls().ShouldBeEmpty();
    }

    [Fact]
    public async Task SaveStateAsync_WhenBrowserWriteFails_KeepsTheValueInMemory()
    {
        await _service.InitializeAsync();
        _localStorage.SetItemAsStringAsync("key", Arg.Any<string>(), Arg.Any<CancellationToken>()).Throws(new InvalidOperationException("quota"));

        await _service.SaveStateAsync("key", "value");

        (await _service.GetStateAsync<string>("key")).ShouldBe("value");
    }

    [Fact]
    public async Task GetStateAsync_InBrowser_ReadsFromBrowserOnceAndThenFromMemory()
    {
        await _service.InitializeAsync();
        _localStorage.ContainKeyAsync("key", Arg.Any<CancellationToken>()).Returns(true);
        _localStorage.GetItemAsStringAsync("key", Arg.Any<CancellationToken>()).Returns("7");

        var first = await _service.GetStateAsync<int>("key");
        var second = await _service.GetStateAsync<int>("key");

        first.ShouldBe(7);
        second.ShouldBe(7);
        await _localStorage.Received(1).GetItemAsStringAsync("key", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetStateAsync_InBrowser_WithMissingOrBlankValue_ReturnsDefault()
    {
        await _service.InitializeAsync();
        _localStorage.ContainKeyAsync("missing", Arg.Any<CancellationToken>()).Returns(false);
        _localStorage.ContainKeyAsync("blank", Arg.Any<CancellationToken>()).Returns(true);
        _localStorage.GetItemAsStringAsync("blank", Arg.Any<CancellationToken>()).Returns("");

        (await _service.GetStateAsync<string>("missing")).ShouldBeNull();
        (await _service.GetStateAsync<string>("blank")).ShouldBeNull();
    }

    [Fact]
    public async Task GetStateAsync_InBrowser_WhenReadFails_ReturnsDefault()
    {
        await _service.InitializeAsync();
        _localStorage.ContainKeyAsync("key", Arg.Any<CancellationToken>()).Throws(new InvalidOperationException("boom"));

        (await _service.GetStateAsync<string>("key")).ShouldBeNull();
    }

    [Fact]
    public async Task GetStateAsync_WhenMemoryValueHasAnotherShape_FallsBackToBrowserStorage()
    {
        await _service.InitializeAsync();
        await _service.SaveStateAsync("key", "not a number");
        _localStorage.ContainKeyAsync("key", Arg.Any<CancellationToken>()).Returns(true);
        _localStorage.GetItemAsStringAsync("key", Arg.Any<CancellationToken>()).Returns("5");

        var result = await _service.GetStateAsync<int>("key");

        result.ShouldBe(5);
    }

    [Fact]
    public async Task GetStateAsync_WithoutBrowserStorage_WhenMemoryValueHasAnotherShape_ReturnsDefault()
    {
        BrowserStorageUnavailable();
        await _service.InitializeAsync();
        await _service.SaveStateAsync("key", "not a number");

        var result = await _service.GetStateAsync<int>("key");

        result.ShouldBe(0);
    }

    [Fact]
    public async Task RemoveStateAsync_WhenBrowserRemoveFails_StillRemovesFromMemory()
    {
        await _service.InitializeAsync();
        await _service.SaveStateAsync("key", "value");
        _localStorage.RemoveItemAsync("key", Arg.Any<CancellationToken>()).Throws(new InvalidOperationException("boom"));
        _localStorage.ContainKeyAsync("key", Arg.Any<CancellationToken>()).Returns(false);

        await _service.RemoveStateAsync("key");

        (await _service.GetStateAsync<string>("key")).ShouldBeNull();
        await _localStorage.Received(1).RemoveItemAsync("key", Arg.Any<CancellationToken>());
    }

    private sealed class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager()
        {
            Initialize("https://localhost/", "https://localhost/");
        }
    }
}
