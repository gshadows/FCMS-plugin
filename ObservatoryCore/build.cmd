@echo off
call ..\options.cmd

call :BUILD x64 Debug
call :BUILD x64 Release

goto :EOF

:BUILD
echo Building %1 %2...
dotnet build -a %1 -c %2 --nologo -o .\build\%1-%2\ -p:ObservatoryPath="%OBSInstall%" -p:PluginName=%PluginName%-OBS -p:PluginVersion=%PluginVersion%
if ERRORLEVEL 1 exit
exit /B
