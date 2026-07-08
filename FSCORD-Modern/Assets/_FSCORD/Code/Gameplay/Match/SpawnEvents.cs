using FSCORD.Data;

namespace FSCORD.Gameplay
{
    /// <summary>
    /// Gameplay-internal command: the MatchEngine asks the WaveSpawner to create a
    /// wave's units. The engine keeps ownership of the enemy count and wave state;
    /// the spawner only instantiates. (Lives in the Gameplay assembly because it
    /// references Data - Core stays reference-free.)
    /// </summary>
    public readonly struct SpawnWaveRequested
    {
        public readonly int WaveIndex;
        public readonly WaveDefinition Wave;
        public SpawnWaveRequested(int waveIndex, WaveDefinition wave) { WaveIndex = waveIndex; Wave = wave; }
    }
}
