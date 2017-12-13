Push-Location ../server
dotnet publish server.csproj -f netcoreapp2.0 -o ../../dist --configuration Release
Pop-Location