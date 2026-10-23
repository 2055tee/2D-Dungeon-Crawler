# Game Design

## Overview

A top-down 2D action/building game (Unity 6, URP 2D, new Input System only).

## World layers

### Mainland
Where the player spawns. Explore to find villages and a broken portal near
the map's center. Basic resources (e.g. wood) are available on the surface.

### Island
The player's personal base for building and decorating. Only reachable after
fixing the Mainland portal.

### Dungeon
Contains monsters and ores. Monsters drop items used to craft weapons and
fighting skills. Ores are used for building, crafting, and reinforcing. Some
materials needed for building exist only here — basic resources like wood
remain surface-only (Mainland).

## Progression loop

1. Spawn on the **Mainland**, explore, find the broken portal at its center.
2. Fix the portal to unlock the personal **Island**.
3. Gather basic resources (wood, etc.) on the Mainland surface.
4. Delve into the **Dungeon** for ores and monster drops — materials exclusive
   to building/crafting/reinforcing that aren't available on the surface.
5. Build and decorate the Island using resources from both layers.

## Resource ownership

| Resource         | Source layer | Used for                              |
|-------------------|--------------|----------------------------------------|
| Wood (basic)       | Mainland     | Basic building                         |
| Ores               | Dungeon      | Building, crafting, reinforcing        |
| Monster drops       | Dungeon      | Crafting weapons, fighting skills      |
| Exclusive materials | Dungeon      | Required for some buildings            |

## Current implementation status

**Done:**
- Player movement (WASD, new Input System, `Rigidbody2D`)
- Mouse aim (player rotates to face cursor)
- Sword swing + hitbox (detection only, no damage system yet)
- Camera follow (smooth-damped)
- Mainland portal interaction (placeholder — see below)

**Done (placeholder):**
- Portal fix: no resource cost yet, no transition to an Island scene yet
  (that layer doesn't exist). Just the interaction system itself.

**Not started:**
- Inventory / crafting
- Damage / health system
- Villages
- Island scene
- Dungeon scene
- Scene switching / level loading

Note: `Assets/Scenes/SampleScene.unity` currently *is* the Mainland.

## Systems conventions

- Procedural placeholder sprites use a local `MakeSolidSprite(color, size,
  pixelsPerUnit)` helper (`Sprite.Create` with `SpriteMeshType.FullRect`) —
  no image assets yet.
- Only the new Input System is used (`Keyboard.current`, `Mouse.current`,
  etc.) — the legacy Input Manager is disabled project-wide.
- Scene edits are made live via Unity MCP's `Unity_RunCommand` (compiled C#
  run against the open Editor in Edit mode), not by hand-editing scene YAML.
- Interactable objects derive from `Interactable` and expose a list of
  `InteractionOption`s — adding a new option to an object (e.g. "Investigate")
  is a one-line change, not a restructure.
- Triggers identify the player via `other.attachedRigidbody`'s component,
  after filtering out other trigger colliders (e.g. the sword's hitbox) —
  no GameObject tags are used in this project.

## Open questions / future work

- What should fixing the portal actually cost (materials, time)?
- What does the "Investigate" option (or others) show/do?
- Island and Dungeon entry points/transitions.
- How are villages generated or placed on the Mainland?
