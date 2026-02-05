param(
    [string]$UnityPath = "C:\Program Files\Unity\Hub\Editor\2022.3.20f1\Editor\Unity.exe",
    [string]$ProjectPath = "$(Resolve-Path ..)",
    [string]$BuildPath = "$(Resolve-Path ..)\Builds\Windows"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $UnityPath)) {
    throw "Unity.exe bulunamadı: $UnityPath"
}

if (-not (Test-Path $BuildPath)) {
    New-Item -ItemType Directory -Path $BuildPath | Out-Null
}

$logFile = Join-Path $BuildPath "build.log"
$executeMethod = "FootballSim.Build.BuildPipeline.BuildWindows"

& $UnityPath `
    -batchmode `
    -nographics `
    -quit `
    -projectPath $ProjectPath `
    -executeMethod $executeMethod `
    -logFile $logFile

Write-Host "Build tamamlandı: $BuildPath"
