using UnityEngine;
using FSCORD.Core;
using FSCORD.Data;

namespace FSCORD.Gameplay
{
    /// <summary>
    /// A spawnable combat unit driven by a UnitDefinition. Replaces the 658-line
    /// BasicTank.cs god-component with a small, pooled, data-driven unit that owns
    /// only identity, health and death. Movement is delegated to an IUnitMover and
    /// decision-making to TankBrain (which replaces RAIN). Implements ICombatant so
    /// fire missions can damage it and the TargetRegistry / AI can perceive it.
    /// The prefab needs a Collider (for fire-mission overlap queries).
    /// </summary>
    public sealed class TankUnit : MonoBehaviour, IPoolable, ICombatant
    {
        UnitDefinition _def;
        EventBus _bus;
        IUnitMover _mover;
        TargetRegistry _registry;
        float _hitPoints;
        bool _dead;

        public UnitDefinition Definition => _def;
        public Faction Faction => _def != null ? _def.faction : Faction.Neutral;
        public Vector3 Position => transform.position;
        public bool IsAlive => !_dead && _def != null;

        void Awake() => _mover = GetComponent<IUnitMover>();

        public void Init(UnitDefinition definition, EventBus bus, Vector3 destination)
        {
            _def = definition;
            _bus = bus;
            _hitPoints = definition.maxHitPoints;
            _dead = false;
            if (_mover == null) _mover = GetComponent<IUnitMover>();

            ResolveRegistry();
            _registry?.Register(this);

            var brain = GetComponent<TankBrain>();
            if (brain != null) brain.Configure(this, destination, _registry);
            else _mover?.Begin(definition, destination);
        }

        public void ApplyDamage(float amount)
        {
            if (_dead || _def == null) return;
            float mitigated = Mathf.Max(1f, amount - _def.armor);
            _hitPoints -= mitigated;
            if (_hitPoints <= 0f) Die();
        }

        void Die()
        {
            _dead = true;
            GetComponent<TankBrain>()?.Deactivate();
            _mover?.Stop();
            _registry?.Unregister(this);
            _bus?.Publish(new TankDestroyed(_def.faction, _def.xpWorth, transform.position));
            GetComponent<PooledObject>()?.Release();
        }

        public void OnSpawned() => _dead = false;

        public void OnDespawned()
        {
            GetComponent<TankBrain>()?.Deactivate();
            _mover?.Stop();
            _registry?.Unregister(this);
        }

        void ResolveRegistry()
        {
            if (_registry == null && GameBootstrap.Instance != null)
                GameBootstrap.Instance.Services.TryGet(out _registry);
        }
    }
}
