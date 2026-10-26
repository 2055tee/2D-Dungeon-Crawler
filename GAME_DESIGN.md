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
  used before real art is imported for a given object.
- **World scale: 1 tile = 1 world unit.** Every sprite's Pixels Per Unit (PPU)
  is set to match *that asset's own* native pixel density so it lands on that
  grid — PPU is **not** the same number across every sprite, it's chosen per
  asset so 1 world unit means the same physical size everywhere:
  - `Assets/Sprites/Scenes/NormalForest/*` (tiles + props, natively 256px per
    tile) → PPU 256. A ground tile is then exactly 1x1 world units.
  - `Assets/Sprites/Character/Swordman1/*` (natively ~16px-tall character art,
    8-direction sheet sliced into a 16x24px grid, bottom-center pivot) → PPU
    16. Comes out ~1 unit tall standing on a 1-unit tile.
  - Any new pixel-art import: PPU = (that sprite's native pixel size) /
    (desired size in world units), not copied from another asset's PPU.
- All pixel-art texture imports use **Filter Mode = Point (no filter)**,
  **Compression = Uncompressed**, and mipmaps disabled — bilinear filtering
  and compression both blur/artifact hard pixel edges.
- The Mainland floor is a real `Grid`/`Tilemap` (`MainlandGrid` →
  `GroundTilemap`, cell size 1x1, sorting order -10) painted with a `Tile`
  asset (`Assets/Sprites/Scenes/GroundTile01.asset`) wrapping one of the
  NormalForest ground sprites — not a single giant stretched sprite. Paint
  more variety by hand via Unity's Tile Palette window, or extend the painted
  area/add more `Tile` assets per ground variant via `Unity_RunCommand`.
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
