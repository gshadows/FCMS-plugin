@echo off
call ..\options.cmd

if not exist build\x64-Release (
	echo "Release not found!"
	exit 1
)
pushd build\x64-Release

set ZIP_FILE=%RELEASE_PATH%\%PluginName%-OBS.%PluginVersion%.eop

del /Q %ZIP_FILE% 2>nul
%SZ% a -tzip %ZIP_FILE% %PluginName%-OBS.dll

popd
