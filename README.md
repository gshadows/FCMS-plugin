# FCMS API connection plugin.
This plugin is compilable for "EDDescovery" and "Observatory Core" tools for "Elite Dangerous" game.

### Current features

* Send FC events to the FCMS site.

### Future plans

- None yet.

Ask for new features in GitHub issue tracker, or find me on Discord servers: TSEG (for guild members), ["ObsCore"](https://discord.gg/nuayD8Nh), ["EDCD"](https://discord.gg/0uwCh6R62aQ0eeAX).

## How To Install
Download plugin DLL and put into plugins folder of EDDiscovery.

## How To Use
Setup your FCMS API key and enjoy.

## How to build

### Requirements
- C# 9.0 / .NET 8.0
- **EDDiscovery** installed on your PC, or "EDDDLLInterfaces.dll" available somewhere (only if you build plugin for it).
- **ObservatoryCore** installed on your PC, or "ObservatoryFramework.dll" available somewhere (only if you build plugin for it).
- **7-Zip** archiver installed (optionally, for preparing release packages only, not required to build & install locally or if you prepare packages manually).

### Project structure

* **Common** - Tool independent plugin code.
* **EDDiscovery** - EDDiscovery plugin project and interface code.
* **ObservatoryCore** - Observatory Core plugin project and interface code.
* **releases** - Output folder, created by the "package.cmd" script to place release packages there.

### Building plugin DLL from sources

1. Check paths in the "options.cmd" script. By default there's tools default installation folders are already configred.
1. Run "build.cmd" to build plugin DLLs:

   1. To build all versions at once - "build.cmd" from main folder.
   1. To build for one concrete tool - "build.cmd" from concrete tool sub-folder.

### Install and test built plugin

1. Ensure your tool is not running. If it running - exit it.
2. Run "install-release.cmd" from the tool sub-folder to install plugin.
3. Run "start.cmd" to start EDDescovery, or run it from your usual shortcut.

### Prepare release

1. Run "package.cmd" script.
2. Find plugin packages for tools you build for inside "releases" (such as \*.eop or \*.dll).
