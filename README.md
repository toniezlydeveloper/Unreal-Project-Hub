
# Unreal Project Hub
![Unreal Project Hub – project overview](assets/unreal-project-hub-overview.png)

Unreal Project Hub is a small Windows utility that helps you manage and maintain multiple Unreal Engine projects from a single place.

It’s built to remove the repetitive friction of everyday Unreal development: cleaning projects, regenerating IDE files, and jumping between the editor, IDE, and folders.

This is an internal-style developer tool, designed for speed and convenience rather than end-user polish.

---

## Why This Exists

If you work with Unreal Engine regularly, you probably do this a lot:

- delete `Binaries` / `Intermediate` when something breaks  
- regenerate project files after engine or plugin changes  
- open the editor, then the IDE, then the project folder  
- juggle multiple `.uproject` files across different projects  

Unreal Project Hub puts all of that behind a single, repeatable UI, so common maintenance tasks take seconds instead of minutes.

---

## What You Can Do With It

- Manage multiple Unreal projects in one list
- Clean common build artifacts:
  - `.vs`
  - `Binaries`
  - `Intermediate`
  - `DerivedDataCache`
- Regenerate IDE project files via UnrealBuildTool
- Launch Unreal Editor directly from a `.uproject`
- Open the IDE solution
- Open the project folder in Explorer
- Keep a persistent local project list
- See live logs while operations are running

---

## How It Works (High Level)

- Engine installations are resolved from the `.uproject` `EngineAssociation`
- IDE files are regenerated using Unreal Engine’s official UnrealBuildTool
- Any running Unreal Editor instances are closed before maintenance tasks
- Long-running operations run asynchronously with streamed log output
- No Unreal plugins, engine changes, or project modifications are required

This tool operates entirely outside the engine.

---

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

You can also download the release ZIP, extract it, and run:

```
Unreal Project Hub.exe
```

---

## Typical Usage

1. Add one or more `.uproject` files to the project list
2. Select a project and:
   - open Unreal Editor
   - open the IDE solution
   - clean and regenerate project files
3. Quickly access project folders and editor logs

---

## Scope & Limitations

- Windows-only
- Assumes standard Unreal Engine installation layouts
- Focused on developer workflows, not project configuration
- Not a replacement for Epic Games Launcher
- Not an official Epic Games product
