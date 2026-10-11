# Osirion.Blazor Copilot Instructions

## Project Overview

Osirion.Blazor is a modular, high-performance component library for Blazor applications that provides SSR-compatible solutions for common web application needs. Built with a focus on performance, modularity, and developer experience, Osirion.Blazor works seamlessly across all Blazor hosting models (Static Server-Side Rendering/SSG, Server Interactive, and WASM).

**Repository**: https://github.com/obrana-boranija/Osirion.Blazor  
**NuGet Package**: https://www.nuget.org/packages/Osirion.Blazor

### Key Features
- **Analytics Module**: Easily integrate Clarity and Matomo tracking with SSR support
- **Navigation Module**: Enhance Blazor navigation with scroll restoration and smooth transitions
- **Content Module**: Create content-driven sites with flexible provider support (GitHub, local files)
- **Theming Module**: Seamlessly integrate with popular CSS frameworks (Bootstrap, FluentUI, MudBlazor, Radzen)

### Design Philosophy
- **SSR-First**: All components work with Static Server-Side Rendering (SSG), Server Interactive, and WASM
- **Progressive Enhancement**: Minimal JavaScript, enhanced interactivity when available
- **Multi-Version Support**: Targets .NET 9 and .NET 10
- **Production-Ready**: Clean APIs, comprehensive documentation, and flexible customization
- **Best Practices**: Follows SOLID, KISS, DRY, and Clean Code principles

## Architecture

### Module Structure
The solution consists of 12 distinct packages organized in a modular hierarchy:

```
Osirion.Blazor (Meta-package)
├── Osirion.Blazor.Core (Foundation components)
├── Osirion.Blazor.Analytics (Provider-based analytics)
├── Osirion.Blazor.Navigation (Enhanced navigation/scroll)
├── Osirion.Blazor.Theming (CSS framework integration)
└── Osirion.Blazor.Cms (Content management with DDD)
    ├── Osirion.Blazor.Cms.Domain (DDD entities/interfaces)
    ├── Osirion.Blazor.Cms.Application (CQRS handlers)
    ├── Osirion.Blazor.Cms.Infrastructure (Providers/Repositories)
    ├── Osirion.Blazor.Cms.Core (CMS components)
    ├── Osirion.Blazor.Cms.Admin (Admin UI)
    └── Osirion.Blazor.Cms.Web (Public-facing CMS)
```

**Key Pattern**: Each module is independently usable. The meta-package (`Osirion.Blazor`) references all modules for convenience.

### Component Base Class
All components inherit from `OsirionComponentBase` at `src/Osirion.Blazor.Core/Components/_Base/OsirionComponentBase.cs`. This provides:
- Interactivity detection via the `IsInteractive` property (`IsRunningInBrowser` reports the WebAssembly runtime; the old `IsServerSide` is obsolete because it is also true under Interactive Server)
- Theme support (`ThemeMode.Light|Dark|System`)
- .NET 9/10 conditional compilation for interactivity handling
- Unmatched attributes capture via `Attributes` dictionary

### Provider Pattern
Services follow a provider-based architecture for extensibility:
- **Content Providers**: GitHub, FileSystem (inherit from `ContentProviderBase`)
- **Analytics Providers**: Clarity, Matomo, GA4, Yandex (registered via `IAnalyticsBuilder`)
- Multiple providers can coexist; use `IContentProviderRegistry` for multi-provider scenarios

### Fluent Builder API
Service registration uses fluent builders. Example from `IOsirionBuilder`:
```csharp
builder.Services.AddOsirionBlazor(osirion => {
    osirion
        .UseContent(content => content.AddGitHub(...))
        .UseAnalytics(analytics => analytics.AddClarity(...))
        .UseNavigation(nav => nav.AddScrollToTop())
        .UseTheming(theme => theme.WithFramework(CssFramework.Bootstrap));
});
```

## Development Conventions

### File Organization
- **Components**: `ComponentName/ComponentName.razor`, `ComponentName.razor.cs`, `ComponentName.razor.css`
- **Extensions**: Place in `DependencyInjection/` or `Extensions/` folders, suffix with `Extensions.cs`
- **Interfaces**: Prefix with `I`, define in `Interfaces/` or alongside implementations
- **Options**: Suffix with `Options`, typically in `Options/` folder

### Naming Standards
- **Namespaces**: `Osirion.Blazor.{Module}.{Category}` (e.g., `Osirion.Blazor.Cms.Domain.Entities`)
- **Components**: PascalCase, prefix with `Osirion` for public-facing (e.g., `OsirionBreadcrumbs`)
- **Private fields**: Prefix with `_` (e.g., `_logger`)
- **Parameters**: Use `[Parameter]` attribute, document via XML comments

### Best Practices
**CRITICAL**: Follow these principles rigorously:
- **SOLID**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **KISS**: Keep It Simple, Stupid - avoid over-engineering
- **DRY**: Don't Repeat Yourself - extract reusable logic
- **Clean Code**: Meaningful names, small functions, clear intent, comprehensive comments

### Engineering Checklist (every change)
These turn the principles above into checks. Apply them to every change, however small, and to your own diff before
each commit.

1. **One rule, one place (DRY).** Before writing a helper, look for one that exists: `ContentUrls` (tag, category and
   pagination URLs), `SafeHref` (link schemes), `SanitizedContentCache`, the `--osirion-*` theme tokens. When the same
   expression, fallback or regex would appear a second time, move it into the shared helper and call it from both
   places. When a fix reveals the same rule duplicated in components you are not changing, record it as an issue
   instead of copying it once more.
2. **The smallest change that fully solves the problem (KISS, YAGNI).** No speculative parameters, options or
   abstractions; a new public member needs a caller and a test. Prefer one clear condition over a clever mechanism.
3. **SOLID boundaries.** Components render, services load data, helpers stay pure and static. A module never reaches
   into another module's internals: shared styling goes through public hooks (`--osirion-*` variables and documented
   classes), shared logic through the module that owns it.
4. **Fix the cause in the library.** A defect seen on an example site is fixed in the component, not patched with
   example CSS or markup. The example sites stay thin shells that compose library components and supply content.
5. **Comments explain why, next to what they describe.** A doc comment sits on the member it documents; no
   commented-out code, no TODO without an issue.
6. **Tests come with the change.** A bug fix adds a test that fails without the fix; a guard test is labelled as one.
   Run the tests so their exit code is visible (no pipe that hides it) before pushing.
7. **Self-review the diff before committing.** Read it top to bottom for duplicated logic, dead code, misplaced
   comments, unrelated reformatting, byte-order-mark or line-ending churn, and em dashes.
8. **One step, one pull request.** Each PR states why, what changed, how it was verified, and what was not verified.

### Multi-Targeting (.NET 9 & 10)
- Source projects target `net9.0;net10.0` through `src/Directory.Build.props` (`OsirionTargetFrameworks` in the root `Directory.Build.props`)
- Test projects target `<TargetFramework>net10.0</TargetFramework>` only
- Shared settings live in `Directory.Build.props` (all projects, warnings are errors) and `src/Directory.Build.props` (package metadata, SourceLink); package versions live in `Directory.Packages.props`
- Use conditional compilation for version-specific code:
  ```csharp
  #if NET9_0_OR_GREATER
      [Obsolete("Use RendererInfo.IsInteractive in .NET 9", true)]
  #endif
  ```

### Domain-Driven Design (CMS Modules)
The CMS follows DDD patterns:
- **Entities**: Inherit from `Entity<TId>` in `Cms.Domain/Common/`, support domain events
- **Repositories**: Inherit from `RepositoryBase<T, TId>`, implement `IRepository<T, TId>`
- **CQRS**: Commands/Queries in `Cms.Application/`, handlers use MediatR-style pattern
- **Domain Events**: Implement `IDomainEvent`, raised via `Entity.AddDomainEvent()`

## Testing Strategy

### Frameworks & Libraries
**REQUIRED**: Comprehensive unit tests using:
- **xUnit**: Testing framework
- **bUnit**: Blazor component testing
- **Shouldly**: Fluent assertions
- **NSubstitute**: Mocking and substitutes
- **E2E Tests**: Playwright (see `tests/Osirion.Blazor.E2ETests/`, run via `e2e.yml`)
- **Base Class**: Component tests inherit from `OsirionComponentTestBase` which configures `ThemingOptions` and JSInterop

### Test Organization
- Test projects mirror source structure: `Osirion.Blazor.{Module}.Tests`
- Component tests in `Components/` subfolder matching source structure
- Use descriptive test method names: `ComponentName_Scenario_ExpectedBehavior`

### Running Tests
```powershell
# All tests
dotnet test

# E2E tests (requires Playwright installation)
dotnet test tests/Osirion.Blazor.E2ETests/
```

## Build & Development Workflow

### Local Development
```powershell
# Restore and build (the solution is Osirion.Blazor.slnx)
dotnet restore
dotnet build

# Run every example site (Bootstrap :5296, MudBlazor :5297, Fluent UI :5298) with the Aspire dashboard
dotnet run --project examples/Osirion.Blazor.Examples.AppHost

# Run one example app with hot reload
dotnet watch --project examples/Osirion.Blazor.Example.Bootstrap/Osirion.Blazor.Example.Bootstrap.csproj
```

The three example sites share their menu, footer and framework-neutral pages (home, blog, docs, tags, privacy, not
found) through `examples/Osirion.Blazor.Examples.Shared`. A site keeps only what depends on its CSS framework: `App`,
`MainLayout`, `Routes` and the pages written with the framework's own components. Change shared parts once, there.

### CI/CD
- **.github/workflows/build.yml**: Runs on push to `master`, executes `dotnet build` and `dotnet test`
- **e2e.yml** (repository root, not run by GitHub Actions): Playwright tests, installs Chromium dependencies on Ubuntu
- Workflows install the .NET 9 and .NET 10 SDKs; `global.json` pins the .NET 10 SDK

### CSS Build Process
- CSS minification via Node.js tools in `build/tools/` (uses `clean-css` package)
- Custom MSBuild target in `build/Osirion.Blazor.targets` for CSS tree-shaking

## SSR-First Development

**CRITICAL REQUIREMENT**: Components MUST be compatible with Blazor SSR (no interactivity and no JSInterop where possible).

### Critical Patterns
1. **Avoid JSInterop** in component initialization; use it only for progressive enhancement
2. **Test without JS**: Components must render meaningfully server-side
3. **Progressive Enhancement**: Add JavaScript features via `OnAfterRender(firstRender: true)` for interactive mode
4. **Conditional Rendering**: Use `IsInteractive` (`RendererInfo.IsInteractive`) to branch logic; never `IsServerSide`, which is true under Interactive Server too
5. **SSR Compatibility**: All features must work with Static Server-Side Rendering (SSG), Server Interactive, and WASM

### Example
```csharp
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender && IsInteractive)
    {
        // Client-side enhancement only
        await JSRuntime.InvokeVoidAsync("initScrollBehavior");
    }
}
```

## Documentation Sync

### Component Documentation
- **Docs**: `docs/components/{module}/` contains markdown for each component
- **Automation**: `tools/check_component_docs.ps1` validates documentation completeness
- **Pattern**: Each public component should have corresponding `.md` in `docs/components/`

### Documentation Maintenance
**ALWAYS update these files when making changes**:
- **README.md**: Project overview, installation, quick start
- **CHANGELOG.md**: Version history and changes
- **MIGRATION.md**: Upgrade guides between versions
- **CONTRIBUTING.md**: Development guidelines
- **QUICK_REFERENCE.md**: API quick reference
- **LICENSE.txt**: License information
- **Module-specific docs**: ANALYTICS.md, NAVIGATION.md, GITHUB_CMS.md, THEMING.md, etc.

### Key Documentation Files
- **ARCHITECTURE.md**: Deep-dive on design patterns and data flows
- **GITHUB_CMS.md**: Comprehensive CMS usage guide with frontmatter spec
- **CONTRIBUTING.md**: Development setup and PR guidelines
- **MIGRATION.md**: Version upgrade guides

## Common Tasks

### Adding a New Component
**MANDATORY FILE STRUCTURE**: Every `.razor` component MUST have corresponding `.razor.cs` and `.razor.css` files.

1. Create three files in `{Component}/` folder:
   - `{Component}.razor` (markup only)
   - `{Component}.razor.cs` (code-behind with logic)
   - `{Component}.razor.css` (isolated CSS styles)
2. Inherit from `OsirionComponentBase` in the code-behind
3. Add comprehensive XML documentation to all public members
4. Create test class inheriting `OsirionComponentTestBase` using xUnit, bUnit, Shouldly, NSubstitute
5. Ensure SSR compatibility (no JSInterop in initialization)
6. Add documentation to `docs/components/{module}/{Component}.md`
7. Update module-specific docs (e.g., NAVIGATION.md for navigation components)
8. Run `tools/check_component_docs.ps1` to verify

### Adding a New Provider
1. Implement `IContentProvider` (CMS) or create analytics provider interface
2. Inherit from `ContentProviderBase` if applicable
3. Register via builder extension method in `DependencyInjection/`
4. Add configuration options class suffixed with `Options`
5. Document provider setup in relevant guide (`GITHUB_CMS.md` or `ANALYTICS.md`)

### Multi-Framework CSS Integration
When adding framework-specific styles:
1. Add integration class to `src/Osirion.Blazor.Theming/wwwroot/css/frameworks/`
2. Update `CssFramework` enum in `Osirion.Blazor.Theming/Enums/`
3. Test with `OsirionStyles FrameworkIntegration="{Framework}"`

## Important Gotchas

- **Namespace Mixing**: Some types use `Osirion.Blazor.Components` (components/enums) vs `Osirion.Blazor.Core.{Category}` (services). Check existing patterns.
- **Cache Decorators**: CMS uses decorator pattern for caching; don't add redundant caching in providers (see `ContentProviderBase` comment).
- **.NET 9 RendererInfo**: When targeting .NET 9+, prefer `RendererInfo.IsInteractive` over `SetInteractive` parameter.
- **Playwright Prerequisites**: E2E tests require manual Playwright CLI installation (`playwright install chromium`).
