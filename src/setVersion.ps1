$TimeNow = Get-Date
$d = $TimeNow.ToUniversalTime().ToString("1.yyyy.1MMdd.1HHmm")
dotnet-property "/*.csproj" AssemblyFileVersion:"$d"
dotnet dotnet-property "/*.csproj" AssemblyFileVersion:"$d"