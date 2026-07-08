using System.Collections.Generic;
using UnityEngine;
using FSCORD.Core;
using FSCORD.Data;

namespace FSCORD.Gameplay
{
    /// <summary>
    /// Fire-support resolution (backlog A3-10; High Explosive first). Subscribes to
    /// FireMissionRequested from the input layer and applies area damage to enemy
    /// combatants, spawning a pooled impact VFX. Napalm burn-over-time, mine
    /// placement, the slow-motion concentrated strike and distinct nuke/daisy
    /// behaviour are layered on later; today every weapon does a data-driven area
    /// detonation so the full input -> combat -> wave-clear loop works end to end.
    /// </summary>
    public sealed class FireMissionSystem : MonoBehaviour
    {
        [SerializeField] List<WeaponDefinition> weapons = new();
        [SerializeField] LayerMask damageableLayers = ~0;

        EventBus _bus;
        PoolService _pool;
        readonly Dictionary<WeaponKind, WeaponDefinition> _byKind = new();
        readonly Collider[] _hits = new Collider[64];

        void Awake()
        {
            foreach (var w in weapons)
                if (w != null) _byKind[w.kind] = w;
        }

        void Start()
        {
            if (GameBootstrap.Instance != null)
            {
                _bus = GameBootstrap.Instance.Events;
                GameBootstrap.Instance.Services.TryGet(out _pool);
            }
            _bus?.Subscribe<FireMissionRequested>(OnFireMission);
        }

        void OnDestroy() => _bus?.Unsubscribe<FireMissionRequested>(OnFireMission);

        void OnFireMission(FireMissionRequested m)
        {
            if (!_byKind.TryGetValue(m.Weapon, out var weapon)) return;

            switch (m.Weapon)
            {
                case WeaponKind.Napalm:
                case WeaponKind.Mines:
                    // TODO(A3-10): proper line placement + burn/stop over time.
                    Detonate(m.Start, weapon);
                    Detonate(m.End, weapon);
                    break;
                default:
                    Detonate(m.Start, weapon);
                    break;
            }
        }

        void Detonate(Vector3 point, WeaponDefinition weapon)
        {
            int count = Physics.OverlapSphereNonAlloc(point, weapon.areaRadius, _hits, damageableLayers);
            for (int i = 0; i < count; i++)
            {
                var target = _hits[i].GetComponentInParent<ICombatant>();
                if (target != null && target.IsAlive && target.Faction == Faction.Soviet)
                    target.ApplyDamage(weapon.damage);
            }

            if (weapon.impactVfxPrefab != null && _pool != null)
            {
                var go = _pool.Spawn(weapon.impactVfxPrefab, point, Quaternion.identity);
                var td = go.GetComponent<TimedDespawn>();
                if (td == null) td = go.AddComponent<TimedDespawn>();
                td.lifetime = 2f;
            }
        }
    }
}
