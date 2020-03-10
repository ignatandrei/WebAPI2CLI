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

## If you have another operating system than Win10-x64 and want a demo

It is easy to make a demo for you.
Modify the .csproj and the yml file in azure. Or make an issue at https://github.com/ignatandrei/webAPI2CLI/issues and I will do it for you.

## Could I see an example of the output ? 

Of course. Every build in AzureDevOps has a last step , runningADemo.
See https://dev.azure.com/ignatandrei0674/WebAPI2CLI/_build?definitionId=7&_a=summary

## Could you put here the output ? 
Yes. 
This is the output of 

TestWebAPISite.exe  --CLI_ENABLED=1 --CLI_Commands="GetMathId_Http,MathPOST"

See *Result* variable

1. 2020-03-10T19:03:39.2929915Z ExtensionNetCore3 version:1.2020.10310.11900
1. 2020-03-10T19:03:40.0045036Z info: Microsoft.Hosting.Lifetime[0]
1. 2020-03-10T19:03:40.0046360Z       Now listening on: http://localhost:5000
1. 2020-03-10T19:03:40.0047178Z info: Microsoft.Hosting.Lifetime[0]
1. 2020-03-10T19:03:40.0047815Z       Now listening on: https://localhost:5001
1. 2020-03-10T19:03:40.0048438Z info: Microsoft.Hosting.Lifetime[0]
1. 2020-03-10T19:03:40.0049078Z       Application started. Press Ctrl+C to shut down.
1. 2020-03-10T19:03:40.0049702Z info: Microsoft.Hosting.Lifetime[0]
1. 2020-03-10T19:03:40.0050292Z       Hosting environment: Production
1. 2020-03-10T19:03:40.0050877Z info: Microsoft.Hosting.Lifetime[0]
1. 2020-03-10T19:03:40.0051895Z       Content root path: D:\a\1\a
1. 2020-03-10T19:03:44.6267548Z CLIExecute version:1.2020.10310.11900
1. 2020-03-10T19:03:45.4122334Z *executing GetMathId_Http*
1. 2020-03-10T19:03:46.0585725Z {
1. 2020-03-10T19:03:46.0586788Z   "Command": {
1. 2020-03-10T19:03:46.0587511Z     "NameCommand": "GetMathId_Http",
1. 2020-03-10T19:03:46.0588202Z     "Host": "http://localhost:5000",
1. 2020-03-10T19:03:46.0588851Z     "RelativeRequestUrl": "api/MathAdd/5",
1. 2020-03-10T19:03:46.0589427Z     "Verb": "GET"
1. 2020-03-10T19:03:46.0589936Z   },
1. 2020-03-10T19:03:46.0590434Z   "StatusCode": 200,
1. 2020-03-10T19:03:46.0590955Z   "*Result*": "value5"
1. 2020-03-10T19:03:46.0591452Z }
1. 2020-03-10T19:03:46.0591977Z executing *MathPOST*
1. 2020-03-10T19:03:46.1304847Z {
1. 2020-03-10T19:03:46.1306343Z   "Command": {
1. 2020-03-10T19:03:46.1307062Z     "NameCommand": "MathPOST",
1. 2020-03-10T19:03:46.1307766Z     "Host": "http://localhost:5000",
1. 2020-03-10T19:03:46.1308471Z     "RelativeRequestUrl": "api/MathAdd",
1. 2020-03-10T19:03:46.1309710Z     "Verb": "POST"
1. 2020-03-10T19:03:46.1310813Z   },
1. 2020-03-10T19:03:46.1311457Z   "StatusCode": 200,
1. 2020-03-10T19:03:46.1312079Z   "Result": ""
1. 2020-03-10T19:03:46.1312652Z }

