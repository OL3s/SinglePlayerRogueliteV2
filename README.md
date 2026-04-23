# SinglePlayerRoguelite

Godot 4.6 C# single-player roguelite project.

## Overview

This project is built around repeatable runs, route choices, boss fights, and long-term progression. The player works toward collecting three gems by defeating three different bosses across multiple runs.

**Core Goal**

```text
Collect the 3 gems by defeating the 3 different bosses across runs
```

**Core Loop**

```text
Start -> Choose Path -> Fight -> Upgrade -> Boss -> Reward -> Repeat
```

<img width="800" alt="Game loop diagram" src="docs/images/game-loop-diagram.png" />

## Visual References

<img width="800" alt="Main menu template" src="docs/images/mainmenu-example.png" />

**Main Menu**

<img width="800" alt="Outpost wireframe for the between-run stop" src="docs/images/outpost-example.png" />

**Outpost**

## Current Direction

- Single-player roguelite structure with run-based progression
- Branching paths and encounter flow between bosses
- Hub-style stops between runs such as village, sanctuary, or campsite spaces
- Shared combat, item, and progression systems in reusable C# code
- Autoload-based global systems for save data, overlays, and signals

## Project Status

- Core combat and data models are in place
- Item and dependency resources are defined
- Global autoload systems are registered
- Early UI and screen scenes are present
- Touch controls are available

The project is still in active development, and some scenes and UI flows are placeholders.

## Setup

### Requirements

- Godot 4.6 with C# support
- .NET SDK compatible with the installed Godot version

### Open The Project

1. Open Godot 4.6.
2. Import this folder as an existing project.
3. Let Godot restore and build the C# project files if needed.
4. Run the default main scene configured in `project.godot`.

## Main Project Structure

```text
autoload/
  overlay/    Global overlay autoload scene and script
  save/       Save autoload scene and save data classes
  signals/    Signal autoload scene and script

core/
  combat/     Reusable combat resources and combat-related types
  items/      Item resources and item dependency logic
  progression/ Shared progression data resources
  types/      Shared enums and common types
  ui/         Reusable non-scene UI helper classes

assets/
  gems/       Gem image assets

scenes/
  screens/    Top-level game screens such as menus and outpost screens
  components/ Reusable scene components and UI pieces
  startmenu/  Start menu presentation scenes
```
