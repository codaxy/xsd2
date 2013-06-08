pushd %~dp0
mkdir xsd2\tools
copy ..\xsd2\bin\release\xsd2.exe xsd2\tools\
..\.nuget\nuget.exe pack xsd2\Codaxy.Xsd2.nuspec
popd
pause