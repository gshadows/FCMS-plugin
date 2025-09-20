@echo off
call ..\options.cmd

del /Q "%OBSPlugins%\%PluginName%-OBS*.dll"
