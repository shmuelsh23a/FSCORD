using System.Collections.Generic;
using UnityEngine;

namespace FSCORD.Core
{
    /// <summary>
    /// Central roster of live combatants. Units register on spawn and unregister on
    /// death, so AI can find the nearest enemy without per-unit physics scans - a
    /// cheap, mobile-friendly perception source and the backbone of the AI that
    /// replaces RAIN.
    /// </summary>
    public sealed class TargetRegistry : IService
    {
        readonly List<ICombatant> _all = new();

        public void Initialize() { }
        public void Shutdown() => _all.Clear();

        public void Register(ICombatant c) { if (c != null && !_all.Contains(c)) _all.Add(c); }
        public void Unregister(ICombatant c) => _all.Remove(c);

        /// <summary>Nearest living combatant of an opposing faction within range, or null.</summary>
        public ICombatant FindNearestEnemy(Faction me, Vector3 position, float maxRange)
        {
            ICombatant best = null;
            float bestSqr = maxRange * maxRange;
            for (int i = 0; i < _all.Count; i++)
            {
                var c = _all[i];
                if (c == null || !c.IsAlive || c.Faction == me || c.Faction == Faction.Neutral) continue;
                float d = (c.Position - position).sqrMagnitude;
                if (d <= bestSqr) { bestSqr = d; best = c; }
            }
            return best;
        }
    }
}
