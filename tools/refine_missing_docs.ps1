# Read existing missing report and add diagnostics
$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition)
$toolsDir = Join-Path $root 'tools'
$srcPath = Join-Path $root 'src'
$docsPath = Join-Path $root 'docs\components'
$inReport = Join-Path $toolsDir 'missing_component_docs.json'
$outReport = Join-Path $toolsDir 'missing_component_docs_refined.json'

if (-not (Test-Path $inReport)) { Write-Host "Input report not found: $inReport"; exit 1 }

$items = Get-Content $inReport -Raw | ConvertFrom-Json
$allMd = @()
if (Test-Path $docsPath) { $allMd += Get-ChildItem -Path $docsPath -Recurse -File -Filter *.md }
$allMd += Get-ChildItem -Path $srcPath -Recurse -File -Filter *.md | Where-Object { $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\bin\\' }
$allMdPaths = $allMd | ForEach-Object { $_.FullName }

$results = @()
foreach ($it in $items) {
    $rp = $it.RazorPath
    $component = $it.Component
    $expected = $null
    $expectedExists = $false
    $matched = @()

    if ($rp) {
        # compute relative to src if possible
        if ($rp.ToLowerInvariant().StartsWith($srcPath.ToLowerInvariant())) {
            $rel = $rp.Substring($srcPath.Length).TrimStart('\','/')
            $parts = $rel -split '[\\/]'
            $projectFolder = $parts[0]
            $module = ($projectFolder -replace '^Osirion\.Blazor\.', '').ToLowerInvariant()
            $expected = Join-Path $docsPath (Join-Path $module ($component + '.md'))
            $expectedExists = Test-Path $expected
        }
    }

    # search for any matching md by basename anywhere
    $componentLower = $component.ToLowerInvariant()
    foreach ($p in $allMdPaths) {
        if ($p.ToLowerInvariant().EndsWith("\\$componentLower.md") -or $p.ToLowerInvariant().EndsWith("/$componentLower.md") -or $p.ToLowerInvariant().Contains("/" + $componentLower + ".md")) {
            $matched += $p
        }
        elseif ($p.ToLowerInvariant().Contains($componentLower)) {
            # include looser matches as well
            $matched += $p
        }
    }
    $matched = $matched | Sort-Object -Unique

    $results += [PSCustomObject]@{
        Component = $component
        RazorPath = $rp
        ExpectedDoc = $expected
        ExpectedExists = $expectedExists
        MatchedDocs = $matched
    }
}

$results | ConvertTo-Json -Depth 6 | Out-File -FilePath $outReport -Encoding utf8
Write-Host "Refined report written to: $outReport"
