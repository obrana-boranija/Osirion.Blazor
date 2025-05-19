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

- .NET 8.0 SDK or newer (latest stable version recommended)
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
git remote add upstream https://github.com/obrana-boranija/Osirion.Blazor.git
```

4. Create a new branch for your work:

```bash
git checkout -b feature/your-feature-name
```

5. Build the solution:

```bash
dotnet build
```

6. Run the tests:

```bash
dotnet test
```

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
- [ ] Ensured code builds on both .NET 8 and .NET 9
- [ ] Verified SSR compatibility
- [ ] Updated the CHANGELOG.md
- [ ] Followed the component structure guidelines

## Running Tests

We use xUnit, bUnit, Shouldly, and NSubstitute for testing. To run the tests:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests for a specific project
dotnet test src/Osirion.Blazor.Core.Tests
```

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
- Include parameter tables for components
- Document supported frameworks and versions

## Versioning

We use [Semantic Versioning](https://semver.org/) (SemVer) for versioning:

- **MAJOR** version for incompatible API changes
- **MINOR** version for backward-compatible functionality additions
- **PATCH** version for backward-compatible bug fixes

### Version Update Process

1. Update version numbers in project files
2. Update CHANGELOG.md with new version section
3. Create a pull request for the version update
4. Tag the release commit once merged

We have automated workflows for:
- Preparing releases
- Publishing to NuGet
- Creating GitHub releases

## Thank You!

Your contributions help make Osirion.Blazor better for everyone. We appreciate your time and effort!