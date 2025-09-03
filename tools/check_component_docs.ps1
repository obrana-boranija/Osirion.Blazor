# Deterministic checker for .razor -> .md docs
# Usage: powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\check_component_docs.ps1

$scriptFile = $MyInvocation.MyCommand.Definition
$toolsDir = Split-Path -Parent $scriptFile
$root = Split-Path -Parent $toolsDir
Write-Host "Repository root: $root"

$srcPath = Join-Path $root 'src'
if (-not (Test-Path $srcPath)) {
    Write-Host "Source path not found: $srcPath" -ForegroundColor Red
    exit 1
}
$docsPath = Join-Path $root 'docs\components'

# gather razor and md files
$razorFiles = Get-ChildItem -Path $srcPath -Recurse -File -Filter *.razor | Where-Object { $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\bin\\' }
$docFiles = @()
if (Test-Path $docsPath) { $docFiles += Get-ChildItem -Path $docsPath -Recurse -File -Filter *.md }
$docFiles += Get-ChildItem -Path $srcPath -Recurse -File -Filter *.md | Where-Object { $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\bin\\' }
$docPathsLower = $docFiles | ForEach-Object { $_.FullName.ToLowerInvariant() }

Write-Host "Found $($razorFiles.Count) .razor files and $($docFiles.Count) .md docs" -ForegroundColor Cyan

$missing = @()

foreach ($r in $razorFiles) {
    $componentName = [System.IO.Path]::GetFileNameWithoutExtension($r.Name)
    if ($componentName -eq '_Imports') { continue }

    # compute project folder (first folder under src)
    $rel = $r.FullName.Substring($srcPath.Length).TrimStart('\','/')
    $parts = $rel -split '[\\/]'
    $projectFolder = $parts[0]

    # derive module folder name used under docs/components
    $module = $projectFolder -replace '^Osirion\.Blazor\.', ''
    $moduleLower = $module.ToLowerInvariant()

    # expected doc path under docs/components/<module>/<Component>.md
    $expectedDir = Join-Path $docsPath $moduleLower
    $expectedFile = Join-Path $expectedDir ($componentName + '.md')

    $found = $false

    if (Test-Path $expectedFile) {
        $found = $true
    } else {
        # fallback checks: exact filename anywhere in docs list, or basename match
        $componentLower = $componentName.ToLowerInvariant()
        foreach ($p in $docPathsLower) {
            if ($p -like "*\\$($componentLower).md" -or $p -like "*$($componentLower).md") { $found = $true; break }
            # also allow module+component suffix match
            $expectedSuffix = (Join-Path $moduleLower ($componentLower + '.md')).ToLowerInvariant()
            if ($p -like "*$expectedSuffix") { $found = $true; break }
        }
    }

    if (-not $found) {
        $expectedExists = Test-Path $expectedFile
        $docSample = $docPathsLower | Select-Object -First 50
        $missing += [PSCustomObject]@{
            Component = $componentName
            RazorPath = $r.FullName
            ExpectedDoc = $expectedFile
            ExpectedExists = $expectedExists
            DocPathsSample = $docSample
        }
    }
}

if ($missing.Count -eq 0) {
    Write-Host "All .razor components have a corresponding .md (deterministic check)." -ForegroundColor Green
} else {
    Write-Host "Components missing docs: $($missing.Count)" -ForegroundColor Yellow
    $missing | Format-Table -AutoSize
}

# write JSON report
$reportPath = Join-Path $toolsDir 'missing_component_docs.json'
$missing | ConvertTo-Json -Depth 4 | Out-File -FilePath $reportPath -Encoding utf8
Write-Host "JSON report written to: $reportPath"
