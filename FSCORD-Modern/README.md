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
