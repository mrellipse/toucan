rm -r -d ../dist/*
cd ../src/server
dotnet publish server.csproj -f netcoreapp1.1 -o ../../dist --configuration Release
cd ../ui
webpack -p --config webpack.production.js --env.outputPath=../../dist/wwwroot
cd ../../build