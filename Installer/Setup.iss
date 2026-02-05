#define AppName "FootballSim"
#define AppVersion "0.1.0"
#define AppPublisher "Studio X"
#define AppExeName "FootballSim.exe"

[Setup]
AppId={{E3E5D2D8-9A7E-4E4C-9F6C-2B7B8C2A7E1F}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
OutputDir=..\Builds\Installer
OutputBaseFilename=FootballSim-Setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"

[Files]
Source: "..\Builds\Windows\{#AppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Builds\Windows\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{group}\Kaldir {#AppName}"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall skipifsilent
