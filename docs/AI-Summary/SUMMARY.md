# AI Project Summary

This folder is for AI assistants to keep project-specific context across coding sessions. Update this file when new architecture, conventions, or important implementation details are discovered.

## Project Snapshot

- Godot 4.6 C# single-player roguelite project named `SinglePlayerRogueliteV2`.
- Uses `Godot.NET.Sdk/4.6.2` with `net8.0`; Android targets `net9.0`.
- Main goal: repeatable runs where the player collects three gems by defeating three bosses across runs.
- Core loop from `README.md`: `Start -> Choose Path -> Fight -> Upgrade -> Boss -> Reward -> Repeat`.
- Biome progression is tiered as `A -> B -> C -> D`: the run starts in an A biome, offers roguelite path choices through one B biome and one C biome, then reaches a D boss biome.
- Path choice should feel semi-controlled rather than fully deterministic: the player chooses from available/generated paths, and lucky rolls may expose better or more desirable routes.
- The current diagram model is Grasslands Lv1 start, then Tundra/Desert Lv2, then Icy/Jungle/Lava Lv3, then Ice/Jungle/Lava Boss Lv4. Defeating a boss awards its gem.
- Android export preset exists; mobile/touch support is important.
- Phone-exclusive game designed for landscape mode, not desktop-first play.
- UI and interactable elements should generally be larger than conventional PC UI so they remain usable and readable on phones.
- Visual style direction is pixel art / low-poly, with assets and effects kept simple and easy to produce.
- Project is early active development. Some scenes and flows are placeholders.

## Important Paths

- `project.godot`: Godot project config and autoload registration.
- `README.md`: current game direction, visual references, setup, and autoload usage notes.
- `autoload/save/`: global save system and saved data resources.
- `autoload/overlay/`: global overlay layer for popup/modal UI and scene changes.
- `autoload/signals/`: global event bus helpers.
- `core/combat/`: combat resources and combat enums/types.
- `core/items/`: item resources, item subclasses, dependencies, and item `.tres` data.
- `core/codex/`: codex categories, entry data, and hardcoded resource lists.
- `core/ui/`: reusable button scripts for overlay and scene navigation actions.
- `scenes/main/`: top-level screens like start menu, character select, and outpost.
- `scenes/overlays/`: popup/modal overlays.
- `scenes/components/`: reusable UI/gameplay scene components.
- `scenes/debug/`: development/test scenes.
- `assets/`: fonts, gem art, style, and placeholder building art.

## AI Detail File Index

- `docs/AI-Summary/Item-System-Goal.md`: item-system intent, generic dependencies, consumables, condition/durability, ammo, and future extensibility.
- `docs/AI-Summary/Outpost-System.md`: outpost building generation, `BuildingData`, permanent/random slots, saved run data, and building overlay flow.
- `docs/AI-Summary/Visual-Character-Plan.md`: procedural player visual structure plan, part-based animation direction, and shared item/equipment art usage.

## Godot Setup

- Main scene is configured by UID in `project.godot`.
- Autoloads registered in `project.godot`:
- `SaveNode` from `res://autoload/save/SaveNode.tscn`.
- `GlobalOverlay` from `res://autoload/overlay/GlobalOverlay.tscn`.
- `SignalHandler` from `res://autoload/signals/SignalHandler.tscn`.
- Display stretch uses `canvas_items` and `expand`.
- Rendering uses GL compatibility; default canvas texture filter is nearest (`0`).
- Physics engine is Jolt Physics.

## Coding Style And Conventions

- Scripts are C#, not GDScript.
- Godot resources that should appear in the editor use `[GlobalClass]` and inherit `Resource`.
- Scene scripts generally inherit Godot nodes such as `Control`, `Node2D`, `CanvasLayer`, `CharacterBody2D`, or `Camera2D`.
- Exported editor fields use `[Export] public ... { get; set; }`.
- UI scripts commonly fetch nodes by scene paths in `_Ready()` with `GetNode` or `GetNodeOrNull`.
- For global systems, prefer existing static accessors like `SaveNode.Get()`, `GlobalOverlay.Get()`, and `SignalHandler` helper methods.
- Keep changes small and consistent with existing direct/simple C# style.
- Existing naming includes some typos such as `EquipedItemsData`; preserve existing names unless explicitly asked to rename.
- Store categories, types, and fixed option sets as enums instead of strings whenever practical. This should be remembered for gameplay categories, item types, ammo types, locations, biomes, codex categories, and similar data.

## Save System

- `SaveNode` is the save autoload and central runtime access point for save data.
- Default save path is `user://saves/`.
- Three save file types exist through `SaveData.FileType`: `Meta`, `Run`, and `Settings`.
- Save files are Godot resources saved as `.tres` via `ResourceSaver.Save`.
- `SaveNode` exposes `MetaData`, `RunData`, `SettingsData`, `PlayerData`, `InventoryData`, and `EquipedItemsData`.
- `SaveNode.ExecuteReady()` creates the save directory, loads all data, saves defaults if files are missing, initializes player data, and refreshes start characters.
- `StartCharacters` are generated randomly from `CharacterNameData` with randomized skill XP.
- `NewCharacterComponent` writes selected `PlayerData` to `SaveNode.RunData.PlayerData`, saves run data, refreshes start characters, then moves to the outpost.

## Overlay And Navigation System

- `GlobalOverlay` is a `CanvasLayer` autoload used for overlays and scene transitions.
- Use `GlobalOverlay.Get()?.AddOverlay(packedScene)` to open overlays.
- Use `CloseTopOverlay()` and `CloseAllOverlays()` to remove overlays.
- Use `ChangeRootScene(PackedScene)` to change the active main scene; it clears all overlays first.
- Reusable button scripts in `core/ui/`:
- `OpenOverlay`: opens an exported overlay scene.
- `CloseOverlay`: closes configured overlay targets.
- `ChangeOverlayScene`: changes root scene through `GlobalOverlay`.
- `ControlGotoScene`: another scene navigation helper.
- Use these helpers for simple UI wiring before adding custom scripts.

## Signal System

- `SignalHandler` is the global event bus autoload.
- It emits one generic Godot signal carrying `SignalType` and `Variant` payload.
- Prefer specific helper methods instead of the lower-level generic API.
- Current helper coverage includes purchase item, item equipped, and gold amount changed events.
- Subscribe in `_Ready()` and unsubscribe in `_ExitTree()` to avoid lingering handlers.

## Core Data Models

- `RunData` tracks current biome, current location, current contract, inventory, completed contracts, gold, and active player data.
- `RunData.OutpostBuildings` stores generated outpost `BuildingData` resources for the current run; `null` means the outpost has not generated its random buildings yet.
- `PlayerData` tracks player name, equipped items, and skill data.
- `MetaData`, `SettingsData`, `InventoryData`, `EquipedItemsData`, `PlayerSkillData`, and store data live under `autoload/save/data/`.
- Shared enums live in `core/types/Types.cs` under `MyTypes` and `SaveData` namespaces.
- Current biomes include grasslands, tundra, desert, icy, jungle, lava, and three boss biomes.
- Current locations include village, sanctuary, and campsite.

## Combat And Items

- Combat uses resource classes in `core/combat/`: `CombatContainer`, `Damage`, `Defence`, and `StatusEffects`.
- `CombatContainer.ApplyDamage()` sums damage and status-effect values after defence reflection percentages, then subtracts total from current health.
- `ApplyTick()` delegates ticking to active status effects.
- Items inherit from `ItemBase`, a `[GlobalClass] Resource` with exported name, dependency set, icon, stack size, cost, generated `ItemID`, optional use counts, optional condition, and optional ammo type.
- Item subclasses include equipable, armor, consumable, ammo, and amulet resources.
- Item dependency classes live under `core/items/dependencies/`; `ItemDependency` is the container for requirements like skill level, mana, stamina, and ammo type.
- `ActionContext` is the shared runtime state for generic player actions and generic dependencies.
- `ItemUseContext` inherits from `ActionContext` and adds item-specific state such as ammo, use-count cost, and condition damage.
- `PlayerAction` is the reusable effect layer for item use and future player-triggered abilities; dependencies remain the separate requirement/cost layer.
- `ItemUsable` is the shared usable-item base. `PlayerAction` should live on usable item types like `ItemEquipable` and `ItemConsumable`, not on `ItemBase`.
- `ItemUseContext` is the mutable context for real item use. Use `CanUse` for checks and `TryUse` when mana, stamina, ammo counts, item use counts, condition, or assigned `PlayerAction` execution should actually change.
- Sample item resources are in `core/items/data/` and are currently hardcoded into `CodexData`.

## Codex

- `CodexData` statically defines categories and resource paths.
- Item codex entries are generated from hardcoded item `.tres` resources.
- Enemy/location entries are loaded from hardcoded `core/codex/entries/*.tres` resources.
- `CodexOverlay` builds its UI in C# from existing scene containers.
- Current codex categories are items, enemies, and locations. Enemy subcategories currently include slimes.

## Input And Player

- `Player` is a `CharacterBody2D` with simple movement at speed `300`.
- Movement prefers touch left-stick input from `CanvasLayer/TouchControls`; falls back to keyboard `ui_left`, `ui_right`, `ui_up`, `ui_down`.
- `TouchControls` exports four buttons, two sticks, debug label, and buffered button press duration.
- `TouchInput` carries left/right stick vectors and six button states.
- Touch controls explicitly handle `InputEventScreenTouch` and mark handled touches through the viewport.
- `CameraTiltEffect` uses accelerometer gravity to offset a `Camera2D`, which reinforces mobile orientation support.

## UI And Scenes

- Main flows currently include start menu, run overview, new character selection, and outpost.
- `RunOverview` wires continue/new-run/back buttons to outpost, new character select, and start menu scenes.
- `Outpost.tscn` main overlay buttons are split between `TopUI` and `BottomUI`: `ExitButton` changes back to `StartMenu`, `BtnSettings` opens `SettingsOverlay`, `BtnInventory` opens `InventoryOverlay`, `BtnCharacter` opens `CharacterOverlay`, and `BtnCodex` opens `CodexOverlay`.
- `Outpost.tscn` has permanent `Contract` and `Inn` building template instances under `World`, plus generated slots under `World/OutpostBuildings` named `RandomSlot1`, `RandomSlot2`, and `RandomSlot3`.
- `OutpostBuildings.cs` checks `SaveNode.RunData.OutpostBuildings`, generates mock building data when null, saves it to run data, and instantiates `buildingTemplate.tscn` into random slots.
- `BuildingTemplate` always opens `BuildingOverlay.tscn` when clicked/touched through its `Area2D`, with a 250ms debounce, and passes its `BuildingData` into the overlay.
- `BuildingOverlay` should stay thin; `BuildingData.Generate(BuildingOverlay)` owns regenerating title, description, owner portrait, RPG-style rich text dialogue, and bottom action buttons.
- Panel components display stats, gold, run metadata, location, building information, and player top UI.
- Design UI for landscape phones first. Prefer big readable labels, generous spacing, and large touch targets over compact PC-style layouts.
- Many UI scenes rely on exact node paths. When editing `.tscn` structure, update corresponding C# paths.

## Verification Notes

- Preferred validation is opening/building the project in Godot 4.6 with C# support.
- `dotnet build` may be useful for script compile checks if the Godot .NET SDK is available locally.
- For UI/navigation changes, also run the relevant scene in Godot because node paths and exported scene assignments are editor-dependent.

## Current Cautions For Future Sessions

- Do not assume systems are complete; many flows are scaffolding or placeholders.
- Be careful with saved data shape because Godot resources are serialized to user saves.
- Be careful changing resource class names, namespaces, exported property names, or `[GlobalClass]` types because scenes/resources may reference them.
- Preserve mobile/touch behavior unless the task is explicitly desktop-only.
- Preserve the phone-exclusive, landscape-first assumption unless the user explicitly changes platform direction.
- Keep art and visual implementation aligned with simple pixel-art / low-poly production needs.
- Check existing `.tscn` node paths before changing scene scripts.
