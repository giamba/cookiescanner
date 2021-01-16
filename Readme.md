# Most Active Cookie

This is command line program written in .NET Core 5.0.  
This command line tool allow you to process a log file and return the most active cookie for a specific day.  

e.g.  
```
./cookiescanner.exe -f test1.csv -d 2018-12-09
AtY0laUfhglK3lC7
```
## Prerequirement 
In order to compile and run this application you need .NET Core 5.0 SDK installed in your machine. 
Here the SDKs for Windows, Mac and Linux : https://dotnet.microsoft.com/download/dotnet/5.0

Installation Instructions:
- Windows: https://docs.microsoft.com/en-us/dotnet/core/install/windows?tabs=net50
- Mac: https://docs.microsoft.com/en-us/dotnet/core/install/macos
- Ubuntu: https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu

## Build, Run and Test 
```
#build  
dotnet build -c Release

#run 
cd cookiescanner/bin/Release/net5.0/
./cookiescanner.exe -f test1.csv -d 2018-12-09

#tests
dotnet test 
```

## Publishing
If you want to distribute the cookiescanner cli to various hosts you can use `dotnet publish` command.  
It is possible to publish the cookiescanner for:
- Windows:  
`dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true`  
 output: `cookiescanner\bin\Release\net5.0\win-x64\publish`
- Mac:  
`dotnet publish -c Release -r osx-x64 -p:PublishSingleFile=true --self-contained true`  
output: `cookiescanner\bin\Release\net5.0\osx-x64\publish`
- Ubuntu:  
`dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained true`  
output: `cookiescanner\bin\Release\net5.0\linux-x64\publish`

The host only need the runtime installed (no SDK).
You need to distribute all the files contained in the publish folder.


