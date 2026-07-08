using UnityEngine;
using FSCORD.Core;

namespace FSCORD.Data
{
    /// <summary>
    /// Data-driven definition of a fire-support weapon. In the 2015 code these
    /// values were public fields scattered across Interface.cs and Game.cs;
    /// here each weapon is an editable asset, so balance changes (and, later,
    /// Remote Config overrides) require no code changes.
    /// </summary>
    [CreateAssetMenu(menuName = "FSCORD/Weapon Definition", fileName = "Weapon_")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        public WeaponKind kind = WeaponKind.HighExplosive;
        public string displayName;

        [Header("Damage")]
        public float damage = 100f;
        public float areaRadius = 3f;
        [Tooltip("Seconds of damage-over-time (napalm). 0 = instant impact.")]
        public float burnDuration = 0f;

        [Header("Usage")]
        [Tooltip("-1 = unlimited (default HE shell).")]
        public int startingAmmo = -1;
        public float cooldownSeconds = 0f;

        [Header("Presentation")]
        public GameObject impactVfxPrefab;
        public AudioClip impactSfx;
    }
}
