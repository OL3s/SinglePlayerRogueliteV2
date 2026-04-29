# Item System Goal

This note captures the intended direction for the item system beyond the current implementation.

## Core Intent

Items should be generic data-driven resources. The system should not hardcode many special cases for each item type. Instead, each item should describe what it is and what it requires.

An item should be able to answer these questions:

- Is it consumable or non-consumable?
- Does it require certain player skill levels?
- Does it require mana?
- Does it require ammo?
- Does it require other resources or conditions in general?
- Can the player currently use or equip it?

## Consumable Direction

Consumable status should be a generic item concept, not only a separate hardcoded path.

Current code has `ItemConsumable`, `ItemAmmo`, and generic count fields on `ItemBase` like `UseCountCurrent`, `UseCountDefault`, and `UseCountMax`.

Consumable means the item has a limited use count. It needs:

- A maximum use count before it is empty.
- A default starting count for when the item is bought, found, generated, or otherwise created.
- A current count while the item exists in inventory/equipment/save data.

Important design point: the game should be able to tell if using an item consumes one use, spends a charge, spends ammo, spends mana, or only checks requirements without consuming the item itself.

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

## General Dependencies

Dependencies should become the generic requirement system for item use/equip checks.

Current dependency examples:

- `DependencyAmmo`: requires a specific ammo type.
- `DependencyMana`: requires enough mana.
- `DependencyLevel`: requires a specific player skill level.

The intended direction is that item checks should ask a dependency container whether the current player/context satisfies all requirements.

Example concept:

```text
Item
-> Requirements
   -> Skill requirement
   -> Mana requirement
   -> Ammo requirement
   -> Other future requirement
-> CanUse(context) / CanEquip(context)
```

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

## Important Implementation Caution

The current project still has some existing names with typos, especially `EquipedItemsData`. Preserve existing names unless doing a deliberate project-wide rename. `ItemDepencency` was deliberately renamed to `ItemDependency` for readability.

When implementing this system, avoid overbuilding. Start with the smallest useful requirement check that supports current gameplay needs, but keep it generic enough that new requirements can be added as dependency resources.
