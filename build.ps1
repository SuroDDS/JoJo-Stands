param(
    [Parameter(Mandatory=$true)]
    [string]$RimWorldDir
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'Source/JoJoStand/JoJoStand.csproj'
if (-not (Test-Path (Join-Path $RimWorldDir 'Assembly-CSharp.dll'))) {
    throw "Could not find Assembly-CSharp.dll in: $RimWorldDir. Use the RimWorld 1.7 game root."
}
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'Install .NET SDK 10, then run this script again.'
}

dotnet build $project -c Release "-p:RimWorldDir=$RimWorldDir"
if ($LASTEXITCODE -ne 0) { throw 'Build failed. See compiler output above.' }
Write-Host 'Built 1.7/Assemblies/JoJoStand.dll.'
Write-Host 'Copy this mod folder to your RimWorld/Mods directory and enable Harmony, Biotech, then JoJo Stand System.'
