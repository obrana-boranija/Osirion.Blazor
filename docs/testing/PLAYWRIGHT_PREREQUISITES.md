# Playwright E2E Testing Prerequisites

This is a mirrored copy of the original document from tests/Osirion.Blazor.E2ETests/PLAYWRIGHT_PREREQUISITES.md to make it available in the main documentation tree.

Original: ../tests/Osirion.Blazor.E2ETests/PLAYWRIGHT_PREREQUISITES.md

Before running the Playwright E2E tests, ensure the following prerequisites are installed:

## Required Software

1. Node.js: Required for Playwright
   - Download and install from https://nodejs.org/
   - Verify installation with `node --version`

2. npm: Comes with Node.js
   - Verify installation with `npm --version`

3. .NET SDK: Required to build and run the tests
   - Verify installation with `dotnet --version`

## Installation Options

Choose one of these options to install Playwright:

### Option 1: Using npm Directly (Recommended)

Run the `npm-install-playwright.cmd` batch file, which will:
- Initialize npm if needed
- Install Playwright package
- Install browser dependencies

### Option 2: Using MSBuild Target

Update the project file as shown in the documentation and run:
```
dotnet build /t:InstallPlaywrightBrowsers
```

### Option 3: Manual Installation

```
npm init -y
npm install playwright
npx playwright install --with-deps
```

## Troubleshooting

- "npx is not recognized": ensure Node.js is installed and in PATH
- Permission issues: run terminal as Administrator; in PowerShell consider `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`
- Path issues: restart terminal after Node.js install
- Browser installation issues: try `npx playwright install --browser=chromium --with-deps`
