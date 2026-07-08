using UnityEngine;
using FSCORD.Core;

namespace FSCORD.Data
{
    /// <summary>Behavioural flavour used by the AI layer (Stage B) and the generator.</summary>
    public enum UnitArchetype { LineTank, Cautious, Aggressive }

    /// <summary>
    /// Data-driven tank/unit definition (Abrams, M60, T-55/62/72, Type-99...).
    /// Replaces the tuning fields hard-coded on BasicTank.cs.
    /// </summary>
    [CreateAssetMenu(menuName = "FSCORD/Unit Definition", fileName = "Unit_")]
    public sealed class UnitDefinition : ScriptableObject
    {
        public string displayName;
        public Faction faction = Faction.Soviet;
        public UnitArchetype archetype = UnitArchetype.LineTank;

        [Header("Combat")]
        public float maxHitPoints = 100f;
        public float armor = 10f;
        public float damage = 40f;
        public float range = 30f;
        [Tooltip("Shots per second.")]
        public float rateOfFire = 0.2f;

        [Header("Movement")]
        public float moveSpeed = 6f;
        public float turretTurnSpeed = 90f;

        [Header("Rewards")]
        public int xpWorth = 10;

        [Header("Presentation")]
        public GameObject modelPrefab;
    }
}
