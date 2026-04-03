#define MyAppName "Open Exam Suite"
#define MyAppPublisher "Open Exam Suite"
#define MyAppExeName "OpenExamSuite.Simulator.exe"
#ifndef MyAppVersion
#define MyAppVersion "0.0.0"
#endif

[Setup]
AppId={{B4C7E2F1-8A3D-4B9E-9C2D-1E5F6A7B8C90}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=Output
OutputBaseFilename=OpenExamSuite-{#MyAppVersion}-Setup-x64
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
LicenseFile=..\LICENSE
DisableProgramGroupPage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Types]
Name: "full"; Description: "Full installation"
Name: "custom"; Description: "Custom installation"; Flags: iscustom

[Components]
Name: "simulator"; Description: "Simulator (required)"; Types: full custom; Flags: fixed
Name: "creator"; Description: "Creator"; Types: full
Name: "samples"; Description: "Sample exams"; Types: full

[Files]
Source: "..\build\installer\staging\Simulator\*"; DestDir: "{app}\Simulator"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: simulator
Source: "..\build\installer\staging\Creator\*"; DestDir: "{app}\Creator"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: creator
Source: "..\build\installer\staging\Samples\*"; DestDir: "{app}\Samples"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: samples

[Icons]
Name: "{group}\Open Exam Suite Simulator"; Filename: "{app}\Simulator\{#MyAppExeName}"; Components: simulator
Name: "{group}\Open Exam Suite Creator"; Filename: "{app}\Creator\OpenExamSuite.Creator.exe"; Components: creator
Name: "{autodesktop}\Open Exam Suite Simulator"; Filename: "{app}\Simulator\{#MyAppExeName}"; Tasks: desktopsimulator

[Tasks]
Name: "desktopsimulator"; Description: "Create a &desktop shortcut for Simulator"; GroupDescription: "Additional shortcuts:"; Components: simulator

[Registry]
Root: HKCR; Subkey: ".oef"; ValueType: string; ValueName: ""; ValueData: "OpenExamSuite.oef"; Flags: uninsdeletevalue
Root: HKCR; Subkey: "OpenExamSuite.oef"; ValueType: string; ValueName: ""; ValueData: "Open Exam Suite Exam"; Flags: uninsdeletekey
Root: HKCR; Subkey: "OpenExamSuite.oef\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Simulator\{#MyAppExeName},0"
Root: HKCR; Subkey: "OpenExamSuite.oef\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Simulator\{#MyAppExeName}"" ""%1"""

Root: HKCR; Subkey: ".json"; ValueType: string; ValueName: ""; ValueData: "OpenExamSuite.json"; Flags: uninsdeletevalue
Root: HKCR; Subkey: "OpenExamSuite.json"; ValueType: string; ValueName: ""; ValueData: "Open Exam Suite Document"; Flags: uninsdeletekey
Root: HKCR; Subkey: "OpenExamSuite.json\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Simulator\{#MyAppExeName},0"
Root: HKCR; Subkey: "OpenExamSuite.json\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Simulator\{#MyAppExeName}"" ""%1"""

Root: HKCR; Subkey: ".xml"; ValueType: string; ValueName: ""; ValueData: "OpenExamSuite.xml"; Flags: uninsdeletevalue
Root: HKCR; Subkey: "OpenExamSuite.xml"; ValueType: string; ValueName: ""; ValueData: "Open Exam Suite Document"; Flags: uninsdeletekey
Root: HKCR; Subkey: "OpenExamSuite.xml\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Simulator\{#MyAppExeName},0"
Root: HKCR; Subkey: "OpenExamSuite.xml\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Simulator\{#MyAppExeName}"" ""%1"""
