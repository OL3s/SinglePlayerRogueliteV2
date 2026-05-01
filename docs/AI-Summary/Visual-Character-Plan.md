# Visual Character Plan

Internal AI memory for the planned ingame character visual structure.

## Direction

Use a loose procedural `Node2D` puppet instead of starting with `Skeleton2D`.

This means the player scene should keep gameplay and visuals separate:

- `CharacterBody2D` for movement/collision/game logic
- a child visual root for body-part transforms and equipment visuals

## Recommended Scene Structure

Suggested visual hierarchy:

- `VisualRoot: Node2D`
- `Body: Node2D`
- `Head: Node2D`
- `LeftArmPivot: Node2D`
- `RightArmPivot: Node2D`
- `LeftLegPivot: Node2D`
- `RightLegPivot: Node2D`

Each pivot/body part can then own a `Sprite2D` child.

Equipment and armor visuals should attach to the relevant body-part nodes instead of being baked into one static player sprite.

## Animation Approach

Preferred first implementation is math-driven procedural animation:

- body bob from `sin(time * speed)`
- arm and leg swing from movement intensity
- head lag / follow using smoothing
- body tilt from movement direction
- `lerp` and `lerp_angle` for smooth transitions

This is preferred over authored frame animation for the current project direction because it is lighter to maintain and fits the desired dynamic/loose feel.

## Equipment Visuals

The project currently separates item icon and equipped/world texture through item resources.

Longer term direction should be a small presentation/visual layer for equipment so the same source art can drive:

- inventory icon
- equipped/world-held item visual
- armor body-part overlays

Armor should be split by wearable visual parts where needed instead of assuming one icon texture can represent all ingame body parts.

## SVG Direction

SVG should be treated as the source art format.

Preferred usage:

- keep one SVG source for an item or equipment design
- reuse that source for inventory and ingame presentation assets
- use imported `Texture2D` outputs in Godot scenes/resources rather than depending on runtime SVG manipulation for gameplay visuals

This allows the same visual language across UI and ingame rendering while still supporting separate body-part overlays for armor and held-item visuals.

## Implementation Preference

When this gets built, prefer:

1. `Node2D` puppet hierarchy first
2. procedural math-based animation first
3. equipment visual resource/data second
4. more advanced systems like bones/IK only if the simpler setup becomes limiting
