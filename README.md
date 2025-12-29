
# Unreal Project Hub

Unreal Project Hub is a lightweight Windows tool for managing and maintaining Unreal Engine projects from a single interface.

## Overview

Unreal Engine projects often require repetitive maintenance tasks during development, such as cleaning build artifacts, regenerating IDE files, and navigating between the editor, IDE, and project folders.

Unreal Project Hub consolidates these common operations into a single utility, reducing context switching and manual setup work.

The tool is designed as an internal-style project utility rather than a consumer-facing application.

## Features

- Manage multiple Unreal Engine projects
- Clean project build artifacts:
  - `.vs`
  - `Binaries`
  - `Intermediate`
  - `DerivedDataCache`
- Regenerate IDE project files using UnrealBuildTool
- Launch Unreal Editor directly from `.uproject`
- Open IDE solution
- Open project dictionary in Explorer
- Persistent project list stored locally
- Live logging operations in the UI

## How It Works

- Engine installations are resolved from the `.uproject` `EngineAssociation`
- IDE project files are regenerated via Unreal Engine’s official UnrealBuildTool
- Running Unreal Editor instances are terminated before maintenance operations
- All long-running tasks execute asynchronously with streamed log output
- No Unreal Engine plugins or engine modifications are required

## Build & Run

### Requirements
- Windows
- .NET 8 SDK
- Unreal Engine (Launcher or source build)

### Build
```bash
dotnet build -c Release
```

### Run
```bash
dotnet run -c Release
```
Alternatively, download the release ZIP, extract it, and run `Unreal Project Hub.exe`

## Usage
1. Add one or more .uproject files to the project list
2. Use project actions to:
	- Open Unreal Editor
	- Open the IDE solution
	- Clean and regenerate project files
3. View operation progress and output in the log panel
4. Access project folders and Unreal Editor logs directly from the UI

## Limitations / Notes

- Windows-only
- Assumes standard Unreal Engine installation layout
- Intended as a development utility, not a replacement for Epic Games Launcher
- Not an official Epic Games product
