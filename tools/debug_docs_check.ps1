$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition)
$src = Join-Path $root 'src'
$docs = Join-Path $root 'docs\components'
$docFiles = @()
if (Test-Path $docs) { $docFiles += Get-ChildItem -Path $docs -Recurse -File -Filter *.md }
$docFiles += Get-ChildItem -Path $src -Recurse -File -Filter *.md | Where-Object { $_.FullName -notmatch '\\bin\\' -and $_.FullName -notmatch '\\obj\\' }
$docBasenames = $docFiles | ForEach-Object { [System.IO.Path]::GetFileNameWithoutExtension($_.Name).ToLowerInvariant() } | Sort-Object -Unique
$razorFiles = Get-ChildItem -Path $src -Recurse -File -Filter *.razor | Where-Object { $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\bin\\' }
$missing = @()
foreach ($r in $razorFiles) {
    $component = [System.IO.Path]::GetFileNameWithoutExtension($r.Name)
    if ($component -eq '_Imports') { continue }
    $c = $component.ToLowerInvariant()
    $found = $false
    foreach ($d in $docBasenames) {
        if ($d -eq $c -or $d -like "*$c*" -or $d -like "*.$c*") { $found = $true; break }
    }
    if (-not $found) { $missing += $component }
}
$out = "--- doc basenames ---`r`n"
$out += ($docBasenames -join "`r`n")
$out += "`r`n--- missing components ---`r`n"
$out += ($missing -join "`r`n")
$out += "`r`nMissing count: $($missing.Count)`r`n"
$report = Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Definition) 'debug_docs_output.txt'
Set-Content -Path $report -Value $out -Encoding utf8
Write-Host "Wrote debug output to: $report"
