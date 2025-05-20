using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Admin.Tests.Core.State;

public class CmsStateTests
{
    [Fact]
    public void SelectRepository_ShouldUpdateStateAndNotifyListeners()
    {
        // Arrange
        var state = new CmsState();
        var notificationCount = 0;
        state.StateChanged += () => notificationCount++;

        var repository = new GitHubRepository
        {
            Name = "test-repo",
            FullName = "owner/test-repo",
            DefaultBranch = "main"
        };

        // Act
        state.SelectRepository(repository);

        // Assert
        state.SelectedRepository.ShouldBe(repository);
        state.SelectedBranch.ShouldBeNull();
        state.CurrentPath.ShouldBe(string.Empty);
        state.CurrentItems.ShouldBeEmpty();
        notificationCount.ShouldBe(1);
    }

    [Fact]
    public void SelectBranch_ShouldUpdateStateAndNotifyListeners()
    {
        // Arrange
        var state = new CmsState();
        var notificationCount = 0;
        state.StateChanged += () => notificationCount++;

        var branch = new GitHubBranch
        {
            Name = "main",
            Protected = false
        };

        // Act
        state.SelectBranch(branch);

        // Assert
        state.SelectedBranch.ShouldBe(branch);
        state.CurrentPath.ShouldBe(string.Empty);
        state.CurrentItems.ShouldBeEmpty();
        notificationCount.ShouldBe(1);
    }

    [Fact]
    public void SetCurrentPath_ShouldUpdatePathAndItems()
    {
        // Arrange
        var state = new CmsState();
        var notificationCount = 0;
        state.StateChanged += () => notificationCount++;

        var path = "content/blog";
        var items = new List<GitHubItem>
        {
            new GitHubItem { Name = "post.md", Type = "file", Path = "content/blog/post.md" }
        };

        // Act
        state.SetCurrentPath(path, items);

        // Assert
        state.CurrentPath.ShouldBe(path);
        state.CurrentItems.ShouldBe(items);
        notificationCount.ShouldBe(1);
    }

    [Fact]
    public void SetEditingPost_ShouldUpdateEditingStateAndNotifyListeners()
    {
        // Arrange
        var state = new CmsState();
        var notificationCount = 0;
        state.StateChanged += () => notificationCount++;

        var blogPost = new BlogPost
        {
            FilePath = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        // Act
        state.SetEditingPost(blogPost, true);

        // Assert
        state.EditingPost.ShouldBe(blogPost);
        state.IsEditing.ShouldBeTrue();
        state.IsCreatingNewFile.ShouldBeTrue();
        notificationCount.ShouldBe(1);
    }

    [Fact]
    public void ClearEditing_ShouldResetEditingStateAndNotifyListeners()
    {
        // Arrange
        var state = new CmsState();
        var notificationCount = 0;
        state.StateChanged += () => notificationCount++;

        var blogPost = new BlogPost
        {
            FilePath = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        state.SetEditingPost(blogPost, true);
        notificationCount = 0; // Reset counter

        // Act
        state.ClearEditing();

        // Assert
        state.EditingPost.ShouldBeNull();
        state.IsEditing.ShouldBeFalse();
        state.IsCreatingNewFile.ShouldBeFalse();
        notificationCount.ShouldBe(1);
    }

    [Fact]
    public void Reset_ShouldClearAllStateAndNotifyListeners()
    {
        // Arrange
        var state = new CmsState();
        var notificationCount = 0;
        state.StateChanged += () => notificationCount++;

        // Setup state with values
        state.SelectRepository(new GitHubRepository { Name = "test-repo" });
        state.SelectBranch(new GitHubBranch { Name = "main" });
        state.SetCurrentPath("content", new List<GitHubItem>());
        state.SetEditingPost(new BlogPost(), true);
        state.SetStatusMessage("Test message");

        notificationCount = 0; // Reset counter

        // Act
        state.Reset();

        // Assert
        state.SelectedRepository.ShouldBeNull();
        state.SelectedBranch.ShouldBeNull();
        state.CurrentPath.ShouldBe(string.Empty);
        state.CurrentItems.ShouldBeEmpty();
        state.EditingPost.ShouldBeNull();
        state.IsEditing.ShouldBeFalse();
        state.IsCreatingNewFile.ShouldBeFalse();
        state.StatusMessage.ShouldBeNull();
        state.ErrorMessage.ShouldBeNull();
        notificationCount.ShouldBe(1);
    }

    [Fact]
    public void Serialize_ShouldCreateJsonWithStateData()
    {
        // Arrange
        var state = new CmsState();
        var repository = new GitHubRepository
        {
            Name = "test-repo",
            FullName = "owner/test-repo",
            DefaultBranch = "main"
        };
        var branch = new GitHubBranch
        {
            Name = "main",
            Protected = false
        };

        state.SelectRepository(repository);
        state.SelectBranch(branch);
        state.SetCurrentPath("content/blog", new List<GitHubItem>());

        // Act
        var serialized = state.Serialize();

        // Assert
        serialized.ShouldNotBeNullOrEmpty();

        // Verify serialized content contains key state
        var deserialized = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(serialized);
        deserialized.ShouldNotBeNull();
        deserialized.ShouldContainKey("SelectedRepository");
        deserialized.ShouldContainKey("SelectedBranch");
        deserialized.ShouldContainKey("CurrentPath");

        // The values should match what we expect
        var repoElement = deserialized["SelectedRepository"];
        repoElement.GetProperty("Name").GetString().ShouldBe("test-repo");

        var branchElement = deserialized["SelectedBranch"];
        branchElement.GetProperty("Name").GetString().ShouldBe("main");

        deserialized["CurrentPath"].GetString().ShouldBe("content/blog");
    }

    [Fact]
    public void DeserializeFrom_ShouldRestoreState()
    {
        // Arrange
        var state = new CmsState();
        var notificationCount = 0;
        state.StateChanged += () => notificationCount++;

        var serializedData = @"{
            ""SelectedRepository"": {
                ""Name"": ""test-repo"",
                ""FullName"": ""owner/test-repo"",
                ""DefaultBranch"": ""main""
            },
            ""SelectedBranch"": {
                ""Name"": ""develop"",
                ""Protected"": false
            },
            ""CurrentPath"": ""content/articles"",
            ""CurrentTheme"": ""dark""
        }";

        // Act
        state.DeserializeFrom(serializedData);

        // Assert
        state.SelectedRepository.ShouldNotBeNull();
        state.SelectedRepository.Name.ShouldBe("test-repo");
        state.SelectedRepository.FullName.ShouldBe("owner/test-repo");
        state.SelectedRepository.DefaultBranch.ShouldBe("main");

        state.SelectedBranch.ShouldNotBeNull();
        state.SelectedBranch.Name.ShouldBe("develop");

        state.CurrentPath.ShouldBe("content/articles");
        notificationCount.ShouldBe(1);
    }
}