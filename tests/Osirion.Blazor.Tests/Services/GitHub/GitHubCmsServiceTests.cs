using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Options;
using Osirion.Blazor.Services.GitHub;
using Shouldly;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Osirion.Blazor.Tests.Services.GitHub;

public class GitHubCmsServiceTests
{
    private readonly IOptions<GitHubCmsOptions> _options;
    private readonly ILogger<GitHubCmsService> _logger;
    private readonly TestHttpMessageHandler _httpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly GitHubCmsService _service;

    public GitHubCmsServiceTests()
    {
        _options = Microsoft.Extensions.Options.Options.Create(new GitHubCmsOptions
        {
            Owner = "test-owner",
            Repository = "test-repo",
            ContentPath = "content",
            Branch = "main",
            ApiToken = "test-token",
            CacheDurationMinutes = 30
        });

        _logger = Substitute.For<ILogger<GitHubCmsService>>();
        _httpMessageHandler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(_httpMessageHandler)
        {
            BaseAddress = new Uri("https://api.github.com/")
        };

        _service = new GitHubCmsService(_httpClient, _options, _logger);
    }

    [Fact]
    public async Task GetAllContentItemsAsync_ShouldReturnCachedContent_WhenCacheIsValid()
    {
        // Arrange
        var contentItems = new List<GitHubContent>
        {
            new GitHubContent
            {
                Name = "test.md",
                Path = "content/test.md",
                Type = "file",
                Url = "https://api.github.com/repos/test-owner/test-repo/contents/content/test.md"
            }
        };

        var fileContent = new GitHubFileContent
        {
            Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                @"---
title: ""Test Post""
author: ""Test Author""
date: ""2023-01-01""
description: ""Test description""
tags: [test, example]
keywords: [test, example]
slug: ""test-post""
is_featured: true
featured_image: ""test.jpg""
---

# Test Content

This is test content."))
        };

        var commits = new List<GitHubCommit>
        {
            new GitHubCommit
            {
                Sha = "test-sha",
                Commit = new GitHubCommitDetail
                {
                    Author = new GitHubCommitAuthor
                    {
                        Date = DateTime.Now
                    }
                }
            }
        };

        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/contents/content?ref=main", contentItems);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/contents/content/test.md", fileContent);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/commits?path=content/test.md&page=1&per_page=1", commits);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/commits?path=content/test.md&per_page=100", commits);

        // Act - First call should hit the API
        var result1 = await _service.GetAllContentItemsAsync();

        // Act - Second call should use cache
        var result2 = await _service.GetAllContentItemsAsync();

        // Assert
        result1.ShouldNotBeEmpty();
        result2.ShouldBe(result1);
        _httpMessageHandler.CallCount.ShouldBe(4); // One set of calls, not two
    }

    [Fact]
    public async Task GetContentItemByPathAsync_ShouldReturnCorrectItem()
    {
        // Arrange
        var contentItems = new List<GitHubContent>
        {
            new GitHubContent
            {
                Name = "test.md",
                Path = "content/test.md",
                Type = "file",
                Url = "https://api.github.com/repos/test-owner/test-repo/contents/content/test.md"
            }
        };

        var fileContent = new GitHubFileContent
        {
            Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                @"---
title: ""Test Post""
author: ""Test Author""
date: ""2023-01-01""
slug: ""test-post""
---

# Test Content"))
        };

        var commits = new List<GitHubCommit>
        {
            new GitHubCommit
            {
                Sha = "test-sha",
                Commit = new GitHubCommitDetail
                {
                    Author = new GitHubCommitAuthor
                    {
                        Date = DateTime.Now
                    }
                }
            }
        };

        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/contents/content?ref=main", contentItems);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/contents/content/test.md", fileContent);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/commits?path=content/test.md&page=1&per_page=1", commits);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/commits?path=content/test.md&per_page=100", commits);

        // Act
        var result = await _service.GetContentItemByPathAsync("content/test.md");

        // Assert
        result.ShouldNotBeNull();
        result.Title.ShouldBe("Test Post");
        result.Author.ShouldBe("Test Author");
        result.Slug.ShouldBe("test-post");
    }

    [Fact]
    public async Task GetCategoriesAsync_ShouldReturnCorrectCategories()
    {
        // Arrange
        var contentItems = new List<GitHubContent>
        {
            new GitHubContent
            {
                Name = "test1.md",
                Path = "content/test1.md",
                Type = "file",
                Url = "https://api.github.com/repos/test-owner/test-repo/contents/content/test1.md"
            }
        };

        var fileContent = new GitHubFileContent
        {
            Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                @"---
title: ""Test Post""
categories: [Category1, Category2]
---

# Test Content"))
        };

        var commits = new List<GitHubCommit>
        {
            new GitHubCommit
            {
                Sha = "test-sha",
                Commit = new GitHubCommitDetail
                {
                    Author = new GitHubCommitAuthor
                    {
                        Date = DateTime.Now
                    }
                }
            }
        };

        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/contents/content?ref=main", contentItems);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/contents/content/test1.md", fileContent);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/commits?path=content/test1.md&page=1&per_page=1", commits);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/commits?path=content/test1.md&per_page=100", commits);

        // Act
        var result = await _service.GetCategoriesAsync();

        // Assert
        result.Count.ShouldBe(2);
        result.Select(c => c.Name).ShouldContain("Category1");
        result.Select(c => c.Name).ShouldContain("Category2");
    }

    [Fact]
    public async Task SearchContentItemsAsync_ShouldReturnMatchingItems()
    {
        // Arrange
        var contentItems = new List<GitHubContent>
        {
            new GitHubContent
            {
                Name = "matching-post.md",
                Path = "content/matching-post.md",
                Type = "file",
                Url = "https://api.github.com/repos/test-owner/test-repo/contents/content/matching-post.md"
            },
            new GitHubContent
            {
                Name = "non-matching.md",
                Path = "content/non-matching.md",
                Type = "file",
                Url = "https://api.github.com/repos/test-owner/test-repo/contents/content/non-matching.md"
            }
        };

        var matchingFileContent = new GitHubFileContent
        {
            Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                @"---
title: ""Matching Post""
description: ""This contains the search term""
---

# Test Content

This post contains the search term we're looking for."))
        };

        var nonMatchingFileContent = new GitHubFileContent
        {
            Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                @"---
title: ""Non-Matching Post""
description: ""This doesn't contain it""
---

# Different Content

This post is about something else entirely."))
        };

        var commits = new List<GitHubCommit>
        {
            new GitHubCommit
            {
                Sha = "test-sha",
                Commit = new GitHubCommitDetail
                {
                    Author = new GitHubCommitAuthor
                    {
                        Date = DateTime.Now
                    }
                }
            }
        };

        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/contents/content?ref=main", contentItems);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/contents/content/matching-post.md", matchingFileContent);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/contents/content/non-matching.md", nonMatchingFileContent);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/commits?path=content/matching-post.md&page=1&per_page=1", commits);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/commits?path=content/matching-post.md&per_page=100", commits);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/commits?path=content/non-matching.md&page=1&per_page=1", commits);
        _httpMessageHandler.SetupResponse("/repos/test-owner/test-repo/commits?path=content/non-matching.md&per_page=100", commits);

        // Act
        var result = await _service.SearchContentItemsAsync("search term");

        // Assert
        result.Count.ShouldBe(1);
        result[0].Title.ShouldBe("Matching Post");
    }
}

public class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, HttpResponseMessage> _responses = new();
    public int CallCount { get; private set; }

    public void SetupResponse<T>(string url, T responseContent)
    {
        var responseJson = JsonSerializer.Serialize(responseContent);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };
        _responses[url] = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        CallCount++;

        if (request.RequestUri == null)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        if (_responses.TryGetValue(request.RequestUri.ToString(), out var response))
        {
            return Task.FromResult(response);
        }

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
    }
}