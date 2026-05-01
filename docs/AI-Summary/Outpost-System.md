# Outpost System Memory

Internal AI memory for the outpost and building system. Keep this aligned with current code and intended direction.

## Core Intent

The outpost is the between-run hub. Some buildings are permanent, while other building slots are generated and saved as part of the current run.

Important design split:

- `Node2D` is used for world placement, collision, and game-object logic.
- `Control` is used for UI such as building prompts, overlay buttons, owner portraits, and dialogue text.
- Do not build non-trivial UI from `Node2D` unless it is intentionally very simple world art.

## Important Scenes And Scripts

- `scenes/main/outpost/Outpost.tscn`: main outpost scene.
- `scenes/main/outpost/OutpostBuildings.cs`: script on `World/OutpostBuildings`; owns random/generated outpost building generation.
- `scenes/components/outpost/buildingTemplate.tscn`: reusable in-world building scene.
- `scenes/components/outpost/BuildingTemplate.cs`: click/touch handler for a building; always opens `BuildingOverlay.tscn`.
- `scenes/components/outpost/BuildingData.cs`: resource that owns building display data and overlay generation logic.
- `scenes/overlays/building/BuildingOverlay.tscn`: full-screen building overlay.
- `scenes/overlays/building/BuildingOverlay.cs`: intentionally thin; forwards `Update(BuildingData)` to the resource.
- `scenes/overlays/building/BuildingOverlayButtonData.cs`: resource for one building overlay button.

## Save Flow

Generated outpost buildings are saved inside `RunData`:

```csharp
[Export] public Array<BuildingData> OutpostBuildings { get; set; }
```

Current first-entry behavior:

- `RunData.OutpostBuildings == null` means the player has not had this outpost generated yet.
- `OutpostBuildings._Ready()` checks this value through `SaveNode.Get().RunData.OutpostBuildings`.
- If null, it generates mock data for now, assigns it to `RunData.OutpostBuildings`, and calls `SaveNode.SaveRunData()`.
- On later visits, saved `BuildingData` is reused.

Be careful changing exported names on `BuildingData` because saved `.tres` run data can serialize these fields.

## Current Outpost Layout

In `Outpost.tscn`:

- `World/OutpostBuildings` owns randomized/generated slots.
- Generated slots are named `RandomSlot1`, `RandomSlot2`, `RandomSlot3`.
- Permanent outpost buildings currently include `Contract` and `Inn` as direct children under `World`.
- `Contract` and `Inn` intentionally instance `buildingTemplate.tscn` because they should always exist in an outpost.
- Random slots are empty `Node2D` placeholders; `OutpostBuildings.cs` instantiates `buildingTemplate.tscn` into them.

## BuildingData Resource

`BuildingData` is the central data/resource object for a building. It should contain the information needed to generate the building overlay and should be suitable for saving in `RunData`.

Current exported data includes:

- `LabelName`: building title, such as `Inn`, `Contract`, or `Blacksmith`.
- `Description`: short descriptive text for `PanelBuilding`.
- `OwnerTexture`: portrait/storekeeper/building-owner image for the overlay homepage.
- `OwnerText`: rich text dialogue shown in the center homepage area.
- `OwnerTextRevealSeconds`: duration for RPG-style dialogue reveal.
- `BuildingTexture`: in-world building sprite texture.
- `OverlayButtons`: array of `BuildingOverlayButtonData` resources.

`BuildingData.Generate(BuildingOverlay overlay)` owns the overlay regeneration. Keep the overlay construction logic here unless there is a strong reason to move it.

## Building Overlay

`BuildingOverlay` should stay simple. Its public flow is:

```csharp
public void Update(BuildingData buildingData) {
    buildingData.Generate(this);
}
```

The overlay homepage currently has:

- Owner/building image on the left.
- Rich text dialogue in the center.
- Dialogue reveal animation using `RichTextLabel.VisibleRatio` from `0` to `1`.
- Bottom action buttons generated from `BuildingData.OverlayButtons`.

`BuildingOverlayButtonData` currently has:

- `IconPath`: path to an icon resource; empty path falls back to placeholder icon.
- `LabelName`: button text.
- `PathController`: packed scene opened when the button is pressed.

## Current Mock Data

`OutpostBuildings.GenerateMockBuildings()` currently creates one mock `Blacksmith` building.

Current placeholder behavior:

- Blacksmith uses generated `PlaceholderTexture2D` for building and owner images.
- The mock `Forge` button uses an empty icon path so `BuildingData` falls back to a placeholder icon.
- The mock `Forge` button currently has no `PathController`; pressing it logs a warning.

## Permanent Building Data Files

Permanent building data currently lives in resource files:

- `scenes/components/outpost/inn_building_data.tres`
- `scenes/components/outpost/contract_building_data.tres`

These are referenced by `Outpost.tscn` on the permanent building template instances.

## Implementation Cautions

- Keep generated building slots separate from permanent buildings.
- If a building is always present, it can directly instance `buildingTemplate.tscn` in `Outpost.tscn`.
- If a building is generated/randomized, place only an empty `Node2D` slot in `World/OutpostBuildings` and let `OutpostBuildings.cs` instantiate it.
- For building UI, prefer data-driven changes in `BuildingData.Generate(...)` over adding more logic to `BuildingOverlay.cs`.
- Validate C# changes with `dotnet build`, but still run the Godot scene for node path/editor assignment issues.
