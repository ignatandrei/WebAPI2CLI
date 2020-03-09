# If you want a demo 

## If you have Win10 x64 

Go to https://dev.azure.com/ignatandrei0674/WebAPI2CLI/_build?definitionId=7&_a=summary

Click on the latest job.

Click on artifacts.

See drop1 . In the right of drop1 , you can download the drop1 folder.

Unzip the drop1 ( you will need the cli.txt that contains the WebAPI definitions)

Inside drop1 , you will find TestWebAPISite.exe

Run TestWebAPISite.exe and browse to http://localhost:5000/swagger

Close TestWebAPISite.exe command prompt.

Now run 

TestWebAPISite.exe --CLI_Enabled=1 --CLI_COMMANDS="GetMathId_Http,WeatherGet"

( ensure the cli.txt is near to TestWebAPISite.exe)

## If you have another operating system than Win10-x64 and I want a demo

It is easy to make a demo for you.
Modify the .csproj and the yml file in azure. Or make an issue at https://github.com/ignatandrei/webAPI2CLI/issues and I will do it for you.

