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

## $\textcolor{orange}{\textsf{To Do}}$

> [!TIP]
> This list always stays at the bottom of the log — add new items here as they come up,
> and check items off (or remove them) once done instead of leaving stale "next steps"
> scattered across old day entries.

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
