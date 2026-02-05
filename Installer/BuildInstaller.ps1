param(
    [string]$InnoPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
    [string]$ScriptPath = "$(Resolve-Path .)\Setup.iss"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $InnoPath)) {
    throw "ISCC.exe bulunamadı: $InnoPath"
}

& $InnoPath $ScriptPath
Write-Host "Installer üretildi: ..\Builds\Installer"
