@echo off

call :BUILD EDDiscovery
call :BUILD ObservatoryCore

goto :EOF

:BUILD
echo ===== Building %1...
pushd %1
call build.cmd
popd
if ERRORLEVEL 1 exit
exit /B
