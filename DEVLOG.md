# Dev Log

## $\textcolor{blue}{\textsf{Day 1: Interaction system, Portal, Island layer (2026-10-23)}}$

### $\textcolor{gray}{\textsf{What got built}}$

- Repo pushed to GitHub for the first time (2055tee/2D-Dungeon-Crawler, `main`).

**Core movement/combat (earlier in this session):**
- `PlayerController` — WASD movement (new Input System, `Rigidbody2D`).
- `PlayerAim` — rotates the player to face the mouse cursor.
- `SwordAttack` / `SwordHitbox` — left-click swing with a hitbox (detection only, no damage system yet).
- `CameraFollow` — smooth-damped camera follow.

**Interaction system (generic, reusable foundation):**
- `Interactable` + `InteractionOption` — base class any object derives from to offer a menu of choices.
  Options are either "leaf" (`Action` — runs immediately, e.g. "Fix Portal") or "branch"
  (`Func<List<InteractionOption>>` — opens a nested sub-menu, e.g. a future "Investigate").
- `PlayerInteractor` (on Player) — tracks what's in range, shows the idle prompt, opens the menu on
  **E**, navigates with **W/S**, confirms with **E**, cancels with **Esc**. Supports multi-level
  sub-menus with an auto-injected "Back" entry (navigation history stack). Locks player
  movement/combat (`PlayerInteractor.InputLocked`) while a menu is open.
- `InteractionMenuUI` (on `InteractionCanvas`) — the floating UI: an idle prompt window, a main
  dialogue window, and a pool of small option windows, all world-anchored (follow the target via
  `Camera.WorldToScreenPoint`) and highlight the selected option.

**World objects built with it:**
- `PortalInteractable` (Mainland "Portal", world center) — "Fix Portal" (placeholder: no resource
  cost yet, just a color-fade confirmation), then "Teleport" once fixed.
- `TeleportLink` (generic, reusable one-way teleport point, no fix step) — used to build the Island's
  return portal ("IslandPortal").

**Mainland/Island separation (original version — superseded on Day 2):**
- `WorldLayers` — Unity physics layers 8 ("Mainland") / 9 ("Island"), with
  `Physics2D.IgnoreLayerCollision` set up in `PlayerController.Awake()`. The player's GameObject layer
  switches on every teleport. This fixes a real bug: both portals sit at the same XY (just different
  Z), and Physics2D ignores Z entirely, so without this they cross-triggered each other.
- Mainland is at Z=0 (`Portal`, `Background`), Island is at Z=50 (`IslandPortal`,
  `IslandBackground`, blue). Both are in the *same scene* — not a real scene boundary yet, matching
  the "Mainland is an infinite plane" design (layers separated by Z/physics-layer rather than by
  moving arbitrarily far away in XY, or by a Unity Scene switch).

### $\textcolor{gray}{\textsf{Known placeholders / limitations (intentional for now)}}$

- Portal fix has no resource cost — `GAME_DESIGN.md` has the planned wood/ore costs, not wired up.
- No persistence — portal resets to "Broken" every time Play mode stops.
- No real scene boundary between Mainland/Island — same scene, physics-layer trick only. Colliders
  with the *same* layer sharing XY would still interact; fine today since neither layer has other
  geometry yet.
- No multi-line dialogue / floating feedback text system yet (e.g. for "I need 2 more wood" failure
  messages) — tracked as its own memory/future item, see `project_floating_text_todo` memory.
- Sword hitbox only logs hits (`SwordHitbox`), no damage/health system.

### $\textcolor{gray}{\textsf{Every function is commented}}$

Per standing instruction, every function across all scripts has a one-line comment explaining what it
does — keep doing this for all future code in this project.

---

## $\textcolor{blue}{\textsf{Day 2: Sprite import pipeline, Tilemap, Mainland/Island root restructure (2026-10-26)}}$

### $\textcolor{gray}{\textsf{What got built}}$

- Fixed import settings on all tile/prop sprites (89 files, PPU 256) and the character
  sheet (PPU 16) — Point filter, Uncompressed, no mipmaps, so pixel art renders crisp.
- Re-sliced the character sheet (`Small-8-Direction-Characters_by_AxulArt.png`) into a
  correct 8x12 grid (16x24px cells, bottom-center pivot) — the asset's actual layout is
  3 color variants x (1 arrow-legend row + 3 character pose rows).
- Replaced the old flat stretched `Background` sprite with a real `Grid`/`Tilemap`.
- Assigned a character sprite frame (blue variant, facing down) to the Player's
  SpriteRenderer — static for now, no animation/direction-switching yet.
- Removed the `PlayerAim` component from Player — it continuously rotated the whole
  sprite toward the mouse, which looks wrong on this 8-direction character style
  (needs a flip/direction-swap replacement instead; see To Do).

### $\textcolor{gray}{\textsf{Architecture change: Mainland/Island are now root GameObjects, not Z-offset}}$

- Old approach (physics layers + 50-unit Z offset) is being replaced.
- New structure: `MainLand` and `SkyIsland` root GameObjects, each self-contained
  (own Grid/Tilemap, own portal), meant to be toggled via `SetActive` on teleport.
- `Player`, `Main Camera`, `Global Light 2D`, `InteractionCanvas` stay outside both
  roots (persistent, shared across areas).
- Confirmed in scene: both roots exist with their Grid + portal children.

### $\textcolor{gray}{\textsf{Known gaps / not done yet}}$

- Teleport SetActive toggle logic not wired up — `PortalInteractable.Teleport()` and
  `TeleportLink` still reference the old Z-offset/WorldLayers approach, now stale.
- Portal still uses placeholder sprite (intentionally, until real art is found/made).
- Character is a single static frame — no walk animation, no direction switching.

---

## $\textcolor{blue}{\textsf{Day 3: Tile collision, finer grid, Y-sorting (2026-10-28)}}$

### $\textcolor{gray}{\textsf{What got built}}$

**Collision foundation for painted obstacles:**
- Added `TilemapCollider2D` + `CompositeCollider2D` + a static `Rigidbody2D` to `Obstacle_Layer` —
  this is the master switch that makes every painted obstacle tile solid. Each `Tile` asset's
  `Collider Type` field (`None`/`Sprite`/`Grid`) was already being set correctly, but was inert
  without this component actually generating physics from it.
- Custom physics shapes ("base-only", same idea as a banner's post) set on the 14 tall props —
  trees, house, castles, towers, windmill, tent, banners, flag — so only the trunk/wall/base blocks
  movement, not the canopy/roof. Matches the usual top-down trick of visually walking "behind" a
  tall object.
- `Wooden Bridge Horizontal/Vertical` set to fully walkable (`Collider Type = None`) — they're
  crossings, not obstacles.
- Squat props (rocks, bushes, stumps, campfire, chest, barrel, cart, well, fences) left on full-
  silhouette collision, since they have no overhang worth carving out.
- Prefab-brush workflow (used for the `Decorating.prefab` banner): fixed by adding a
  `PolygonCollider2D` directly on the prefab, which auto-reads the sprite's custom physics shape.
  This is a different system from regular painted tiles — no `TilemapCollider2D` involved, the
  prefab instance carries its own collider.
- Bug found + fixed: a sprite's custom physics shape coordinates are relative to its **trimmed**
  rect (Unity auto-crops transparent padding), not the full PNG canvas — the first pass at these
  shapes used the wrong basis and was misaligned for anything with padding.

**Grid density change (for future decoration detail):**
- Converted the whole world grid from 1 unit/cell to 0.5 unit/cell (2x finer), so future detail
  tiles don't require higher-res art, just more of the existing art at finer placement.
- Re-imported all 89 NormalForest sprites at PPU 512 (previously 256) to match the new cell size —
  every object's real-world size stayed the same, only the grid resolution got finer.
- Considered a secondary "detail-only" grid layered on top instead (non-destructive, smaller
  change) but chose to convert the whole grid now instead, while the project is still small —
  closer to how shipped games lock in one grid resolution up front.

**Pivot/anchor system (a few false starts, resolved):**
- Learned: for "Multiple" sprite mode (sliced sheets, which all these props use), pivot is
  per-sprite-rect data set via `ISpriteEditorDataProvider` — the simpler `TextureImporterSettings`
  API only works for "Single" mode and silently does nothing here, which caused an early fix to
  not actually apply despite looking right in the Sprite Editor.
- Learned: `Tilemap.tileAnchor` controls where *within a cell* a tile's pivot gets placed (default:
  cell center) — this combines with the sprite's own pivot to determine final placement, and both
  have to be reasoned about together.
- Computed and applied precise per-sprite pivot values for the 14 tall props so their trunk/base
  lands exactly on the bottom edge of whichever cell is painted, instead of floating mid-tile.

**Depth sorting (Y-sort) fix:**
- `Obstacle_Layer`'s `TilemapRenderer.mode`: `Chunk` → `Individual`, so each tile sorts
  independently instead of batching as one flat layer with one fixed draw order.
- Project's URP 2D Renderer asset (`Assets/Settings/Renderer2D.asset`): `Transparency Sort Mode`
  → `Custom Axis (0, 1, 0)` — every sprite (player, trees, props) now sorts dynamically by world Y
  position project-wide, so standing in front of/behind overlapping objects reads correctly.

### $\textcolor{gray}{\textsf{Known gaps / not done yet}}$

- Walking animation — didn't get to it this session, top priority for next time.
- Character still feels a bit large relative to the tile grid; a PPU adjustment (16 → ~24) was
  discussed but not applied, got sidetracked by the grid-density work — revisit together.
- Tree/prop placement isn't symmetric across the path yet (a cluster exists on one side only) —
  this is a painting/placement task, not a bug, just not done yet.
- `SkyIsland` has no `Obstacle_Layer` yet — collision + props there still need to be set up to
  match Mainland whenever it gets decorated.

---

## $\textcolor{orange}{\textsf{To Do}}$

> [!TIP]
> This list always stays at the bottom of the log — add new items here as they come up,
> and check items off (or remove them) once done instead of leaving stale "next steps"
> scattered across old day entries.

- [ ] Build walking animation (direction-based, matching the 8-direction sprite sheet).
- [ ] Revisit character scale — raise PPU from 16 toward ~24 so it reads less oversized on the
      now-finer tile grid.
- [ ] Balance tree/prop placement symmetrically around paths (currently clustered on one side).
- [ ] Set up `SkyIsland`'s `Obstacle_Layer` (+ collision) to match Mainland once it has props.
- [ ] Wire an actual resource requirement into `PortalInteractable.Fix()`.
- [ ] Build the generic floating-text popup utility (see `project_floating_text_todo` memory).
- [ ] Wire the `SetActive(MainLand/SkyIsland)` toggle into the portal teleport code,
      replacing the stale `WorldLayers` Z-offset logic.
- [ ] Build a `PlayerAim` replacement (flip/direction-based facing) to match the
      8-direction sprite style, since the old continuous-rotation version was removed.
- [ ] Paint more tile variety on both Mainland/Island via the Tile Palette.
- [ ] Find or draw a proper Portal sprite.
- [ ] Villages on the Mainland, Dungeon layer, inventory/crafting system — all still
      "not started" per `GAME_DESIGN.md`.
