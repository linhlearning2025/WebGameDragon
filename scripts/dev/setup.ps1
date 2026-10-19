$ErrorActionPreference = 'Stop'

$repositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
Push-Location $repositoryRoot
try {
    $expectedNode = (Get-Content '.node-version' -Raw).Trim()
    $actualNode = (& node --version).Trim().TrimStart('v')
    if ($actualNode -ne $expectedNode) {
        throw "Node.js $expectedNode is required; found $actualNode."
    }

    $packageManifest = Get-Content 'package.json' -Raw | ConvertFrom-Json
    $expectedPnpm = ($packageManifest.packageManager -split '@')[-1]
    $actualPnpm = (& pnpm --version).Trim()
    if ($actualPnpm -ne $expectedPnpm) {
        throw "pnpm $expectedPnpm is required; found $actualPnpm."
    }

    $expectedDotnet = (Get-Content 'global.json' -Raw | ConvertFrom-Json).sdk.version
    $actualDotnet = (& dotnet --version).Trim()
    if ($actualDotnet -ne $expectedDotnet) {
        throw ".NET SDK $expectedDotnet is required; found $actualDotnet."
    }

    if ([string]::IsNullOrWhiteSpace($env:ConnectionStrings__Default)) {
        throw 'Required environment variable ConnectionStrings__Default is missing.'
    }

    if ([string]::IsNullOrWhiteSpace($env:Privacy__AuditPseudonymKey)) {
        throw 'Required environment variable Privacy__AuditPseudonymKey is missing.'
    }

    & pnpm install --frozen-lockfile
    if ($LASTEXITCODE -ne 0) { throw 'pnpm dependency restore failed.' }

    & dotnet tool restore
    if ($LASTEXITCODE -ne 0) { throw '.NET local tool restore failed.' }

    & dotnet restore 'apps/api/tests/DragonEnglishKids.Api.Tests/DragonEnglishKids.Api.Tests.csproj' --locked-mode
    if ($LASTEXITCODE -ne 0) { throw '.NET dependency restore failed.' }

    Write-Host 'Prerequisites validated and dependencies restored.'
    Write-Host 'Apply migrations with: pnpm db:migrate'
    Write-Host 'Start the API with: pnpm dev:api'
    Write-Host 'Start the web app with: pnpm dev:web'
    Write-Host 'Start the content tool with: pnpm dev:content'
} finally {
    Pop-Location
}