# WebAPI2CLI
Execute ASP.NET Core WebAPI from Command Line . Source at https://github.com/ignatandrei/WebAPI2CLI

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/ignatandrei/WebAPI2CLI/blob/master/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/ExtensionNetCore3.svg)](https://www.nuget.org/packages/ExtensionNetCore3)
![MyGet](https://img.shields.io/myget/ignatandrei/v/ExtensionNetCore3?label=CD2MyGet)
[![Build Status](https://dev.azure.com/ignatandrei0674/WebAPI2CLI/_apis/build/status/ignatandrei.WebAPI2CLI?branchName=master)](https://dev.azure.com/ignatandrei0674/WebAPI2CLI/_build/latest?definitionId=7&branchName=master)
![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/ignatandrei0674/WebAPI2CLI/7/master)
![Azure DevOps coverage (branch)](https://img.shields.io/azure-devops/coverage/ignatandrei0674/WebAPI2CLI/7/master)
![generateDocs](https://github.com/ignatandrei/webAPI2CLI/workflows/generateDocs/badge.svg)

# Why

What if, instead of running the WebAPI ( or just the site ) and waiting for commands from the user, you want also to execute from the command line some controllers actions ?

This project let's you do that by enabling the command line with 

 &lt; myexe &gt;.exe --CLI_ENABLED=1 --CLI_Commands=" ... " 

The command names are in a *cli.txt* file that can be generated with

 &lt; myexe &gt;.exe --CLI_ENABLED=1 --CLI_HELP=1


# How to use ( for .NET Core 3.1 )

## Step 0 : install into your ASP.NET Core Web 

Install the package https://www.nuget.org/packages/ExtensionNetCore3

Modify your ASP.NET Core as below:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddCLI();
//your code omitted
}    
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseCLI();
//your code omitted
}       
```
And that is all modifications that you need to do for the source code.

## Step 1 - find and save the definition of the commands, i.e. WebAPI endpoints

First, you must generate the definition of the commands. For this, we take the OPEN API (swagger ) approach.

For this, after you compile the project, you will run your .exe program with arguments:
 
 &lt; myexe &gt;.exe --CLI_ENABLED=1 --CLI_HELP=1

( or make this from *Visual Studio, Project, Properties, Debug* )

This will generate a *cli.txt* file with all definitions of the WebAPI.
( if your API does not appear, check if you have *ApiController* defined)

Open your *cli.txt*  file and modify the names of the commands as you wish (also , the arguments )

Copy this *cli.txt* in your solution and be sure that is copied with the exe ( in Visual Studio right click the file, properties, Build Action = Content, CopyToOutputDirectory = Copy if newer)

## Step 2 - run the commands

Ensure that the file is near your exe WebAPI.

Run the exe with the following:

 &lt; myexe &gt;.exe --CLI_ENABLED=1 --CLI_Commands="your first command,your second command, and enumerate all commands"

The program will run the commands and output the result.

## Optional Step 3 - letting others download the app to use as CLI
Modify the endpoints to add zip

```csharp
 app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MakeZip(app);
            });
```
and browser to <root of the site>/zip  to download the whole application.
More details here( including a demo)

https://ignatandrei.github.io/WebAPI2CLI/
