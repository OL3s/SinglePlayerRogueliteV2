# Save / Run / Outpost Context

Context notes for future AI sessions. Verify against code before relying on old implementation details.

## SaveNode flow

- `SaveNode.RunData` is the active run state.
- `SaveNode.WipeRun()` is the central reset path for starting a new run.
- `WipeRun()`:
  - replaces `RunData` with a fresh `RunData`
  - increments `MetaData.RunCount`
  - saves metadata immediately, but does not save the new run by itself

## Player / inventory ownership

- Inventory is intended to be player-owned, not run-owned.
- `InventoryData` belongs on `PlayerData`.
- `SaveNode.InventoryData` should resolve through `PlayerData.InventoryData`.
- `EquipedItemsData` belongs on `PlayerData` and represents current equipment slots.
- Equipment slots and inventory item slots may be `null`; this means the slot is empty and should not be treated as an error by itself.
- New character selection should:
  - call `WipeRun()`
  - assign the selected duplicated `PlayerData`
  - initialize that player's inventory
  - add duplicated starting item into that player inventory

## Outpost generation

- Outpost state is stored in `RunData.OutpostData`.
- Outpost buildings are stored in `RunData.OutpostData.Buildings`.
- The array is slot-based, not compacted:
  - `null` means empty slot
  - non-null `BuildingData` means that slot spawns a building
- New outposts should generate a random count from `0..available`
- Building types are selected from the configured resource pool without replacement.
- Chosen buildings are placed into random slots without replacement.
- If saved outpost data is missing or has the wrong slot count, it should be regenerated.

Example layouts:
- `E, E, E`
- `E, B, C`
- `A, E, B`

## Building resources

Current test building resources live under `core/buildings/data/`:
- `blacksmith.tres`
- `jeweler.tres`
- `general_goods.tres`

These are intended to be the reusable source pool for outpost generation.

## Storefront behavior

- Shared storefront UI is handled by `StorefrontOverlay`.
- `BuildingData` contains `StorefrontItems`.
- Storefront preview shows:
  - selected item preview
  - price above the purchase button
  - purchase button disabled when funds are insufficient

## Contract completion

- `SaveNode.CompleteContract()` is the centralized transition for finishing a contract.
- It uses `RunData.CurrentContract` and then:
  - sets `RunData.CurrentBiome = contract.Biome`
  - increments `RunData.ContractsCompleted`
  - increments or resets `RunData.CurrentBiomeContractsCompleted` depending on whether the biome changed
  - clears `RunData.CurrentContract`
  - clears `RunData.OutpostData`
  - saves run data
- It does not currently update `RunData.CurrentLocation`.
- Clearing `OutpostData` is intentional so the next outpost is treated as new and regenerated.

## Run overview rule

- `New Run` button should only be visible when:
  - there is no saved player, or
  - `ContractsCompleted >= 1`

This prevents fast rerolling before completing at least one contract.
