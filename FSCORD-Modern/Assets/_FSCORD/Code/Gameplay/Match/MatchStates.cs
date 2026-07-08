using FSCORD.Core;

namespace FSCORD.Gameplay
{
    /// <summary>Brief intro beat, then hand off to the first wave.</summary>
    internal sealed class IntroState : IState
    {
        readonly MatchEngine _m;
        float _t;
        public IntroState(MatchEngine m) { _m = m; }
        public void Enter()
        {
            _m.Bus.Publish(new MatchStarted(_m.Context.Definition.matchId));
            _t = 0f;
        }
        public void Tick(float dt)
        {
            _t += dt;
            if (_t < 0.5f) return;
            _m.TryStartNextWave();
            _m.Go(new PlayingState(_m));
        }
        public void Exit() { }
    }

    /// <summary>Active combat. Watches the time limit and wave-clear condition.</summary>
    internal sealed class PlayingState : IState
    {
        readonly MatchEngine _m;
        public PlayingState(MatchEngine m) { _m = m; }
        public void Enter() { }
        public void Tick(float dt)
        {
            var ctx = _m.Context;
            ctx.Elapsed += dt;

            if (ctx.Definition.timeLimit > 0f && ctx.Elapsed >= ctx.Definition.timeLimit)
            {
                _m.Go(new EndState(_m, victory: false));
                return;
            }

            if (ctx.EnemiesAlive <= 0)
            {
                if (ctx.AllWavesSpawned) _m.Go(new EndState(_m, victory: true));
                else _m.Go(new WaveBreakState(_m));
            }
        }
        public void Exit() { }
    }

    /// <summary>Pause between waves, then spawn the next one.</summary>
    internal sealed class WaveBreakState : IState
    {
        readonly MatchEngine _m;
        float _t;
        readonly float _delay;
        public WaveBreakState(MatchEngine m)
        {
            _m = m;
            int next = m.Context.CurrentWaveIndex + 1;
            _delay = m.Context.Definition.waves[next].startDelay;
        }
        public void Enter()
        {
            _m.Bus.Publish(new WaveCleared(_m.Context.CurrentWaveIndex + 1));
            _t = 0f;
        }
        public void Tick(float dt)
        {
            _t += dt;
            if (_t < _delay) return;
            _m.TryStartNextWave();
            _m.Go(new PlayingState(_m));
        }
        public void Exit() { }
    }

    /// <summary>Terminal state: reports victory/defeat once.</summary>
    internal sealed class EndState : IState
    {
        readonly MatchEngine _m;
        readonly bool _victory;
        public EndState(MatchEngine m, bool victory) { _m = m; _victory = victory; }
        public void Enter() => _m.EndMatch(_victory);
        public void Tick(float dt) { }
        public void Exit() { }
    }
}
