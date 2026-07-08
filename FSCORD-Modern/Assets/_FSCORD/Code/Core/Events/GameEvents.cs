using UnityEngine;

namespace FSCORD.Core
{
    // ---- Match lifecycle ----
    public readonly struct MatchStarted
    {
        public readonly string MatchId;
        public MatchStarted(string id) { MatchId = id; }
    }

    public readonly struct MatchEnded
    {
        public readonly bool Victory;
        public MatchEnded(bool victory) { Victory = victory; }
    }

    public readonly struct WaveStarted
    {
        public readonly int WaveNumber;
        public readonly int EnemyCount;
        public WaveStarted(int waveNumber, int enemyCount) { WaveNumber = waveNumber; EnemyCount = enemyCount; }
    }

    public readonly struct WaveCleared
    {
        public readonly int WaveNumber;
        public WaveCleared(int waveNumber) { WaveNumber = waveNumber; }
    }

    // ---- Combat ----
    public readonly struct TankDestroyed
    {
        public readonly Faction Faction;
        public readonly int XpWorth;
        public readonly Vector3 Position;
        public TankDestroyed(Faction faction, int xpWorth, Vector3 position)
        {
            Faction = faction; XpWorth = xpWorth; Position = position;
        }
    }

    public readonly struct ControlPointCaptured
    {
        public readonly int PointId;
        public readonly Faction NewOwner;
        public ControlPointCaptured(int pointId, Faction newOwner) { PointId = pointId; NewOwner = newOwner; }
    }

    // ---- Player fire missions (produced by the input layer, consumed by gameplay) ----
    public readonly struct FireMissionRequested
    {
        public readonly WeaponKind Weapon;
        public readonly Vector3 Target;
        public FireMissionRequested(WeaponKind weapon, Vector3 target) { Weapon = weapon; Target = target; }
    }
}
