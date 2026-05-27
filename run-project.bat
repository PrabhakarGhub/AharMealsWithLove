@echo off
set DOTNET_CLI_HOME=%~dp0..\.dotnet_home
set DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
set DOTNET_CLI_TELEMETRY_OPTOUT=1
set APPDATA=%~dp0..\.appdata
set LOCALAPPDATA=%~dp0..\.localappdata

echo Starting AHAR - Meals with Love...
echo Open http://localhost:5098 in your browser.
"C:\Program Files\dotnet\dotnet.exe" run --urls http://localhost:5098
pause
