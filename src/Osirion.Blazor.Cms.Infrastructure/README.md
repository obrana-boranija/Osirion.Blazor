# Osirion.Blazor.Cms.Infrastructure

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms.Infrastructure)](https://www.nuget.org/packages/Osirion.Blazor.Cms.Infrastructure)
[![License](https://img.shields.io/github/license/demonjic/Osirion.Blazor)](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt)

Infrastructure layer for the Osirion.Blazor CMS ecosystem, implementing repositories, external service integrations, and infrastructure concerns.

## Overview

The Infrastructure project implements the Infrastructure layer in the Clean Architecture pattern, providing concrete implementations of the interfaces defined in the Domain and Application layers. It includes repositories, API clients, storage implementations, and external service integrations.

## Features

- **GitHub Provider**: Content provider and repositories that read and write through the GitHub API (`GitHubContentProvider`, `GitHubContentRepository`)
- **File System Provider**: Local file system content repository (`FileSystemContentRepository`). Without `last_modified` in a file's front matter, an item is last modified when it was created; set `FileSystemOptions.UseFileWriteTimeAsLastModified` to use the file's write time instead
- **Caching Integration**: In-memory, stale-while-revalidate caching of repository results, configured with `CacheOptions`
- **HTTP Clients**: Typed HTTP clients for the GitHub API (`GitHubApiClient`, `GitHubTokenProvider`)
- **Local Storage**: `LocalStorageService`, an `IStateStorageService` implementation backed by Blazored.LocalStorage
- **Dependency Injection**: Service registration extensions in `Osirion.Blazor.Cms.Infrastructure.DependencyInjection`
- **Category and tag queries**: A `Category` or `Tag` query matches by name (ignoring case) or by slug, exactly

## Registration

```csharp
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

builder.Services.AddCms(builder.Configuration, cms => cms
    .AddGitHub()
    .AddFileSystem(options => options.BasePath = "content"));
```

`AddCms(IConfiguration, Action<OsirionCmsBuilder>?)` registers the core services. Without the delegate it registers a provider for each configuration section it finds under `Osirion:Cms` (`GitHub` and `FileSystem`). `AddGitHubContentProvider`, `AddFileSystemContentProvider`, `AddCustomContentProvider<TProvider>` and `AddOsirionCqrs` are available as separate `IServiceCollection` extensions.

## Configuration Options

Both option classes derive from `ContentProviderOptions` (namespace `Osirion.Blazor.Cms.Domain.Options`), which adds `ProviderId`, `IsDefault`, `ContentPath`, `EnableLocalization`, `DefaultLocale`, `SupportedLocales`, `SupportedExtensions` and `ValidateContent`.

`GitHubOptions` binds from `Osirion:Cms:GitHub`. A named provider (`AddGitHubContentProvider(configuration, "Blog")`) binds from `Osirion:Cms:Web:GitHub:{name}`, or `Osirion:Cms:Admin:GitHub:{name}` when that section is absent:

```csharp
public class GitHubOptions : ContentProviderOptions
{
    public string Owner { get; set; } = string.Empty;
    public string Repository { get; set; } = string.Empty;
    public string Branch { get; set; } = "main";
    public string ApiToken { get; set; } = string.Empty;
    public string? ApiUrl { get; set; } = "https://api.github.com";
    public string? WebhookSecret { get; set; }
    public bool EnablePolling { get; set; } = false;
    public int PollingIntervalSeconds { get; set; } = 86400;
    public bool UseBackgroundCacheUpdate { get; set; } = true;
    public List<string> ExcludePatterns { get; set; } = new() { ".*", "_*" };
    // Authentication and inherited properties
}
```

`FileSystemOptions` binds from `Osirion:Cms:Web:FileSystem`, or from `Osirion:Cms:FileSystem:Web` when the first section is absent:

```csharp
public class FileSystemOptions : ContentProviderOptions
{
    public string BasePath { get; set; } = string.Empty;
    public bool WatchForChanges { get; set; } = true;
    public int PollingIntervalMs { get; set; } = 30000;
    public bool IncludeSubdirectories { get; set; } = true;
    public bool CreateDirectoriesIfNotExist { get; set; } = false;
    public List<string> IncludePatterns { get; set; } = new() { "**/*.md", "**/*.markdown" };
    public List<string> ExcludePatterns { get; set; } = new() { "**/node_modules/**", "**/bin/**", "**/obj/**" };
    public string? ContentRoot { get; set; }
    public bool UseFileWriteTimeAsLastModified { get; set; }
}
```

`ExcludePatterns` skips files whose path under `BasePath` matches a glob, such as `**/drafts/**`. `ContentRoot` is the prefix removed when URLs are built, and the inherited `ContentPath` stands in for it when `ContentRoot` is empty. `PollingIntervalMs` and `IncludePatterns` are obsolete because nothing reads them: use `WatchForChanges` to refresh content, and `SupportedExtensions` to choose the file types. `GitHubOptions.ExcludePatterns` is obsolete for the same reason: use `ContentPath` and `SupportedExtensions`.

`CacheOptions` binds from `Osirion:Cms:Cache` and is obsolete: the providers keep their own content caches, and the options only configure `IContentCacheService` and `CacheDecoratorFactory`, which no CMS component uses. Remove the section; the type goes in the next major version.

## GitHub API Clients and Credentials

- `IGitHubApiClientFactory.GetClient(name)` and `GetDefaultClient()` return a new `GitHubApiClient` on every call. The caller owns it: `SetRepository`, `SetBranch` and `SetAccessToken` change that instance only and never another caller's client.
- A client sends `Accept`, `User-Agent` and `Authorization` on each request message and never changes `HttpClient.DefaultRequestHeaders`. An `Accept` or `User-Agent` header you configure on the `HttpClient` is kept.
- Factory clients take a short-lived `HttpClient` named `GitHub_{ProviderName}` from `IHttpClientFactory` for every operation. Configure it with `services.AddHttpClient("GitHub_Blog")`.
- Cookies are turned off on the primary handler of the `GitHub_*`, `IGitHubApiClient`, `IGitHubTokenProvider` and `IAuthenticationService` clients, because a pooled handler is shared by every user. The policy runs after all handler configuration, so a primary handler you configure (through `ConfigureHttpClientDefaults` or the named client, before or after the Osirion registration) is kept with cookies turned off. In the browser it does nothing, because the fetch API owns cookies there.
- Repository paths, branch names, owner and repository are percent-encoded per segment. A backslash in a repository path is read as a `/` separator. A path or branch name with an empty, `.` or `..` segment is rejected with an `ArgumentException` before any request is sent. URLs built from letters, digits, `-`, `_`, `.`, `~` and `/` are unchanged.
- `IAuthenticationService` and `IGitHubAdminService` are registered as scoped, one per circuit or request, replacing any earlier registration. The admin service never uses a content provider's `ApiToken`: it uses the signed-in user's token, or `CmsAdminOptions.Authentication.PersonalAccessToken` when one is configured. Without either, repository operations send no request; `DeleteFileAsync`, for example, throws `InvalidOperationException` instead of sending an unauthenticated delete. Signing out drops the user's token from the admin service of the same scope.
- `AuthenticationService` keeps the user's token in memory only. It never writes the token to `IStateStorageService` (which stores plain text) and never signs anyone in with the configured `PersonalAccessToken`. `InitializeAsync` and sign-out remove a `github_auth_token` value that earlier versions left in browser storage. The admin module keeps a signed-in user's token between page loads in encrypted, user-bound protected browser storage instead.
- `GitHubTokenProvider` requests `Accept: application/json`, sends the `redirect_uri` and PKCE `code_verifier` it is given, treats an `error` field in a 200 response as a rejected exchange, and honours its `CancellationToken`. The original `ExchangeCodeForTokenAsync(code, clientId, clientSecret)` overload still works and sends neither value.

## Dependencies

The infrastructure layer has the following dependencies:

- **Osirion.Blazor.Cms.Domain**: For domain entities and interfaces
- **Osirion.Blazor.Cms.Application**: For application services and interfaces
- **Osirion.Blazor.Core**: For shared Osirion components and services
- **Blazored.LocalStorage**: For browser local storage access
- **Scrutor**: For assembly scanning in `AddOsirionCqrs`
- **System.IO.Abstractions**: For file system access in the file system provider
- **Microsoft.AspNetCore.App**: Shared framework reference

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt) file for details.