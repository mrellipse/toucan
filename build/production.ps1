Push-Location ../server
dotnet publish server.csproj -f netcoreapp2.1 -o ../../dist --configuration Release
Pop-Location