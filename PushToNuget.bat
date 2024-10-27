SET /p ApiKey=<NugetApiKey.txt

cd nuget

for /r %i in (*.nupkg) do dotnet nuget push %i --api-key %ApiKey%
for /r %i in (*.snupkg) do dotnet nuget push %i --api-key %ApiKey% 

cd ..