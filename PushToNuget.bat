SET /p ApiKey=<NugetApiKey.txt

cd nupkg

for /r %%i in (*.nupkg) do (
    dotnet nuget push %%i --api-key %ApiKey% -s https://api.nuget.org/v3/index.json
)
REM for /r %%i in (*.snupkg) do (
REM    dotnet nuget push %%i --api-key %ApiKey% -s https://api.nuget.org/v3/index.json
REM )

cd ..