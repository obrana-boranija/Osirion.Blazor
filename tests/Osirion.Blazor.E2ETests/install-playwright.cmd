@echo off
echo Installing Playwright browsers and dependencies...

rem Make sure Node.js is installed
where node >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo Error: Node.js is not installed or not in the PATH.
    echo Please install Node.js from https://nodejs.org/
    pause
    exit /b 1
)

rem Install browsers using npx directly
powershell.exe -ExecutionPolicy Bypass -Command "npx playwright install --with-deps"

if %ERRORLEVEL% NEQ 0 (
    echo Installation failed. Please check the error message above.
) else (
    echo Playwright installation completed successfully!
)

pause