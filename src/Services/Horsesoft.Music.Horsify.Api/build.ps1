#dotnet clean
dotnet publish -c Release --self-contained --runtime win7-x86
XCOPY /I /E /Y  ".\bin\Release\netcoreapp2.2\win7-x86\publish\*.*" "..\..\Build\x86\Service"

dotnet publish -c release --self-contained --runtime win7-x64
xcopy /i /e /y  ".\bin\release\netcoreapp2.2\win7-x64\publish\*.*" "..\..\build\x64\service"