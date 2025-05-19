using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services
{
    public class LocalStorageServiceTests
    {
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly ILogger<LocalStorageService> _logger;
        private readonly LocalStorageService _service;

        public LocalStorageServiceTests()
        {
            _localStorage = Substitute.For<ILocalStorageService>();
            _navigationManager = Substitute.For<NavigationManager>();
            _logger = Substitute.For<ILogger<LocalStorageService>>();

            _service = new LocalStorageService(
                _localStorage,
                _navigationManager,
                _logger);
        }

        [Fact]
        public async Task InitializeAsync_SetsIsInitialized()
        {
            // Arrange
            _service.IsInitialized.ShouldBeFalse();

            // Act
            await _service.InitializeAsync();

            // Assert
            _service.IsInitialized.ShouldBeTrue();
        }

        [Fact]
        public async Task SaveStateAsync_WithInitializedService_SavesToLocalStorage()
        {
            // Arrange
            await _service.InitializeAsync();
            var key = "test-key";
            var value = "test-value";

            // Act
            await _service.SaveStateAsync(key, value);

            // Assert
            await _localStorage.Received(1).SetItemAsStringAsync(
                key,
                Arg.Is<string>(s => s.Contains(value)));
        }

        [Fact]
        public async Task GetStateAsync_WithExistingItem_ReturnsItem()
        {
            // Arrange
            await _service.InitializeAsync();
            var key = "test-key";
            var value = "test-value";
            var json = System.Text.Json.JsonSerializer.Serialize(value);

            _localStorage.ContainKeyAsync(key).Returns(true);
            _localStorage.GetItemAsStringAsync(key).Returns(json);

            // Act
            var result = await _service.GetStateAsync<string>(key);

            // Assert
            result.ShouldBe(value);
            await _localStorage.Received(1).GetItemAsStringAsync(key);
        }

        [Fact]
        public async Task RemoveStateAsync_WithInitializedService_RemovesFromLocalStorage()
        {
            // Arrange
            await _service.InitializeAsync();
            var key = "test-key";

            // Act
            await _service.RemoveStateAsync(key);

            // Assert
            await _localStorage.Received(1).RemoveItemAsync(key);
        }
    }
}