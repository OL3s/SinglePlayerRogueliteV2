# Item System Memory

Internal AI memory for the item system. Keep this aligned with current code and intended direction.

## Core Intent

Items should be generic data-driven resources. Avoid hardcoding special cases into each item type. An item should describe what it is, what state it has, and what dependencies it requires.

An item should be able to answer these questions:

- Is it consumable or non-consumable?
- Does it require certain player skill levels?
- Does it require mana?
- Does it require stamina?
- Does it require ammo?
- Does it require other resources or conditions in general?
- Can the player currently use it?

Prefer enums over strings for categories, types, and fixed option sets. This applies to item types, ammo types, skills, locations, biomes, codex categories, and similar values.

## Consumable Direction

Current code has `ItemConsumable`, `ItemAmmo`, and generic count fields on `ItemBase` like `UseCountCurrent`, `UseCountDefault`, and `UseCountMax`.

Consumable means the item has a limited use count. It needs:

- A maximum use count before it is empty.
- A default starting count for when the item is bought, found, generated, or otherwise created.
- A current count while the item exists in inventory/equipment/save data.

Important design point: the game should be able to tell if using an item consumes one use, spends a charge, spends ammo, spends mana, or only checks requirements without consuming the item itself.

Use `ItemBase.TryUse(ItemUseContext context)` when values need to actually change. The context is mutable and should be read after `TryUse` to get updated mana/stamina values.

Important structure: item use now builds on a shared action model.

- `ActionContext` is the shared runtime state for generic player actions and generic dependencies.
- `ItemUseContext` inherits from `ActionContext` and adds item-specific fields like `Item`, `AmmoItem`, `AmmoType`, `UseCountCost`, and `ConditionDamage`.
- `PlayerAction` is the reusable effect/execution resource.
- `ItemUsable` is the shared base for item types that can actually be used. `PlayerAction` belongs on `ItemUsable`, not on `ItemBase`.
- `Dependency` remains the generic requirement/cost layer.

This means item-specific behavior relies on shared action/dependency abstractions, while still keeping item wear, ammo usage, and condition damage in the item-specific context.

Keep this flow simple: `CanUse(context)` checks only, and `TryUse(context)` is the single public item-use mutation path. Avoid duplicate `TryUse` overloads or separate public consumable-use flows unless there is a concrete gameplay need.

## Condition / Durability

Items may optionally have condition. Condition is the item's health/durability.

Not every item needs condition. The item system should support both:

- Items with no durability at all.
- Items with condition values that can decrease through use, combat, or other events.

Condition should include enough data to know the item's maximum condition, starting/default condition when generated, and current condition while owned.

Do not assume all equipment has condition and do not assume only equipment can have condition unless the design later confirms that.

## Ammo

Ammo items must include an ammo type.

The ammo type is required so weapons or abilities can check whether the equipped/available ammo matches their dependency. For example, a bow can require `AmmoType.Arrow`.

Ammo can also behave like a consumable because it has a count that decreases as it is used.

## Skill Requirements

Items should support skill-level requirements.

Examples:

- Sword requires `Strength >= 5`.
- Bow requires `Agility >= 4`.
- Staff requires `Arcana >= 6`.
- Heavy armor requires `Vitality >= 3`.

The current `DependencyLevel` class represents a specific player skill and required level.

Important: skill checks compare levels, not raw XP. Use the explicit getters like `GetStrengthLevel()`, `GetAgilityLevel()`, `GetArcanaLevel()`, and `GetVitalityLevel()`.

## General Dependencies

Dependencies are the generic requirement/cost system for action and item-use checks.

Current dependency examples:

- `DependencyAmmo`: requires a specific ammo type.
- `DependencyMana`: requires enough mana.
- `DependencyStamina`: requires enough stamina.
- `DependencyLevel`: requires a specific player skill level.

The intended direction is that item checks ask `ItemDependency` whether the current use context satisfies all requirements. Shared dependencies should depend on `ActionContext`. Item-specific dependencies may cast to `ItemUseContext` when they need item-only data such as ammo state.

Example concept:

```text
Item
-> Requirements
   -> Skill requirement
   -> Mana requirement
   -> Stamina requirement
   -> Ammo requirement
   -> Other future requirement
-> CanUse(context)
-> TryUse(context) applies costs and item wear
```

## Actions

`PlayerAction` is the reusable execution unit for something the player does. The first item integration keeps `PlayerAction` separate from dependencies.

Reasoning:

- dependencies answer whether use is allowed and what it costs
- actions answer what actually happens on success
- this avoids bloating dependencies with gameplay effects
- this also keeps item use compatible with future non-item player-triggered actions like parry

Important rule: do not move item-specific wear or use-count logic into `PlayerAction`. Charges, condition, and similar item state stay under item-use handling.

Current placement rule:

- `ItemBase`: generic item data that all items share
- `ItemUsable`: shared use flow plus optional `PlayerAction`
- `ItemEquipable` and `ItemConsumable`: current usable item types

Do not put `PlayerAction` directly on `ItemBase` unless every item type is intended to be usable.

## Use Flow

Public item-use API should stay small:

- `ItemBase.CanUse(ItemUseContext context)`: checks only, no mutation.
- `ItemBase.TryUse(ItemUseContext context)`: checks, applies dependency costs, executes `PlayerAction`, then applies item wear.

Dependency check/cost helpers are internal implementation details. Do not bypass `TryUse` from gameplay code.

`TryUse` should be used for actual gameplay actions such as attacking with a weapon, using armor effects, drinking a potion, or firing a bow. It should:

- Check all dependencies first.
- Check the assigned `PlayerAction` if there is one.
- Spend mana if there is a mana dependency.
- Spend stamina if there is a stamina dependency.
- Consume required ammo if an ammo dependency has an ammo item in the context.
- Execute the assigned `PlayerAction`.
- Reduce the item's use count if the item is consumable.
- Reduce the item's condition if the use context asks for condition damage.

For example, armor can use `TryUse` with `ConditionDamage` when it absorbs damage. A weapon can use `TryUse` with stamina/mana/ammo dependencies and condition damage when attacking.

`ItemUseContext` is the state of one use attempt. It inherits shared player-action state from `ActionContext`, then adds item-specific state such as player skills, mana, stamina, ammo type/item, use-count cost, and condition damage. If mana/stamina are changed by dependencies, read the updated values back from the same context after `TryUse` succeeds.

Execution order matters:

1. generic item state checks
2. dependency checks
3. action availability check
4. dependency cost application
5. action execution
6. item use-count and condition mutation

This keeps dependencies as the gate/cost layer and actions as the effect layer, while preventing item wear from being applied before the action succeeds.

## Future Extensibility

The dependency system should stay open-ended. Future dependencies might include:

- Required biome or location.
- Required item in inventory.
- Required weapon type.
- Required character status.
- Required building upgrade.
- Required run progression.
- Cooldown or charge availability.

Prefer adding new dependency resource types over adding many item-specific conditional branches.

Keep dependency classes small:

- `IsMet(context)` checks if the requirement is satisfied.
- `ApplyCost(context)` mutates the context or related item only for dependencies that spend something.
- Most dependency internals should remain `internal`; item use should go through `ItemBase`.

## Important Implementation Caution

The current project still has some existing names with typos, especially `EquipedItemsData`. Preserve existing names unless doing a deliberate project-wide rename. `ItemDepencency` was deliberately renamed to `ItemDependency` for readability.

When implementing this system, avoid overbuilding. Start with the smallest useful requirement check that supports current gameplay needs, but keep it generic enough that new requirements can be added as dependency resources.
