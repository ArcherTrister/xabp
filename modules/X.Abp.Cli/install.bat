@ECHO off
cls

dotnet tool uninstall -g X.Abp.Cli

dotnet tool install -g X.Abp.Cli --add-source ./src/X.Abp.Cli/bin/Debug

ECHO.
ECHO.Press any key to exit.
pause > nul
