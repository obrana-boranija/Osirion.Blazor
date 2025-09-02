# E2E Testing with Playwright

This is a mirrored copy of the original document from tests/Osirion.Blazor.E2ETests/PLAYWRIGHT_TESTING.md to make it available in the main documentation tree.

Original: ../tests/Osirion.Blazor.E2ETests/PLAYWRIGHT_TESTING.md

## Setup Instructions

### First-time Setup

1. Install Playwright browsers and dependencies:

```bash
# Option 1: Using the batch file (recommended for Windows)
./install-playwright.cmd

# Option 2: Build the project with the special target
dotnet build /t:InstallPlaywrightBrowsers
```

The installation process will:
- Build the test project
- Install the required Playwright browser binaries
- Install browser dependencies

### Running Tests

Run all E2E tests:

```bash
dotnet test tests/Osirion.Blazor.E2ETests
```

Run specific test categories:

```bash
dotnet test tests/Osirion.Blazor.E2ETests --filter "FullyQualifiedName~NavigationComponentTests"
```

### Debug Mode

To run tests with browser visible for debugging:

```bash
# Windows PowerShell
$env:PWDEBUG=1; dotnet test tests/Osirion.Blazor.E2ETests

# Windows Command Prompt
set PWDEBUG=1 && dotnet test tests/Osirion.Blazor.E2ETests

# Linux/macOS
PWDEBUG=1 dotnet test tests/Osirion.Blazor.E2ETests
```

### Troubleshooting

- Browser installation: ensure permissions and try running as admin
- Test app: ensure a demo app is running on http://localhost:5000 with required routes

### CI/CD

These tests can be executed on CI agents. Configure the workflow to install Playwright and run `dotnet test` for tests/Osirion.Blazor.E2ETests.
