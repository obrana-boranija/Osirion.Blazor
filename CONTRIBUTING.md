# Contributing to Osirion.Blazor

Thank you for your interest in contributing to Osirion.Blazor! This document provides guidelines and instructions for contributing to the project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Development Setup](#development-setup)
- [Project Structure](#project-structure)
- [Coding Guidelines](#coding-guidelines)
- [Component Structure](#component-structure)
- [Pull Request Process](#pull-request-process)
- [Running Tests](#running-tests)
- [Documentation](#documentation)
- [Versioning](#versioning)

## Code of Conduct

This project adheres to a Code of Conduct that we expect all contributors to follow. Please be respectful, inclusive, and considerate in all interactions.

## Development Setup

### Prerequisites

- .NET 10 SDK (the version in `global.json` or a later feature band)
- Your favorite IDE (Visual Studio, VS Code, Rider, etc.)
- Git

### Getting Started

1. Fork the repository on GitHub
2. Clone your fork locally:

```bash
git clone https://github.com/YOUR-USERNAME/Osirion.Blazor.git
cd Osirion.Blazor
```

3. Add the upstream repository as a remote:

```bash
git remote add upstream https://github.com/demonjic/Osirion.Blazor.git
```

4. Create a new branch for your work:

```bash
git checkout -b feature/your-feature-name
```

5. Build the solution (`Osirion.Blazor.slnx`):

```bash
dotnet build
```

6. Run the tests:

```bash
dotnet test
```

7. Run the example sites. The Aspire AppHost starts every example (Bootstrap on `http://localhost:5296`, MudBlazor on
   `http://localhost:5297`, Fluent UI on `http://localhost:5298`) and opens the Aspire dashboard with their logs:

```bash
dotnet run --project examples/Osirion.Blazor.Examples.AppHost
```

   The examples read the repository's `content` folder in Development, so they need no GitHub token. To run one site
   alone, use `dotnet watch --project examples/Osirion.Blazor.Example.Bootstrap`.

## Project Structure

The Osirion.Blazor repository is organized into several projects:

- **Osirion.Blazor**: Main package that references all modules
- **Osirion.Blazor.Core**: Core components and base classes
- **Osirion.Blazor.Analytics**: Analytics tracking components
- **Osirion.Blazor.Navigation**: Navigation and scroll components
- **Osirion.Blazor.Cms**: Content management system
- **Osirion.Blazor.Cms.Core**: Core CMS abstractions
- **Osirion.Blazor.Cms.Admin**: Admin interface for CMS
- **Osirion.Blazor.Cms.Infrastructure**: Infrastructure for CMS
- **Osirion.Blazor.Theming**: Theming and styling system

## Coding Guidelines

We follow standard C# coding conventions with a few specific requirements:

### General

- Use C# latest language features
- Follow SOLID, KISS, and DRY principles
- Aim for Clean Code principles
- Use nullable reference types (enable `<Nullable>enable</Nullable>` in project files)
- Use pattern matching where appropriate
- Use `async`/`await` for asynchronous code

### Formatting

- Use 4 spaces for indentation (not tabs)
- Add a newline at the end of the file
- Trim trailing whitespace
- Keep line length reasonable (around 120 characters)

### Naming

- Use PascalCase for class names, properties, and methods
- Use camelCase for local variables and parameters
- Use PascalCase for public fields and camelCase for private fields
- Prefix interfaces with `I`
- Prefix private fields with `_`

### Documentation

- Use XML comments for public APIs
- Document parameters and return values
- Provide example usage for complex functionality

### SSR Compatibility

- Avoid JavaScript Interop where possible
- Use progressive enhancement techniques
- Test all components in SSR mode

## Component Structure

All components should follow this structure:

1. **Separation of Concerns**: Split components into separate files for markup, code, and styling
2. **File Naming**:
   - `ComponentName.razor`: Component markup
   - `ComponentName.razor.cs`: Component code
   - `ComponentName.razor.css`: Component styles
3. **Directory Structure**:
   ```
   src/Osirion.Blazor.ModuleName/
   ├── Components/
   │   ├── ComponentName/
   │   │   ├── ComponentName.razor
   │   │   ├── ComponentName.razor.cs
   │   │   └── ComponentName.razor.css
   ```

### Component Base Class

Components should inherit from `OsirionComponentBase` in the Core package:

```csharp
using Osirion.Blazor.Components;

public partial class MyComponent : OsirionComponentBase
{
    // Component code here
}
```

### CSS Isolation

Use Blazor's CSS isolation for component styles:

```css
/* ComponentName.razor.css */
.my-component {
    display: flex;
    padding: var(--osirion-padding);
}
```

Always use CSS variables from the theming system for colors, sizes, etc.

## Pull Request Process

1. Create a new branch for your work (never commit directly to `master`)
2. Make your changes, following the coding guidelines
3. Add or update tests for your changes
4. Add or update documentation as needed
5. Update the CHANGELOG.md file with your changes
6. Ensure all tests pass with `dotnet test`
7. Submit a pull request (PR) to the `master` branch
8. Fill out the PR template with all required information
9. Request a review from a maintainer

### PR Checklist

Before submitting your PR, make sure you've:

- [ ] Added tests for your changes
- [ ] Updated relevant documentation
- [ ] Ensured code builds on both .NET 9 and .NET 10
- [ ] Verified SSR compatibility
- [ ] Updated the CHANGELOG.md
- [ ] Followed the component structure guidelines

## Running Tests

We use xUnit, bUnit, Shouldly, and NSubstitute for testing. To run the tests:

```bash
# Run all tests
dotnet test

# Run tests with coverage (same settings as CI)
dotnet test tests/Osirion.Blazor.Core.Tests --settings coverlet.runsettings --collect:"XPlat Code Coverage"

# Run tests for a specific project
dotnet test tests/Osirion.Blazor.Core.Tests
```

The Playwright suite in `tests/Osirion.Blazor.E2ETests` needs browsers installed (`playwright install chromium`)
and is not part of the coverage run.

### Coverage and CRAP scores

The target is 90% line coverage, and no method may have a CRAP score above 30
(`CRAP = CC² × (1 - coverage)³ + CC`, where CC is cyclomatic complexity). A complex method therefore needs
either more tests or a split into smaller methods. Measure both locally before opening a pull request:

```powershell
dotnet tool restore                    # installs the pinned crap4dotnet
./tools/run-crap.ps1                   # runs every test project with coverage, writes docs/crap/<Project>.json
./tools/verify-crap.ps1 -MaxCrap 30    # lists methods over 30 and regressions against the last snapshot
```

`run-crap.ps1 -SkipTests` reuses coverage from an earlier run, `-Project Osirion.Blazor.Core` limits the
analysis to one project, and `-RecordSnapshot` appends the result to `docs/crap/.progress-history.json` and
regenerates `docs/crap/PROGRESS.md`. Do not edit the files in `docs/crap` by hand. The CI coverage workflow
runs the same scripts and fails when a method exceeds CRAP 30; Codecov fails a pull request whose project or patch
coverage is below 90%. Record a snapshot (`-RecordSnapshot`) in the pull request that lowers the number of methods
over the threshold, so the gate's regression check compares against it.

### Writing Tests

When writing tests:

1. Use the AAA pattern (Arrange, Act, Assert)
2. Keep tests focused and small
3. Use descriptive test names that explain what's being tested
4. Use bUnit for component tests
5. Use Shouldly for assertions
6. Use NSubstitute for mocking

Example:

```csharp
[Fact]
public void ThemeToggle_ShouldRenderCorrectly_WhenSystemPrefIsEnabled()
{
    // Arrange
    var service = Substitute.For<IThemeService>();
    service.CurrentTheme.Returns(Theme.System);
    
    using var ctx = new TestContext();
    ctx.Services.AddSingleton(service);
    
    // Act
    var cut = ctx.RenderComponent<ThemeToggle>(parameters => parameters
        .Add(p => p.ShowSystemOption, true));
    
    // Assert
    cut.Find("button[data-theme='system']").ClassList.Should().Contain("active");
    cut.FindAll("button").Count.Should().Be(3);
}
```

## Documentation

Documentation is a crucial part of our project. When adding or modifying features:

1. Update the relevant README.md files
2. Add XML comments to public APIs
3. Update or create documentation in the `docs/` directory
4. Add usage examples for new features
5. Update the MIGRATION.md file if there are breaking changes

### Documentation Guidelines

- Use clear, concise language
- Include code examples for all features
- Structure documents with proper headings
- Link to related documentation
- Include parameter tables for components (see below)
- Document supported frameworks and versions

### Generated parameter tables

Parameter tables in the site content can be generated from the components, so they cannot drift from the code. Put
a pair of markers where the table belongs:

```markdown
<!-- osirion:parameters OsirionTabs -->
<!-- /osirion:parameters -->
```

Then run the generator over the content folder. It rewrites everything between each pair of markers with a table of
the component's `[Parameter]` properties (its own first, then inherited ones), their types, the defaults their
initializers set and the `<summary>` from the XML docs:

```powershell
dotnet run --project tools/Osirion.Blazor.ApiDocs -- content
dotnet run --project tools/Osirion.Blazor.ApiDocs -- content --check   # exit code 1 when a table is out of date
```

The description comes from the XML docs, so write the `<summary>` of a parameter for the reader of the website.

## Versioning

We use [Semantic Versioning](https://semver.org/) (SemVer) for versioning:

- **MAJOR** version for incompatible API changes
- **MINOR** version for backward-compatible functionality additions
- **PATCH** version for backward-compatible bug fixes

### Version Update Process

1. Run the "Prepare Release" workflow with the new version. It opens the pull request `release/<version>`, which sets
   the version in every package under `src` and moves the Unreleased notes in CHANGELOG.md under the new version.
2. Read the notes in that pull request: they become the GitHub release text. Then merge it.
3. Push the tag `v<version>` on the merge commit. "Publish to NuGet" checks that the tag, every package and
   CHANGELOG.md name the same version, runs the tests, publishes the packages and creates the GitHub release.

## Thank You!

Your contributions help make Osirion.Blazor better for everyone. We appreciate your time and effort!