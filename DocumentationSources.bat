cd externals
cd docfx
rmdir ..\..\docs\sitedocs /s /q
mkdir ..\..\docs\sitedocs 
docfx Documentation\docfx.json -o ..\..\docs\DocumentationAPIcd 
xcopy ..\..\docs\DocumentationAPIcd\_site ..\..\docs\sitedocs /S /Y
rmdir ..\..\docs\DocumentationAPIcd /s /q