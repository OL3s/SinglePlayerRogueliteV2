# SinglePlayerRogueliteV2

Godot 4.6 C# project for rebuilding the singleplayer frontend from scratch while keeping selected shared backend logic from the previous project.

## Core Goal

```text
Collect the 3 gems by defeating the 3 different bosses across runs
```

## Core Loop

```text
Start -> Choose Path -> Fight -> Upgrade -> Boss -> Reward -> Repeat
```

<img width="800" alt="Game loop diagram" src="docs/images/game-loop-diagram.png" />

## Gameplay Notes

- Branching paths per run
- Locations between levels (Village / Sanctuary / Campsite)
- Boss ends run
- Reward given on completion or death

## Current Scope

- Autoloads are transferred and registered.
- Shared backend/data logic is transferred.
- Gem image assets are transferred.
- Old frontend scenes and UI logic are intentionally not transferred.
- Old map generation logic is intentionally not transferred.

## Kept From V1

- Autoload singletons
- Shared backend/data logic
- Gem image assets
- Touch controls component

## Intentionally Left Out

- Old frontend scenes
- Old frontend UI logic
- Old map generation logic

## Folder Structure

```text
autoload/
  overlay/    Global overlay autoload scene and script
  save/       Save autoload scene and save data classes
  signals/    Signal autoload scene and script

core/
  combat/     Reusable combat resources and types
  items/      Item resources and dependency logic
  progression/ Shared progression data resources
  types/      Shared enums and common types
  ui/         Reusable non-scene UI helper classes

assets/
  gems/       Imported gem images only

scenes/
  screens/    Top-level app/game screens
  components/ Reusable UI scenes
  gameplay/   Gameplay scenes
  menus/      Menu scenes
  debug/      Debug/test scenes
```

## Notes

- Keep scene files in `scenes/`.
- Keep reusable non-visual C# logic in `core/`.
- Keep autoload singleton scenes in `autoload/`.
- Place scene-specific scripts next to the scene that uses them.
