@echo off
REM Releases file locks held by MSBuild (e.g. on the build task DLLs in lib\WebFormsForCore.Build).
REM Close Visual Studio first if it is open on this solution; it hosts its own MSBuild nodes.

echo Shutting down .NET build servers...
dotnet build-server shutdown >nul 2>&1

echo Killing msbuild.exe processes...
taskkill /F /T /IM msbuild.exe >nul 2>&1

echo Killing dotnet.exe processes running MSBuild.dll (reused MSBuild nodes)...
powershell -NoProfile -Command "Get-CimInstance Win32_Process -Filter \"Name = 'dotnet.exe'\" | Where-Object { $_.CommandLine -match 'msbuild\.dll' } | ForEach-Object { Write-Host ('  killing PID ' + $_.ProcessId); Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }"

echo Done.
