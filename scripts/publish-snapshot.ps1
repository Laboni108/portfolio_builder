<#
  publish-snapshot.ps1

  Exports the public home page of the portfolio as static files and publishes
  them to GitHub Pages (the gh-pages branch of a separate clone).

  Run it from the repo root with:
    powershell -ExecutionPolicy Bypass -File .\scripts\publish-snapshot.ps1

  What it does:
    1. Starts the app on a spare port, using your normal database.
    2. Downloads the home page HTML.
    3. Fixes the HTML for static hosting (relative paths, Formspree contact
       form, no admin link, no anti-forgery token).
    4. Copies css, js, images and files from wwwroot.
    5. Commits and pushes the result to the gh-pages branch.
#>

param(
    [int]$Port = 5299,
    [string]$FormspreeUrl = 'https://formspree.io/f/meewjnzv',
    [string]$PublishDir = ''
)

$ErrorActionPreference = 'Stop'

$repo    = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$project = Join-Path $repo 'src\PortfolioBuilder.Web'
$wwwroot = Join-Path $project 'wwwroot'

if ([string]::IsNullOrWhiteSpace($PublishDir)) {
    $PublishDir = Join-Path (Split-Path $repo -Parent) 'portfolio-pages'
}

# ---------- safety checks ----------

if (-not (Test-Path (Join-Path $PublishDir '.git'))) {
    throw "Publish folder not found or not a git clone: $PublishDir`nDo the one-time setup first."
}

if (Get-Process -Name 'PortfolioBuilder.Web' -ErrorAction SilentlyContinue) {
    throw 'The portfolio app is already running. Stop it with Ctrl+C in its terminal, then run this script again.'
}

if (Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue) {
    throw "Port $Port is already in use. Close whatever uses it, or run with -Port 5300."
}

# ---------- start the app and download the page ----------

$logOut = Join-Path $env:TEMP 'portfolio-export.out.log'
$logErr = Join-Path $env:TEMP 'portfolio-export.err.log'
$url    = "http://localhost:$Port/"

$env:ASPNETCORE_ENVIRONMENT = 'Development'

Write-Host "Starting the app on port $Port (the first build can take a minute)..."

$startArgs = @{
    FilePath               = 'dotnet'
    ArgumentList           = "run --no-launch-profile --urls http://localhost:$Port"
    WorkingDirectory       = $project
    PassThru               = $true
    WindowStyle            = 'Hidden'
    RedirectStandardOutput = $logOut
    RedirectStandardError  = $logErr
}
$proc = Start-Process @startArgs

try {
    $ready = $false
    for ($i = 0; $i -lt 120; $i++) {
        if ($proc.HasExited) { break }
        try {
            $r = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 3
            if ($r.StatusCode -eq 200) { $ready = $true; break }
        }
        catch { }
        Start-Sleep -Seconds 1
    }

    if (-not $ready) {
        Write-Host '--- app output ---'
        if (Test-Path $logOut) { Get-Content $logOut -Tail 20 }
        if (Test-Path $logErr) { Get-Content $logErr -Tail 20 }
        throw 'The app did not start. See the output above.'
    }

    Write-Host 'App is up. Downloading the home page...'
    $wc = New-Object System.Net.WebClient
    $wc.Encoding = [System.Text.Encoding]::UTF8
    $html = $wc.DownloadString($url)
}
finally {
    if ($proc -and -not $proc.HasExited) {
        & taskkill /PID $proc.Id /T /F | Out-Null
    }
}

# ---------- fix the HTML for static hosting ----------

Write-Host 'Fixing paths and the contact form for static hosting...'

# remove the Admin link from the footer
$html = [regex]::Replace($html, '(?s)<a\s+href="/Admin"[^>]*>.*?</a>\s*', '')

# send the contact form to Formspree instead of the (offline) database
$html = [regex]::Replace($html, 'action="[^"]*handler=Contact[^"]*"', ('action="' + $FormspreeUrl + '"'))
$html = [regex]::Replace($html, '<input[^>]*__RequestVerificationToken[^>]*>\s*', '')
$html = $html.Replace('name="Contact.Name"',    'name="name" required')
$html = $html.Replace('name="Contact.Email"',   'name="email" required')
$html = $html.Replace('name="Contact.Subject"', 'name="subject"')
$html = $html.Replace('name="Contact.Message"', 'name="message" required')

# make root-relative links relative, so the site works under /portfolio_builder/
$html = [regex]::Replace($html, '(href|src)="/(?!/)', '$1="')

# .NET adds a fingerprint to asset names (css/portfolio.abc123xyz.css,
# images/pc.abc123xyz.gif, ...), but the files we copy have their normal names,
# so remove the fingerprint from the links
$html = [regex]::Replace($html, '((?:href|src)="(?:css|js|images|files)/[^"/]*?)\.[a-z0-9]{8,12}(\.[a-z0-9]+)(?=["?#])', '$1$2')

# refuse to publish something broken
if ($html.Length -lt 5000) {
    throw 'The downloaded page looks too small. Aborting so nothing broken gets published.'
}
if ($html -notmatch [regex]::Escape($FormspreeUrl)) {
    throw 'Could not find the contact form to rewrite. Aborting.'
}
if ($html -match '__RequestVerificationToken') {
    throw 'The anti-forgery token was not removed. Aborting.'
}
if ($html -match '(href|src)="/(?!/)') {
    throw 'Some root-relative links are left. Aborting.'
}

# ---------- write the files ----------

Write-Host "Preparing $PublishDir ..."

Get-ChildItem -Path $PublishDir -Force |
    Where-Object { $_.Name -ne '.git' } |
    Remove-Item -Recurse -Force

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText((Join-Path $PublishDir 'index.html'), $html, $utf8NoBom)

foreach ($folder in 'css', 'js', 'images', 'files') {
    $from = Join-Path $wwwroot $folder
    if (Test-Path $from) {
        Copy-Item -Path $from -Destination (Join-Path $PublishDir $folder) -Recurse -Force
    }
}

New-Item -ItemType File -Path (Join-Path $PublishDir '.nojekyll') -Force | Out-Null

# every local file the page links to must exist in the publish folder
$missing = @()
foreach ($m in [regex]::Matches($html, '(?:href|src)="((?:css|js|images|files)/[^"?#]+)')) {
    $rel = $m.Groups[1].Value
    if (-not (Test-Path (Join-Path $PublishDir $rel))) { $missing += $rel }
}
if ($missing.Count -gt 0) {
    throw ('The page links to files that were not copied, so nothing was published: ' + ($missing -join ', '))
}

# ---------- commit and push ----------

Write-Host 'Committing the snapshot...'

& git -C $PublishDir add -A
$changes = & git -C $PublishDir status --porcelain

if (-not $changes) {
    Write-Host 'Nothing changed since the last publish. The live site is already up to date.'
    return
}

$stamp = Get-Date -Format 'yyyy-MM-dd HH:mm'
& git -C $PublishDir commit -m "snapshot $stamp"
if ($LASTEXITCODE -ne 0) { throw 'git commit failed.' }

& git -C $PublishDir push origin gh-pages
if ($LASTEXITCODE -ne 0) { throw 'git push failed.' }

Write-Host ''
Write-Host 'Published! It can take a minute or two to appear at:'
Write-Host 'https://laboni108.github.io/portfolio_builder/'