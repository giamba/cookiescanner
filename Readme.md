

Build 


CookieScanner

Assumption:  
The file is correctly formatted 


Build 
dotnet build 

Tests
dotnet test





- dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
- dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained true
- dotnet publish -c Release -r osx-x64 -p:PublishSingleFile=true --self-contained true



Install .NET Runtime Linux, Mac, Windows 

.NET Core SDK and Runtimes  Linux, Mac, Windows 
https://dotnet.microsoft.com/download/dotnet/5.0



Install the .NET Core Runtimes for Windown, Mac or Linux
https://dotnet.microsoft.com/download/dotnet/5.0

Check the runtime running:
dotnet --list-runtimes




Install Runtime on Ubuntu
Go to https://dotnet.microsoft.com/download/dotnet/5.0


Check the you have the Runtime 5.0.x
dotnet --list-runtimes






mkdir -p "$HOME/dotnet" && tar zxf aspnetcore-runtime-5.0.0-linux-x64.tar.gz -C "$HOME/dotnet"
export DOTNET_ROOT=$HOME/dotnet
export PATH=$PATH:$HOME/dotnet

