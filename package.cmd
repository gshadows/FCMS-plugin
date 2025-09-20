@echo off

call :PACKAGE EDDiscovery
call :PACKAGE ObservatoryCore

goto :EOF

:PACKAGE
echo ===== Packaging %1...
pushd %1
call %1\package.cmd
popd
if ERRORLEVEL 1 exit
exit /B
