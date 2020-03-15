# F.A.Q.

## My port changes from development to integration. How to wrote the address one time ?

WebAPI2CLI has a cli.txt file ( see https://github.com/ignatandrei/WebAPI2CLI/blob/master/src/TestWebAPISite/cli.txt ) 

In this file put just -  

Host: http:// 

or 

Host: https://
 
 ( Assumption : just one http and/or just 1 https when asp.net core will start )

WebAPI2CLI will find the adress and the port comparing 

Alternatively, you can put the full URI ( without RelativeRequestUrl ! )

Host: http://localhost:5000/

## I want to contribute  . Where is the code  ? 

All code source is at https://github.com/ignatandrei/WebAPI2CLI/ 

Please see issues tab if you want to know what needs development .

## Will my WebAPI work as before?

The software takes care about 

--CLI_ENABLED=1

If you do not have this command, your website runs as before

## I want to run as a CLI and then use the WebAPI as before. Could I do that? 

Yes. Use 

 &lt; myexe &gt;.exe  --CLI_ENABLED=1 --CLI_Commands="your first command,your second command" --CLI_STAY=1

## I found a bug / I need a feature . Where can I report ?

You can report problems at https://github.com/ignatandrei/WebAPI2CLI/issues

## I want to see the classes documentation before downloading the project.

Glad you asked . See https://ignatandrei.github.io/WebAPI2CLI/sitedocs/api/index.html and choose from the left menu the classes.

## Where can I download this document ? 

There is a PDF at https://ignatandrei.github.io/WebAPI2CLI/Web2CLI.pdf



