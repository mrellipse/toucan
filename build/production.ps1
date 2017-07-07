Push-Location ../server
dotnet publish server.csproj -f netcoreapp1.1 -o ../../dist --configuration Release
Pop-Location
Push-Location ../ui
webpack -p --config webpack.production.js
Pop-Location