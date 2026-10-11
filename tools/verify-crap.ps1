#Requires -Version 5.1
<#
.SYNOPSIS
    CRAP gate: checks the reports written by tools/run-crap.ps1 against a threshold and against the last
    recorded snapshot.

.DESCRIPTION
    Fails (exit 1) when any method's CRAP score exceeds -MaxCrap, when a project's average exceeds -MaxAverage
    (if given), or when a project has more methods over the threshold than in the last snapshot in
    docs/crap/.progress-history.json (a net regression). With -ReportOnly it prints the same findings and
    exits 0. Writes a Markdown summary to $env:GITHUB_STEP_SUMMARY when that variable is set.

.NOTES
    Runs on Windows PowerShell 5.1 and PowerShell 7; CI uses pwsh on Ubuntu.

.EXAMPLE
    ./tools/verify-crap.ps1 -MaxCrap 30
    ./tools/verify-crap.ps1 -MaxCrap 30 -ReportOnly
#>
[CmdletBinding()]
param(
    [double] $MaxCrap = 30,
    [double] $MaxAverage = 0,
    [switch] $ReportOnly
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$reportDirectory = Join-Path $repositoryRoot 'docs/crap'
$historyFile = Join-Path $reportDirectory '.progress-history.json'
$listedOffenders = 25

$reports = @(Get-ChildItem $reportDirectory -Filter '*.json' |
    Where-Object { $_.FullName -ne $historyFile } |
    ForEach-Object { Get-Content $_.FullName -Raw | ConvertFrom-Json })
if (-not $reports) { throw "No CRAP reports in $reportDirectory; run tools/run-crap.ps1 first." }

# Piping through ForEach-Object unrolls the array on Windows PowerShell 5.1, which returns it as one object.
$history = @()
if (Test-Path $historyFile) { $history = @(Get-Content $historyFile -Raw | ConvertFrom-Json | ForEach-Object { $_ }) }
$baseline = if ($history.Count) { $history[-1] } else { $null }

$violations = [System.Collections.Generic.List[string]]::new()
$offenders = @($reports | ForEach-Object {
        $projectName = $_.project
        $_.methods | Where-Object crap -gt $MaxCrap | ForEach-Object {
            [pscustomobject] @{ project = $projectName; method = $_.fullName; complexity = $_.complexity
                coverage = $_.coverage; crap = $_.crap; location = "$($_.filePath):$($_.lineNumber)" }
        }
    } | Sort-Object crap -Descending)
if ($offenders.Count) { $violations.Add("$($offenders.Count) method(s) exceed CRAP $MaxCrap.") }

# The regression check compares counts taken at the snapshot's own threshold, whatever -MaxCrap is.
$baselineThreshold = if ($baseline) { [double] $baseline.threshold } else { 0 }
$rows = foreach ($report in ($reports | Sort-Object project)) {
    $overThreshold = @($report.methods | Where-Object crap -gt $MaxCrap).Count
    $before = if ($baseline) { $baseline.projects | Where-Object project -eq $report.project } else { $null }
    $change = 0
    if ($before) {
        $overBaselineThreshold = @($report.methods | Where-Object crap -gt $baselineThreshold).Count
        $change = $overBaselineThreshold - $before.crappyMethodCount
    }
    if ($change -gt 0) {
        $violations.Add("$($report.project): $change more method(s) over CRAP $baselineThreshold than the last snapshot.")
    }
    if ($MaxAverage -gt 0 -and $report.stats.averageCrap -gt $MaxAverage) {
        $violations.Add("$($report.project): average CRAP $($report.stats.averageCrap) exceeds $MaxAverage.")
    }
    [pscustomobject] @{ project = $report.project; methods = $report.stats.methodCount; over = $overThreshold
        change = $change; max = $report.stats.maxCrap; average = $report.stats.averageCrap }
}

$summary = [System.Collections.Generic.List[string]]::new()
$summary.Add("## CRAP gate (max $MaxCrap)")
$summary.Add('')
$summary.Add("| Project | Methods | Over $MaxCrap | Change vs snapshot (over $baselineThreshold) | Max | Average |")
$summary.Add('|---|---:|---:|---:|---:|---:|')
$rows | ForEach-Object { $summary.Add("| $($_.project) | $($_.methods) | $($_.over) | $($_.change) | $($_.max) | $($_.average) |") }
if ($offenders.Count) {
    $summary.Add('')
    $summary.Add("### Top offenders (first $listedOffenders of $($offenders.Count))")
    $summary.Add('')
    $summary.Add('| CRAP | CC | Coverage | Method | Location |')
    $summary.Add('|---:|---:|---:|---|---|')
    $offenders | Select-Object -First $listedOffenders | ForEach-Object {
        $summary.Add("| $($_.crap) | $($_.complexity) | $([Math]::Round(100 * $_.coverage))% | ``$($_.method)`` | $($_.location) |")
    }
}
$summary.Add('')
$summary.Add($(if ($violations.Count) { "**Result:** $($violations.Count) violation(s)$(if ($ReportOnly) { ', report only' })." } else { '**Result:** pass.' }))

$summary | Write-Host
if ($env:GITHUB_STEP_SUMMARY) { $summary | Add-Content $env:GITHUB_STEP_SUMMARY -Encoding utf8 }

foreach ($violation in $violations) {
    if ($ReportOnly) { Write-Warning $violation }
    elseif ($env:GITHUB_ACTIONS) { Write-Host "::error::$violation" }
    else { Write-Host "ERROR: $violation" -ForegroundColor Red }
}
if ($violations.Count -and -not $ReportOnly) { exit 1 }
exit 0
