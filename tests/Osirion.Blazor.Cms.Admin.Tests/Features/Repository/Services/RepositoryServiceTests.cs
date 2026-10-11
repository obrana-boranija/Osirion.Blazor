using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Repository.Services;

public class RepositoryServiceTests
{
    private readonly IContentRepositoryAdapter _adapter = Substitute.For<IContentRepositoryAdapter>();
    private readonly RecordingLogger<RepositoryService> _logger = new();
    private readonly RepositoryService _service;

    public RepositoryServiceTests()
    {
        _service = new RepositoryService(_adapter, _logger);
    }

    [Fact]
    public async Task GetRepositoriesAsync_ReturnsTheAdapterResult()
    {
        var repositories = new List<GitHubRepository> { new() { Name = "site" } };
        _adapter.GetRepositoriesAsync().Returns(repositories);

        var result = await _service.GetRepositoriesAsync();

        result.ShouldBeSameAs(repositories);
        _logger.HasEntry(LogLevel.Information, "Retrieved 1 repositories").ShouldBeTrue();
    }

    [Fact]
    public async Task GetRepositoriesAsync_AdapterFails_LogsAndRethrows()
    {
        _adapter.GetRepositoriesAsync().ThrowsAsync(new InvalidOperationException("boom"));

        await Should.ThrowAsync<InvalidOperationException>(() => _service.GetRepositoriesAsync());

        _logger.HasEntry(LogLevel.Error, "Error retrieving repositories").ShouldBeTrue();
    }

    [Fact]
    public async Task GetBranchesAsync_AdapterFails_LogsTheRepositoryAndRethrows()
    {
        _adapter.GetBranchesAsync("site").ThrowsAsync(new InvalidOperationException("boom"));

        await Should.ThrowAsync<InvalidOperationException>(() => _service.GetBranchesAsync("site"));

        _logger.HasEntry(LogLevel.Error, "Error retrieving branches", "site").ShouldBeTrue();
    }

    [Fact]
    public async Task CreateBranchAsync_ReturnsTheCreatedBranch()
    {
        var branch = new GitHubBranch { Name = "feature" };
        _adapter.CreateBranchAsync("feature", "main").Returns(branch);

        var result = await _service.CreateBranchAsync("feature", "main");

        result.ShouldBeSameAs(branch);
        _logger.HasEntry(LogLevel.Information, "Branch feature created successfully").ShouldBeTrue();
    }

    [Fact]
    public async Task CreateBranchAsync_AdapterFails_LogsBothBranchesAndRethrows()
    {
        _adapter.CreateBranchAsync("feature", "main").ThrowsAsync(new InvalidOperationException("boom"));

        await Should.ThrowAsync<InvalidOperationException>(() => _service.CreateBranchAsync("feature", "main"));

        _logger.HasEntry(LogLevel.Error, "Error creating branch feature from main").ShouldBeTrue();
    }

    [Fact]
    public void SetRepository_ForwardsToTheAdapter()
    {
        _service.SetRepository("site");

        _adapter.Received(1).SetRepository("site");
    }

    [Fact]
    public void SetRepository_AdapterFails_LogsAndRethrows()
    {
        _adapter.When(adapter => adapter.SetRepository("site")).Do(_ => throw new InvalidOperationException("boom"));

        Should.Throw<InvalidOperationException>(() => _service.SetRepository("site"));

        _logger.HasEntry(LogLevel.Error, "Error setting repository: site").ShouldBeTrue();
    }

    [Fact]
    public void SetBranch_AdapterFails_LogsAndRethrows()
    {
        _adapter.When(adapter => adapter.SetBranch("main")).Do(_ => throw new InvalidOperationException("boom"));

        Should.Throw<InvalidOperationException>(() => _service.SetBranch("main"));

        _logger.HasEntry(LogLevel.Error, "Error setting branch: main").ShouldBeTrue();
    }
}
