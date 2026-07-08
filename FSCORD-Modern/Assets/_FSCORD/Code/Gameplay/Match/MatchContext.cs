using FSCORD.Data;

namespace FSCORD.Gameplay
{
    /// <summary>
    /// Mutable runtime state for a single match. In the 2015 code this lived as
    /// dozens of public fields on the Game singleton mixed with scene wiring;
    /// here it is a plain, testable data object owned by the MatchEngine.
    /// </summary>
    public sealed class MatchContext
    {
        public MatchDefinition Definition;
        public int CurrentWaveIndex = -1;
        public int EnemiesAlive;
        public int EnemiesDestroyed;
        public float Elapsed;
        public bool IsOver;
        public bool Victory;

        public bool AllWavesSpawned =>
            Definition != null && CurrentWaveIndex >= Definition.waves.Count - 1;
    }
}
