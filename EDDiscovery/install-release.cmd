@echo off
call ..\options.cmd

call uninstall 2>nul
copy /B "build\x64-Release\%PluginName%-EDD.dll" "%EDDPlugins%\"
