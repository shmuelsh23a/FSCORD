# FSCORD — Modernized Code Foundation (Unity 6)

This is the **Stage A** code foundation for the FSCORD modernization: a clean,
layered, data-driven rebuild of the game's spine on Unity 6 LTS. It implements the
**Phase A2 core framework** from the master roadmap. It is written to be dropped
into a fresh Unity 6 URP project and to compile as-is.

## How to open

1. In **Unity Hub**, create a new project with **Unity 6 LTS** using the **3D (URP)
   Mobile** template. (Prototype: bundle ID / store keys can wait.)
2. Copy this repo's `Assets/_FSCORD/` folder into the new project's `Assets/`.
3. Add the packages listed in `Packages.reference/manifest.reference.json` via
   **Window > Package Manager** (let Unity resolve versions).
4. Open the project. The four assemblies (`FSCORD.Core`, `FSCORD.Data`,
   `FSCORD.Input`, `FSCORD.Gameplay`) should compile.

> The code deliberately does **not** hard-depend on the Input System / UGS packages
> yet, so it compiles before you add them. Concrete implementations arrive with the
> matching backlog tasks.

## What's implemented (Phase A2 — the spine)

| Area | Files | Purpose |
|------|-------|---------|
| Events | `Core/Events/EventBus.cs`, `GameEvents.cs` | Typed pub/sub — replaces GameObject.Find and cross-script pokes |
| Services | `Core/Services/ServiceLocator.cs`, `IService.cs` | Composition root — replaces static singletons |
| Pooling | `Core/Pooling/PoolService.cs`, `ObjectPool.cs`, `IPoolable.cs` | Reuse instances — kills the Instantiate/Destroy GC stutter |
| State machine | `Core/StateMachine/StateMachine.cs`, `IState.cs` | Match flow — replaces Game.cs boolean flags |
| Boot | `Core/Boot/GameBootstrap.cs` | Creates EventBus + services once, survives scene loads |
| Data | `Data/WeaponDefinition.cs`, `UnitDefinition.cs`, `MatchDefinition.cs` | Content as editable ScriptableObjects |
| Input | `Input/InputContracts.cs` | Interface + intents; gameplay never depends on the input backend |
| Match | `Gameplay/Match/MatchEngine.cs`, `MatchStates.cs`, `MatchContext.cs` | Mode-agnostic combat core driven by a MatchDefinition |

### Why "mode-agnostic" matters
`MatchEngine` plays any `MatchDefinition`. In **Stage B**, a campaign mission, a
roguelite encounter, and an endless battle are all just different MatchDefinitions
(hand-authored or produced by the procedural generator) — no new combat codebase.

## Minimal usage sketch

```csharp
// On a GameObject in the Boot scene: add GameBootstrap.
// To start a match (e.g. from a level loader):
var engine = gameObject.AddComponent<FSCORD.Gameplay.MatchEngine>();
engine.Begin(myMatchDefinition, GameBootstrap.Instance.Events);

// Anywhere, react to gameplay without direct references:
GameBootstrap.Instance.Events.Subscribe<FSCORD.Core.WaveStarted>(e =>
    Debug.Log($"Wave {e.WaveNumber} — {e.EnemyCount} enemies"));
```

## What's NOT here yet (next backlog tasks)
- **A2-7** Concrete Input System gesture recognizer (tap/circle/swipe/X/drag/shake).
- **A2-8** Addressable additive scene loading.
- **A3** Gameplay port: pooled WaveSpawner, AI Navigation movement, Behavior graphs
  (retire RAIN), control points, fog of war, combat resolution, HUD.
- **A4/A5** Save, analytics, remote config, backend, tests, budgets.

See `FSCORD_StageA_Backlog.xlsx` for the full task list and acceptance criteria.

## Structure
```
Assets/_FSCORD/
  Code/
    Core/      (FSCORD.Core.asmdef)      boot, events, services, pooling, FSM, types
    Data/      (FSCORD.Data.asmdef)      ScriptableObject definitions
    Input/     (FSCORD.Input.asmdef)     input contracts + intents
    Gameplay/  (FSCORD.Gameplay.asmdef)  match engine + states
  Data/        (ScriptableObject instances live here)
  Scenes/      (Boot, Menu, Game)
```

---

## A2-7 — Gesture input (implemented)

Concrete Input System recognizer lives in `Code/Input/InputSystem/` as its own
assembly (`FSCORD.Input.InputSystem`). It is gated by `defineConstraints:
["ENABLE_INPUT_SYSTEM"]`, so it only compiles once the Input System package is
added — the rest of the project builds fine before that.

**Gesture map** (thresholds in a `GestureSettings` asset — Create ▸ FSCORD ▸ Gesture Settings):

| Gesture | Weapon | Notes |
|---------|--------|-------|
| Tap | High Explosive | short, near-stationary touch |
| Circle | Concentrated Strike | closed loop, low straightness |
| Swipe (fast, straight) | Napalm | published as a start→end line |
| X (two crossing swipes) | Daisy Cutter | arbitrated within `xMaxStrokeGap` |
| Drag (slow, straight) | Mines | start→end line |
| Shake | Tactical Nuke | filtered accelerometer, cooldown-gated |
| Two-finger pan / pinch / twist | Camera | `CameraPan/Zoom/RotateIntent` |

All produce events on the bus (`FireMissionRequested`, camera intents); gameplay
subscribes and never touches raw input.

**Wiring:**
1. Add the **Input System** package; set **Project Settings ▸ Player ▸ Active Input
   Handling** to **Input System** (or **Both**).
2. Add `TouchGestureInputService` to your Game scene; assign the world `Camera`,
   a `GestureSettings` asset, and the ground Y.
3. Call `Configure(GameBootstrap.Instance.Events, cam)` and `Initialize()` (or let
   it self-resolve the bus + `Camera.main` in `Awake`).

```csharp
GameBootstrap.Instance.Events.Subscribe<FSCORD.Core.FireMissionRequested>(m =>
    Debug.Log($"{m.Weapon} from {m.Start} to {m.End}"));
```

> The X and circle recognizers are heuristic and tunable via `GestureSettings`.
> Napalm carries a small latency (`xMaxStrokeGap`) so a following swipe can be read
> as an X; lower it if you don't want the delay.

---

## A3 (slice) — spawning, units & minimal combat (implemented)

This slice closes the gameplay loop end to end on the new architecture:

```
gesture ─▶ FireMissionRequested ─▶ FireMissionSystem ─▶ TankUnit.ApplyDamage
                                                              │ dies
MatchEngine.SpawnWave ─▶ SpawnWaveRequested ─▶ WaveSpawner    ▼
      ▲                                          (pooled)   TankDestroyed
      └────────────── EnemiesAlive-- ◀───────────────────────┘
                       wave clears ─▶ next wave / MatchEnded
```

New pieces:

| File | Role |
|------|------|
| `Core/Pooling/PooledObject.cs` | Auto-attached on spawn; lets any pooled object return itself |
| `Gameplay/Spawning/WaveSpawner.cs` | Pool-spawns each wave's units over time (A3-7) |
| `Gameplay/Units/TankUnit.cs` | Pooled, data-driven unit (`IPoolable`, `IDamageable`) — replaces BasicTank.cs |
| `Gameplay/Units/IUnitMover.cs` + `SimpleMover.cs` | Movement seam; NavMesh mover swaps in at A3-4 |
| `Gameplay/Combat/FireMissionSystem.cs` | Area damage from fire missions (HE now, others stubbed) |
| `Gameplay/Combat/TimedDespawn.cs` | Returns pooled impact VFX after a lifetime |
| `Gameplay/Match/SpawnEvents.cs` | `SpawnWaveRequested` command event |

### Wire up a test scene
1. **Boot scene:** GameObject with `GameBootstrap`.
2. **Assets (Create ▸ FSCORD ▸ …):** a `UnitDefinition` (assign a tank prefab as
   `modelPrefab` — the prefab needs a **Collider**, and ideally a `TankUnit` +
   `SimpleMover`), a `WeaponDefinition` (kind = High Explosive), a `MatchDefinition`
   with one wave referencing the unit, and a `GestureSettings`.
3. **Game scene:**
   - `WaveSpawner` — assign spawn points + an `objective` transform.
   - `FireMissionSystem` — add your `WeaponDefinition`(s) to its list.
   - `TouchGestureInputService` — assign camera + `GestureSettings`.
   - A tiny starter that calls `matchEngine.Begin(matchDef, GameBootstrap.Instance.Events)`.
4. Play. Tanks spawn and advance; tap to drop HE; clearing every enemy ends the
   match with victory. (In the editor, Simulate touch or use a touchscreen; mouse
   works if you add a mouse→touch shim later.)

> Still stubbed (next tasks): NavMesh movement + Behavior-graph combat AI (A3-4/5,
> retires RAIN), napalm/mine/nuke behaviours, control points, fog of war, HUD.

---

## A3-4 / A3-5 — movement & AI (RAIN retired)

The new project carries **no RAIN** at all — the discontinued middleware is simply
not brought across. Its job (per-unit decisions) is now done by clean, testable code.

**Perception (Core):** `ICombatant` + `TargetRegistry`. Units register on spawn and
unregister on death; AI asks the registry for the nearest enemy in range — no
per-unit physics scans, cheap on mobile.

**Movement:** `IUnitMover` with two implementations — `SimpleMover` (transform, works
before a NavMesh exists) and `NavMeshMover` (built-in `NavMeshAgent`). Choose per
prefab; no other code changes. Baking the surface uses the AI Navigation package
(`NavMeshSurface`).

**Decision-making:** `TankBrain` — a small utility AI tuned entirely by
`UnitDefinition`: advance to the objective → when a living enemy enters weapon range,
stop, rotate onto it and fire on the unit's rate of fire → resume when the target is
gone. Add it (plus a mover) to a unit prefab; `TankUnit.Init` configures it
automatically.

**Behavior-graph seam (optional):** `Code/Gameplay/AI/Behavior/` is a ready assembly
that only compiles when the **com.unity.behavior** package is installed. Author
visual-graph nodes there (AcquireTarget / AdvanceToObjective / FireAtTarget /
Retreat) over the same primitives when you want designer-authored tactics. Node code
is deliberately not pre-written (the Behavior API varies by version).

> To see tanks fight, spawn both factions (give some units `Faction = American`); the
> brain targets the opposing faction generically. With only Soviet spawns they advance
> to the objective and you clear them with fire missions.
