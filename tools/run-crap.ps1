#Requires -Version 5.1
<#
.SYNOPSIS
    Measures CRAP scores for every source project and writes one report per project to docs/crap.

.DESCRIPTION
    CRAP(m) = CC^2 * (1 - cov)^3 + CC.

    Cyclomatic complexity comes from crap4dotnet (pinned in .config/dotnet-tools.json). Coverage does not:
    crap4dotnet 0.1.1 maps cobertura data to methods unreliably (it reports 0% for methods in fully covered
    files, and multiple coverage files make it worse). Instead, the coverlet reports of every test project
    are merged (a line counts as covered when any test project hit it) and each method's coverage is the
    share of covered coverable lines between its first line and the next member reported in the same file.
    This is line coverage, not branch coverage, so scores close to the threshold are approximate.

.PARAMETER CoverageDirectory
    Where coverlet results are written (or read with -SkipTests). Searched recursively for coverage.cobertura.xml.
    The directory is deleted and recreated before a test run, so point it at a folder used only for this.

.PARAMETER SkipTests
    Reuse the coverage already in CoverageDirectory instead of running the tests.

.PARAMETER Project
    Analyze only this source project (for example Osirion.Blazor.Core). Coverage still comes from every report found.

.PARAMETER RecordSnapshot
    Append a snapshot to docs/crap/.progress-history.json and regenerate docs/crap/PROGRESS.md.
    Use it when a step of the coverage plan is complete; CI runs without it.

.NOTES
    Runs on Windows PowerShell 5.1 and PowerShell 7; CI uses pwsh on Ubuntu.

.EXAMPLE
    ./tools/run-crap.ps1
    ./tools/run-crap.ps1 -CoverageDirectory ./coverage -SkipTests
#>
[CmdletBinding()]
param(
    [string] $CoverageDirectory = 'TestResults/crap-coverage',
    [switch] $SkipTests,
    [string] $Project,
    [double] $Threshold = 30,
    [switch] $RecordSnapshot
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$reportDirectory = Join-Path $repositoryRoot 'docs/crap'
$excludedFile = Join-Path $reportDirectory 'excluded.txt'
$historyFile = Join-Path $reportDirectory '.progress-history.json'
$runSettings = Join-Path $repositoryRoot 'coverlet.runsettings'
$endToEndTestProject = 'Osirion.Blazor.E2ETests'

if (-not [System.IO.Path]::IsPathRooted($CoverageDirectory)) {
    $CoverageDirectory = Join-Path $repositoryRoot $CoverageDirectory
}

function Get-ExcludedPatterns {
    if (-not (Test-Path $excludedFile)) { return @() }
    Get-Content $excludedFile | ForEach-Object { $_.Trim() } | Where-Object { $_ -and -not $_.StartsWith('#') }
}

# UTF-8 without a byte order mark and with one final newline, on Windows PowerShell 5.1 and PowerShell 7 alike.
function Write-Utf8File([string] $path, [string] $content) {
    [System.IO.File]::WriteAllText($path, $content.TrimEnd() + "`n", [System.Text.UTF8Encoding]::new($false))
}

# Turns 'src/Osirion.Blazor.Core/Foo.cs' or an absolute path into 'Osirion.Blazor.Core/Foo.cs'.
function Get-PathUnderSourceRoot([string] $path) {
    if (-not [System.IO.Path]::IsPathRooted($path)) { $path = Join-Path $repositoryRoot $path }
    $sourceRoot = [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot 'src')).TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar
    $fullPath = [System.IO.Path]::GetFullPath($path)
    if (-not $fullPath.StartsWith($sourceRoot, [System.StringComparison]::OrdinalIgnoreCase)) { throw "$path is not under $sourceRoot." }
    return $fullPath.Substring($sourceRoot.Length).Replace('\', '/')
}

function Test-Excluded([string] $name, [string[]] $patterns) {
    foreach ($pattern in $patterns) {
        if ($name -like $pattern) { return $true }
    }
    return $false
}

function Invoke-TestsWithCoverage {
    if (Test-Path $CoverageDirectory) { Remove-Item $CoverageDirectory -Recurse -Force }
    $testProjects = Get-ChildItem (Join-Path $repositoryRoot 'tests') -Filter '*.csproj' -Recurse |
        Where-Object { $_.BaseName -ne $endToEndTestProject }

    foreach ($testProject in $testProjects) {
        Write-Host "Testing $($testProject.BaseName)"
        dotnet test $testProject.FullName --nologo --settings $runSettings --collect 'XPlat Code Coverage' `
            --results-directory (Join-Path $CoverageDirectory $testProject.BaseName)
        if ($LASTEXITCODE -ne 0) { throw "Tests failed in $($testProject.BaseName); CRAP scores need a green run." }
    }
}

# Returns a map of 'Project/relative/path.cs' to a map of line number to the highest hit count in any report.
function Read-MergedLineHits {
    $reports = Get-ChildItem $CoverageDirectory -Filter 'coverage.cobertura.xml' -Recurse
    if (-not $reports) { throw "No coverage.cobertura.xml found under $CoverageDirectory." }

    $hits = [System.Collections.Generic.Dictionary[string, System.Collections.Generic.Dictionary[int, int]]]::new()
    foreach ($report in $reports) {
        [xml] $document = Get-Content $report.FullName -Raw
        $sourceNode = $document.SelectSingleNode('/coverage/sources/source')
        $source = if ($sourceNode) { $sourceNode.InnerText } else { '' }
        foreach ($class in $document.SelectNodes('//class')) {
            $fileName = $class.GetAttribute('filename')
            # Plain string join: Join-Path resolves drives, which fails for a report made on another OS.
            if (-not [System.IO.Path]::IsPathRooted($fileName)) { $fileName = $source.TrimEnd('\', '/') + '/' + $fileName }
            $normalized = $fileName.Replace('\', '/')
            $index = $normalized.LastIndexOf('/src/')
            if ($index -lt 0) { continue }
            $relative = $normalized.Substring($index + 5)

            if (-not $hits.ContainsKey($relative)) {
                $hits[$relative] = [System.Collections.Generic.Dictionary[int, int]]::new()
            }
            $fileHits = $hits[$relative]
            foreach ($line in $class.SelectNodes('lines/line')) {
                $number = [int] $line.GetAttribute('number')
                $count = [int] $line.GetAttribute('hits')
                if (-not $fileHits.ContainsKey($number) -or $fileHits[$number] -lt $count) { $fileHits[$number] = $count }
            }
        }
    }
    return $hits
}

# crap4dotnet 0.1.1 targets .NET 8. Major roll-forward lets it run where only the .NET 9 or 10 runtime is
# installed (as on the CI runner); where .NET 8 is present it is still used. Scoped to the tool call so the
# caller's session is left as it was.
function Invoke-CrapTool {
    $previous = $env:DOTNET_ROLL_FORWARD
    if (-not $previous) { $env:DOTNET_ROLL_FORWARD = 'Major' }
    try { dotnet tool run dotnet-crap @args }
    finally { $env:DOTNET_ROLL_FORWARD = $previous }
}

function Get-Severity([double] $crap) {
    if ($crap -gt 100) { return 'critical' }
    if ($crap -gt 60) { return 'high' }
    if ($crap -gt 30) { return 'elevated' }
    if ($crap -gt 10) { return 'moderate' }
    return 'low'
}

function Get-Median([double[]] $values) {
    if ($values.Count -eq 0) { return 0 }
    $sorted = $values | Sort-Object
    $middle = [int][Math]::Floor($sorted.Count / 2)
    if ($sorted.Count % 2 -eq 1) { return $sorted[$middle] }
    return ($sorted[$middle - 1] + $sorted[$middle]) / 2
}

function Measure-Project([System.IO.DirectoryInfo] $projectDirectory, $lineHits, [string] $complexitySource, [string] $complexityCoverageFile) {
    $complexityFile = New-TemporaryFile
    try {
        # crap4dotnet refuses to run without a coverage file; its own coverage figures are discarded below.
        Invoke-CrapTool analyze $projectDirectory.FullName --coverage $complexityCoverageFile `
            --output $complexityFile.FullName --threshold $Threshold | Out-Null
        # crap4dotnet exits 1 when it finds methods over the threshold; only 2 and above are failures.
        if ($LASTEXITCODE -gt 1) { throw "dotnet-crap failed for $($projectDirectory.Name) (exit $LASTEXITCODE)." }
        $analysis = Get-Content $complexityFile.FullName -Raw | ConvertFrom-Json
    }
    finally {
        Remove-Item $complexityFile.FullName -ErrorAction SilentlyContinue
    }

    $methods = foreach ($method in $analysis.methods) {
        $relativeFile = Get-PathUnderSourceRoot $method.filePath
        [pscustomobject] @{
            namespace    = $method.namespace
            className    = $method.className
            methodName   = $method.methodName
            signature    = $method.signature
            fullName     = $method.fullName
            filePath     = "src/$relativeFile"
            relativeFile = $relativeFile
            lineNumber   = [int] $method.lineNumber
            complexity   = [int] $method.complexity
            coverage     = 0.0
            coverableLines = 0
            crap         = 0.0
            isCrappy     = $false
            severity     = ''
        }
    }

    foreach ($fileGroup in ($methods | Group-Object relativeFile)) {
        $ordered = @($fileGroup.Group | Sort-Object lineNumber)
        $fileHits = if ($lineHits.ContainsKey($fileGroup.Name)) { $lineHits[$fileGroup.Name] } else { $null }
        for ($i = 0; $i -lt $ordered.Count; $i++) {
            $method = $ordered[$i]
            $lastLine = if ($i + 1 -lt $ordered.Count) { $ordered[$i + 1].lineNumber - 1 } else { [int]::MaxValue }
            $coverable = 0
            $covered = 0
            if ($fileHits) {
                foreach ($entry in $fileHits.GetEnumerator()) {
                    if ($entry.Key -ge $method.lineNumber -and $entry.Key -le $lastLine) {
                        $coverable++
                        if ($entry.Value -gt 0) { $covered++ }
                    }
                }
            }
            $coverage = if ($coverable -gt 0) { $covered / $coverable } else { 0.0 }
            $complexity = $method.complexity
            $crap = $complexity * $complexity * [Math]::Pow(1 - $coverage, 3) + $complexity
            $method.coverableLines = $coverable
            $method.coverage = [Math]::Round($coverage, 4)
            $method.crap = [Math]::Round($crap, 2)
            $method.isCrappy = $crap -gt $Threshold
            $method.severity = Get-Severity $crap
        }
    }

    $crapValues = [double[]] @($methods | ForEach-Object crap)
    $crappyCount = @($methods | Where-Object isCrappy).Count
    $average = if ($crapValues.Count) { ($crapValues | Measure-Object -Average).Average } else { 0 }
    $maximum = if ($crapValues.Count) { ($crapValues | Measure-Object -Maximum).Maximum } else { 0 }

    [pscustomobject] @{
        schemaVersion      = '1.0'
        project            = $projectDirectory.Name
        timestamp          = [DateTimeOffset]::UtcNow.ToString('o')
        threshold          = $Threshold
        complexitySource   = $complexitySource
        coverageSource     = 'merged coverlet cobertura, line coverage per method span'
        stats              = [pscustomobject] @{
            methodCount         = $crapValues.Count
            totalCrap           = [Math]::Round(($crapValues | Measure-Object -Sum).Sum, 2)
            averageCrap         = [Math]::Round($average, 2)
            medianCrap          = [Math]::Round((Get-Median $crapValues), 2)
            maxCrap             = [Math]::Round($maximum, 2)
            crappyMethodCount   = $crappyCount
            crappyMethodPercent = if ($crapValues.Count) { [Math]::Round(100 * $crappyCount / $crapValues.Count, 2) } else { 0 }
        }
        methods            = @($methods | Sort-Object crap -Descending | Select-Object -Property * -ExcludeProperty relativeFile)
    }
}

function Get-LineCoverage($lineHits, [string[]] $projectNames) {
    $coverable = 0
    $covered = 0
    foreach ($file in $lineHits.GetEnumerator()) {
        if ($projectNames -notcontains $file.Key.Split('/')[0]) { continue }
        foreach ($hit in $file.Value.Values) {
            $coverable++
            if ($hit -gt 0) { $covered++ }
        }
    }
    [pscustomobject] @{ coverableLines = $coverable; coveredLines = $covered
        lineCoverage = if ($coverable) { [Math]::Round($covered / $coverable, 4) } else { 0 } }
}

function Save-Snapshot($reports, $coverage) {
    $history = @()
    # Piping through ForEach-Object unrolls the array on Windows PowerShell 5.1, which returns it as one object.
    if (Test-Path $historyFile) { $history = @(Get-Content $historyFile -Raw | ConvertFrom-Json | ForEach-Object { $_ }) }
    $snapshot = [pscustomobject] @{
        timestamp      = [DateTimeOffset]::UtcNow.ToString('o')
        commit         = (git -C $repositoryRoot rev-parse --short HEAD)
        threshold      = $Threshold
        lineCoverage   = $coverage.lineCoverage
        coverableLines = $coverage.coverableLines
        coveredLines   = $coverage.coveredLines
        projects       = @($reports | ForEach-Object {
            [pscustomobject] @{
                project           = $_.project
                methodCount       = $_.stats.methodCount
                crappyMethodCount = $_.stats.crappyMethodCount
                maxCrap           = $_.stats.maxCrap
                averageCrap       = $_.stats.averageCrap
            } })
    }
    $history += $snapshot
    # One compressed snapshot per line keeps the append-only history diff to a single added line.
    $entries = $history | ForEach-Object { '  ' + (ConvertTo-Json -InputObject $_ -Depth 6 -Compress) }
    Write-Utf8File $historyFile ("[`n" + ($entries -join ",`n") + "`n]")
    Write-ProgressReport $history
}

function Write-ProgressReport($history) {
    $latest = $history[-1]
    $previous = if ($history.Count -gt 1) { $history[-2] } else { $null }
    $lines = [System.Collections.Generic.List[string]]::new()
    $lines.Add('# CRAP progress')
    $lines.Add('')
    $lines.Add('Generated by `tools/run-crap.ps1 -RecordSnapshot`. Do not edit by hand.')
    $lines.Add('')
    $lines.Add("Latest snapshot: $($latest.timestamp) at commit ``$($latest.commit)``, threshold $($latest.threshold).")
    $lines.Add('')
    $lines.Add("Line coverage: **$([Math]::Round(100 * $latest.lineCoverage, 1))%** ($($latest.coveredLines) of $($latest.coverableLines) lines).")
    $lines.Add('')
    $lines.Add('| Project | Methods | Over threshold | Change | Max CRAP | Average CRAP |')
    $lines.Add('|---|---:|---:|---:|---:|---:|')
    foreach ($entry in $latest.projects) {
        $before = if ($previous) { $previous.projects | Where-Object project -eq $entry.project } else { $null }
        $change = if ($before) { $entry.crappyMethodCount - $before.crappyMethodCount } else { 0 }
        $lines.Add("| $($entry.project) | $($entry.methodCount) | $($entry.crappyMethodCount) | $change | $($entry.maxCrap) | $($entry.averageCrap) |")
    }
    $lines.Add('')
    $lines.Add('## Trend (last 12 snapshots)')
    $lines.Add('')
    $lines.Add('| Snapshot | Commit | Line coverage | Methods over threshold |')
    $lines.Add('|---|---|---:|---:|')
    foreach ($entry in ($history | Select-Object -Last 12)) {
        $total = ($entry.projects | Measure-Object crappyMethodCount -Sum).Sum
        $lines.Add("| $($entry.timestamp) | ``$($entry.commit)`` | $([Math]::Round(100 * $entry.lineCoverage, 1))% | $total |")
    }
    Write-Utf8File (Join-Path $reportDirectory 'PROGRESS.md') ($lines -join "`n")
}

if (-not $SkipTests) { Invoke-TestsWithCoverage }

dotnet tool restore | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'dotnet tool restore failed.' }

$lineHits = Read-MergedLineHits
$excluded = @(Get-ExcludedPatterns)
$projects = Get-ChildItem (Join-Path $repositoryRoot 'src') -Directory |
    Where-Object { (Test-Path (Join-Path $_.FullName "$($_.Name).csproj")) -and -not (Test-Excluded $_.Name $excluded) } |
    Where-Object { -not $Project -or $_.Name -eq $Project }
if (-not $projects) { throw "No source project matched '$Project'." }

New-Item $reportDirectory -ItemType Directory -Force | Out-Null
$crapToolVersion = Invoke-CrapTool --version
if ($LASTEXITCODE -ne 0 -or -not $crapToolVersion) { throw 'dotnet-crap did not start; run dotnet tool restore and check the .NET runtime.' }
$complexitySource = 'crap4dotnet ' + "$crapToolVersion".Split('+')[0]
$anyCoverageFile = (Get-ChildItem $CoverageDirectory -Filter 'coverage.cobertura.xml' -Recurse | Select-Object -First 1).FullName
$reports = foreach ($projectDirectory in $projects) {
    Write-Host "Analyzing $($projectDirectory.Name)"
    $report = Measure-Project $projectDirectory $lineHits $complexitySource $anyCoverageFile
    Write-Utf8File (Join-Path $reportDirectory "$($projectDirectory.Name).json") (ConvertTo-Json -InputObject $report -Depth 5)
    $report
}

$coverage = Get-LineCoverage $lineHits @($projects | ForEach-Object Name)
$reports | ForEach-Object {
    '{0,-36} methods {1,5}  over {2,3}  max {3,7}  avg {4,6}' -f $_.project, $_.stats.methodCount,
        $_.stats.crappyMethodCount, $_.stats.maxCrap, $_.stats.averageCrap
} | Write-Host
Write-Host ("Line coverage {0:P1} ({1} of {2} lines)" -f $coverage.lineCoverage, $coverage.coveredLines, $coverage.coverableLines)

if ($RecordSnapshot) {
    if ($Project) { throw '-RecordSnapshot needs the full solution; drop -Project.' }
    Save-Snapshot $reports $coverage
}
