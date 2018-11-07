#dotnet clean
dotnet publish -c Release --self-contained --runtime win7-x86
XCOPY /I /E /Y  ".\bin\Release\netcoreapp2.1\win7-x86\publish\*.*" "..\..\Build\x86\Service"

# dotnet publish -c Release --self-contained --runtime win7-x64
# XCOPY /I /E /Y  ".\bin\Release\netcoreapp2.1\win7-x64\publish\*.*" "..\..\Build\x64\Service"