# E2E Testing with Playwright

This project uses Playwright for end-to-end testing of components in real browsers.

## Setup Instructions

### First-time Setup

1. Install Playwright browsers and dependencies:

```bash
# Option 1: Using the batch file (recommended for Windows)
.\install-playwright.cmd

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

#### Browser Installation Issues

If you encounter browser installation issues:

1. Ensure you have sufficient permissions to install browsers
2. Try running the installation commands with administrator privileges
3. Check that your antivirus software isn't blocking the installation

#### Test Application Issues

The tests expect a test application running at http://localhost:5000. Make sure:

1. Your test application is running before executing tests
2. The application includes all necessary component demos
3. The component demos match the expected URL routes

### Running on CI/CD

The E2E tests are configured to run automatically in the CI/CD pipeline. See the GitHub workflow files for details.