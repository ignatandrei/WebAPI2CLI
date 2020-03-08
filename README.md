# WebAPI2CLI
Execute ASP.NET Core WebAPI from Command Line

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/ignatandrei/WebAPI2CLI/blob/master/LICENSE)  
[![NuGet](https://img.shields.io/nuget/v/ExtensionNetCore3.svg)](https://www.nuget.org/packages/ExtensionNetCore3)
![MyGet](https://img.shields.io/myget/ignatandrei/v/ExtensionNetCore3?label=CD2MyGet)
[![Build Status](https://dev.azure.com/ignatandrei0674/WebAPI2CLI/_apis/build/status/ignatandrei.WebAPI2CLI?branchName=master)](https://dev.azure.com/ignatandrei0674/WebAPI2CLI/_build/latest?definitionId=7&branchName=master)
![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/ignatandrei0674/WebAPI2CLI/7/master)
![Azure DevOps coverage (branch)](https://img.shields.io/azure-devops/coverage/ignatandrei0674/WebAPI2CLI/7/master) 

# How to use ( for .NET Core 3.1 )

## Step 1 - find the definition of the commands

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

 &lt; myexe &gt.exe --CLI_ENABLED=1 ----CLI_Commands="your first command,your second command, and enumerate all commands"

The program will run the commands and output the result.
