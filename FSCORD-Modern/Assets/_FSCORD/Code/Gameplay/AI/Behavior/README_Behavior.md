# Unity Behavior graph seam (optional)

`TankBrain` (in `Code/Gameplay/AI`) is the runtime AI and needs no package.

This assembly only compiles when the **com.unity.behavior** package is installed
(guarded by `defineConstraints: ["FSCORD_BEHAVIOR"]`). Add custom Behavior
action/condition nodes here — authored against the installed package version — that
call the same primitives the brain uses:

- **AcquireTarget** → `TargetRegistry.FindNearestEnemy(...)`
- **AdvanceToObjective** → `IUnitMover.SetDestination(...)`
- **FireAtTarget** → `ICombatant.ApplyDamage(...)` on cooldown
- **Retreat / TakeCover** (archetype-driven)

Node code is intentionally not pre-written here because the Behavior API varies by
package version; generate it once the package is pinned.
