namespace FSCORD.Core
{
    /// <summary>Allegiance of a unit / control point. (Was ItemIdentity in the 2015 code.)</summary>
    public enum Faction { Neutral, American, Soviet }

    /// <summary>The player's fire-support options, driven by touch gestures.</summary>
    public enum WeaponKind
    {
        HighExplosive,      // tap
        ConcentratedStrike, // circle (slow-motion)
        Napalm,             // swipe
        DaisyCutter,        // X
        Mines,              // press & drag
        TacticalNuke        // shake
    }
}
