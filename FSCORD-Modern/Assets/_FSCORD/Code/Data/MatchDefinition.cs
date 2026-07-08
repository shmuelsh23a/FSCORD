using System.Collections.Generic;
using UnityEngine;

namespace FSCORD.Data
{
    public enum VictoryCondition { SurviveAllWaves, HoldControlPoints, Elimination }

    [System.Serializable]
    public sealed class SpawnEntry
    {
        public UnitDefinition unit;
        [Min(1)] public int count = 1;
    }

    [System.Serializable]
    public sealed class WaveDefinition
    {
        public string label;
        public List<SpawnEntry> spawns = new();
        [Tooltip("Delay before this wave begins, seconds.")]
        public float startDelay = 3f;
        [Tooltip("Time between individual unit spawns, seconds.")]
        public float spawnInterval = 1.5f;

        public int Count
        {
            get { int c = 0; foreach (var s in spawns) c += s.count; return c; }
        }
    }

    /// <summary>
    /// Mode-agnostic description of one playable match. This is the key
    /// architectural piece for Stage B: campaign missions, roguelite encounters
    /// and endless battles are all MatchDefinitions - authored by hand, or
    /// assembled by the procedural generator from the same data. The runtime
    /// MatchEngine plays any of them without knowing which mode it is.
    /// </summary>
    [CreateAssetMenu(menuName = "FSCORD/Match Definition", fileName = "Match_")]
    public sealed class MatchDefinition : ScriptableObject
    {
        public string matchId;
        public VictoryCondition victory = VictoryCondition.SurviveAllWaves;
        public List<WaveDefinition> waves = new();
        [Tooltip("Overall time limit, seconds. 0 = none.")]
        public float timeLimit = 0f;

        public int TotalEnemyCount
        {
            get { int t = 0; foreach (var w in waves) t += w.Count; return t; }
        }
    }
}
