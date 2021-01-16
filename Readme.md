

Build 


CookieScanner

Assumption:  
The file is correctly formatted 


Build 
dotnet build 

Tests
dotnet test



Publish 
dotnet publish -r osx-x64
dotnet publish -r win-x64

dotnet publish -c Release -r osx-x64 -p:PublishReadyToRun=true
dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true


dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained true
dotnet publish -c Release -r osx-x64 -p:PublishSingleFile=true --self-contained true
