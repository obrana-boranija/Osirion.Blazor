# Playwright E2E Testing Prerequisites

Before running the Playwright E2E tests, ensure the following prerequisites are installed:

## Required Software

1. **Node.js**: Required for Playwright
   - Download and install from [nodejs.org](https://nodejs.org/)
   - Verify installation with `node --version`

2. **npm**: Comes with Node.js
   - Verify installation with `npm --version`

3. **.NET SDK**: Required to build and run the tests
   - You should already have this for Blazor development
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

Run these commands in your terminal:
```
npm init -y
npm install playwright
npx playwright install --with-deps
```

## Troubleshooting

### "npx is not recognized"
- Make sure Node.js is installed correctly
- Try reinstalling Node.js, making sure to check the option for automatic tools installation

### Permission Issues
- Run the command prompt or terminal as Administrator
- If using PowerShell, ensure you have execution policy set to allow running scripts:
  `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`

### Path Issues
- Make sure Node.js is in your system PATH
- Restart your terminal after installing Node.js

### Browser Installation Issues
- Some browsers may fail to install due to corporate policies or antivirus software
- In this case, try installing with `--browser=chromium` to only install Chromium:
  `npx playwright install --browser=chromium --with-deps`