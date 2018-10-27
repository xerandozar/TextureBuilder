# TextureBuilder

## Build

https://docs.microsoft.com/en-us/dotnet/core/rid-catalog

~~~~
dotnet clean
dotnet restore
dotnet publish -c release -f netcoreapp2.1 -r win7-x64 -o bin/Publish/win-x64 --self-contained
dotnet publish -c release -f netcoreapp2.1 -r ubuntu.18.04-x64 -o bin/Publish/ubuntu.18.04-x64 --self-contained
dotnet publish -c release -f netcoreapp2.1 -r osx.10.13-x64 -o bin/Publish/osx.10.13-x64 --self-contained
~~~~