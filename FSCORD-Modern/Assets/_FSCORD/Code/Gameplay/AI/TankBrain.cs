using UnityEngine;
using FSCORD.Core;
using FSCORD.Data;

namespace FSCORD.Gameplay
{
    /// <summary>
    /// The unit AI that replaces the discontinued RAIN middleware. A small, cheap,
    /// data-driven utility brain: advance toward the objective; when a living enemy
    /// comes within weapon range, stop, turn onto it and fire on the unit's rate of
    /// fire; resume advancing when the target is gone. Perception comes from the
    /// TargetRegistry (no per-unit physics scans). Behaviour is tuned entirely by
    /// UnitDefinition (range, rateOfFire, damage, turretTurnSpeed, archetype).
    ///
    /// This is the runtime brain today. A Unity Behavior graph can later orchestrate
    /// the same primitives (advance / acquire / fire / retreat) for designer-authored
    /// tactics - see Code/Gameplay/AI/Behavior - without replacing this logic.
    /// </summary>
    [RequireComponent(typeof(TankUnit))]
    public sealed class TankBrain : MonoBehaviour
    {
        [SerializeField] float retargetInterval = 0.3f;

        TankUnit _unit;
        IUnitMover _mover;
        UnitDefinition _def;
        TargetRegistry _registry;
        Vector3 _objective;

        ICombatant _target;
        float _retargetTimer;
        float _fireCooldown;
        bool _active;

        public void Configure(TankUnit unit, Vector3 objective, TargetRegistry registry)
        {
            _unit = unit;
            _def = unit.Definition;
            _objective = objective;
            _registry = registry;
            _mover = GetComponent<IUnitMover>();
            _mover?.Begin(_def, objective);

            _active = true;
            _target = null;
            _retargetTimer = 0f;
            _fireCooldown = 0f;
        }

        public void Deactivate()
        {
            _active = false;
            _target = null;
        }

        void Update()
        {
            if (!_active || _def == null || _unit == null || !_unit.IsAlive) return;

            float dt = Time.deltaTime;

            _retargetTimer -= dt;
            if (_retargetTimer <= 0f)
            {
                _retargetTimer = retargetInterval;
                _target = _registry != null
                    ? _registry.FindNearestEnemy(_unit.Faction, transform.position, _def.range)
                    : null;
            }

            if (_target != null && _target.IsAlive)
            {
                Vector3 to = _target.Position - transform.position;
                to.y = 0f;
                if (to.sqrMagnitude <= _def.range * _def.range)
                {
                    _mover?.Stop();
                    if (to.sqrMagnitude > 0.0001f)
                    {
                        var look = Quaternion.LookRotation(to);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, _def.turretTurnSpeed * dt);
                    }
                    _fireCooldown -= dt;
                    if (_fireCooldown <= 0f)
                    {
                        _target.ApplyDamage(_def.damage);
                        _fireCooldown = _def.rateOfFire > 0f ? 1f / _def.rateOfFire : 1f;
                        // TODO(A3-10): pooled muzzle flash + tracer/projectile travel.
                    }
                    return;
                }
            }

            _mover?.SetDestination(_objective);
        }
    }
}
