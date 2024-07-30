@ECHO off
cls

ECHO Starting generate...
ECHO.

dotnet tool uninstall -g X.Abp.Cli
dotnet tool install -g X.Abp.Cli --version 7.2.3-preview --add-source ./

dotnet new uninstall X.Abp.Templates
dotnet new install X.Abp.Templates.7.2.3-preview.nupkg


ECHO.
ECHO.successfully install. Press any key to exit.
pause > nul
