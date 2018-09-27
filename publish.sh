
rm -rf ./bin/Release
dotnet publish -o ./bin/Release --self-contained -r win-x64 -f netcoreapp2.1
