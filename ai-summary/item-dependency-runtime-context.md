# Item Dependency / Runtime Stats Context

## Dependency checks

- Item dependency checks are centered on the non-static `ItemDependency.CanExecute(ItemUseContext context = null, PlayerRuntimeStats playerStats = null)` instance method.
- If no context/runtime stats are passed, dependency context is hydrated from `SaveNode.Get()` using the current `RunData`, `PlayerData`, skills, equipment, and inventory.
- `CanExecute` should stay as the single dependency-check entry point instead of adding static/save-specific overloads.

## Runtime stats

- `PlayerRuntimeStats` is the runtime player stat container intended to live on the player later.
- It tracks current health/mana/stamina, current equipment, active hand item, equipped ammo, consumable, and a single stored ammo item resolved from player inventory.
- Max/default stat helpers currently derive from skills:
  - health from vitality
  - mana from arcana
  - stamina from agility

## Cost mutation

- `ItemUsable.CanUse(ItemUseContext? context = null, PlayerRuntimeStats? playerStats = null)` checks whether an item can be used.
- `ItemUsable.TryUse(ItemUseContext? context = null, PlayerRuntimeStats? playerStats = null)` performs the use and applies costs.
- `ItemDependency.ApplyCosts(ActionContext context)` prepares the item context before applying dependency costs.
- Resource costs should mutate the provided `PlayerRuntimeStats` when present:
  - mana dependencies reduce `CurrentMana`
  - stamina dependencies reduce `CurrentStamina`
  - ammo dependencies consume the resolved `ItemAmmo`

## Inventory / equipment nullability

- Equipment slots and inventory item slots may be `null`; this means the slot is empty and should not be treated as an error by itself.
- Null entries in `InventoryData.Items` should be ignored when resolving stored ammo.
- Ammo is expected to resolve to one item instance, either equipped ammo or a single stored ammo item in the player inventory.
- Missing equipment/items in slots is valid; errors should only be printed when required container data is missing or a dependency requires a specific resource, such as ammo, and it cannot be found.
