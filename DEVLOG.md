# Dev Log

## Session: Interaction system, Portal, Island layer (2026-10-23)

### What got built

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

**Mainland/Island separation:**
- `WorldLayers` — Unity physics layers 8 ("Mainland") / 9 ("Island"), with
  `Physics2D.IgnoreLayerCollision` set up in `PlayerController.Awake()`. The player's GameObject layer
  switches on every teleport. This fixes a real bug: both portals sit at the same XY (just different
  Z), and Physics2D ignores Z entirely, so without this they cross-triggered each other.
- Mainland is at Z=0 (`Portal`, `Background`), Island is at Z=50 (`IslandPortal`,
  `IslandBackground`, blue). Both are in the *same scene* — not a real scene boundary yet, matching
  the "Mainland is an infinite plane" design (layers separated by Z/physics-layer rather than by
  moving arbitrarily far away in XY, or by a Unity Scene switch).

### Known placeholders / limitations (intentional for now)
- Portal fix has no resource cost — `GAME_DESIGN.md` has the planned wood/ore costs, not wired up.
- No persistence — portal resets to "Broken" every time Play mode stops.
- No real scene boundary between Mainland/Island — same scene, physics-layer trick only. Colliders
  with the *same* layer sharing XY would still interact; fine today since neither layer has other
  geometry yet.
- No multi-line dialogue / floating feedback text system yet (e.g. for "I need 2 more wood" failure
  messages) — tracked as its own memory/future item, see `project_floating_text_todo` memory.
- Sword hitbox only logs hits (`SwordHitbox`), no damage/health system.

### Next likely steps
- Wire an actual resource requirement into `PortalInteractable.Fix()`.
- Build the generic floating-text popup utility (see memory).
- Decide on real scene-switching (vs. staying with the Z/layer trick) once Island/Dungeon get real
  content with their own colliders.
- Villages on the Mainland, Dungeon layer, inventory/crafting system — all still "not started" per
  `GAME_DESIGN.md`.

### Every function is commented
Per standing instruction, every function across all scripts has a one-line comment explaining what it
does — keep doing this for all future code in this project.
