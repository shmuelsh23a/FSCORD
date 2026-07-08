using UnityEngine;
using FSCORD.Core;
using FSCORD.Data;

namespace FSCORD.Gameplay
{
    /// <summary>
    /// Drives a match from a MatchDefinition using a small state machine and the
    /// EventBus. This is the mode-agnostic combat core: the same engine runs a
    /// campaign mission, a roguelite encounter or an endless battle - the only
    /// difference is which MatchDefinition it is handed. Spawning and combat
    /// resolution are wired in during the gameplay port (backlog A3).
    /// </summary>
    public sealed class MatchEngine : MonoBehaviour
    {
        readonly StateMachine _fsm = new();
        MatchContext _ctx;
        EventBus _bus;

        public MatchContext Context => _ctx;
        public EventBus Bus => _bus;

        /// <summary>Start a match. Pass the shared EventBus from GameBootstrap.</summary>
        public void Begin(MatchDefinition definition, EventBus bus)
        {
            _bus = bus;
            _ctx = new MatchContext { Definition = definition };
            _bus.Subscribe<TankDestroyed>(OnTankDestroyed);
            _fsm.Change(new IntroState(this));
        }

        void Update()
        {
            if (_ctx != null && !_ctx.IsOver) _fsm.Tick(Time.deltaTime);
        }

        internal void Go(IState next) => _fsm.Change(next);

        /// <summary>Advance to the next wave and spawn it. Returns false when none remain.</summary>
        internal bool TryStartNextWave()
        {
            int next = _ctx.CurrentWaveIndex + 1;
            if (next >= _ctx.Definition.waves.Count) return false;
            _ctx.CurrentWaveIndex = next;
            SpawnWave(next);
            return true;
        }

        void SpawnWave(int index)
        {
            var wave = _ctx.Definition.waves[index];
            _ctx.EnemiesAlive += wave.Count;
            _bus.Publish(new WaveStarted(index + 1, wave.Count));
            _bus.Publish(new SpawnWaveRequested(index, wave)); // WaveSpawner instantiates the units
        }

        internal void EndMatch(bool victory)
        {
            _bus.Unsubscribe<TankDestroyed>(OnTankDestroyed);
            _ctx.IsOver = true;
            _ctx.Victory = victory;
            _bus.Publish(new MatchEnded(victory));
        }

        void OnTankDestroyed(TankDestroyed e)
        {
            if (e.Faction != Faction.Soviet) return;
            _ctx.EnemiesAlive--;
            _ctx.EnemiesDestroyed++;
        }
    }
}
