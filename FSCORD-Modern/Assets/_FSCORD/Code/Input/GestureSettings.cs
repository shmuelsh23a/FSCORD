using UnityEngine;

namespace FSCORD.Input
{
    /// <summary>
    /// Tunable thresholds for gesture recognition. An asset so designers can
    /// tune feel without code (and, later, override via Remote Config).
    /// Screen thresholds are in pixels; consider scaling by DPI for shipping.
    /// </summary>
    [CreateAssetMenu(menuName = "FSCORD/Gesture Settings", fileName = "GestureSettings")]
    public sealed class GestureSettings : ScriptableObject
    {
        [Header("Tap (High Explosive)")]
        public float tapMaxDuration = 0.2f;
        public float tapMaxMove = 20f;

        [Header("Circle (Concentrated Strike)")]
        public float circleClosureDistance = 60f;
        [Range(0f, 1f)] public float circleMaxStraightness = 0.55f;
        public float minCircleRadius = 40f;

        [Header("Swipe / Drag (Napalm / Mines)")]
        [Range(0f, 1f)] public float swipeMinStraightness = 0.8f;
        [Tooltip("Pixels per second above which a straight stroke is a swipe (napalm); slower is a drag (mines).")]
        public float swipeMinSpeed = 800f;

        [Header("X (Daisy Cutter)")]
        [Tooltip("Max seconds between two crossing swipes to read them as an X.")]
        public float xMaxStrokeGap = 0.4f;

        [Header("Shake (Tactical Nuke)")]
        [Tooltip("Filtered acceleration magnitude (g) to trigger.")]
        public float shakeThreshold = 2.2f;
        public float shakeCooldown = 1.0f;
    }
}
