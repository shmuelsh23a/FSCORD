using UnityEngine;

namespace FSCORD.Core
{
    /// <summary>
    /// A damageable, locatable combat participant. Used both by fire missions
    /// (area damage) and by unit AI (perception via TargetRegistry). Lives in Core
    /// so the registry stays reference-free of gameplay types.
    /// </summary>
    public interface ICombatant
    {
        Faction Faction { get; }
        Vector3 Position { get; }
        bool IsAlive { get; }
        void ApplyDamage(float amount);
    }
}
