using System.Collections.Generic;
using UnityEngine;

namespace FSCORD.Input.InputSystem
{
    /// <summary>Accumulated screen samples for one finger stroke, plus shape features.</summary>
    public sealed class GesturePath
    {
        public readonly List<Vector2> Points = new();
        public float StartTime;
        public float EndTime;

        public Vector2 Start => Points.Count > 0 ? Points[0] : default;
        public Vector2 End => Points.Count > 0 ? Points[Points.Count - 1] : default;
        public float Duration => EndTime - StartTime;

        public void Begin(Vector2 p, float t) { Points.Clear(); Points.Add(p); StartTime = t; EndTime = t; }

        public void Append(Vector2 p, float t)
        {
            if (Points.Count == 0 || (p - Points[Points.Count - 1]).sqrMagnitude > 1f) Points.Add(p);
            EndTime = t;
        }

        public float TotalLength
        {
            get { float d = 0f; for (int i = 1; i < Points.Count; i++) d += Vector2.Distance(Points[i - 1], Points[i]); return d; }
        }

        public float StraightDistance => Vector2.Distance(Start, End);

        /// <summary>1 = perfectly straight, →0 for loops.</summary>
        public float Straightness
        {
            get { float tl = TotalLength; return tl <= 0.0001f ? 1f : Mathf.Clamp01(StraightDistance / tl); }
        }

        public void Bounds(out Vector2 min, out Vector2 max)
        {
            min = new Vector2(float.MaxValue, float.MaxValue);
            max = new Vector2(float.MinValue, float.MinValue);
            for (int i = 0; i < Points.Count; i++) { min = Vector2.Min(min, Points[i]); max = Vector2.Max(max, Points[i]); }
        }

        public Vector2 Centroid
        {
            get { if (Points.Count == 0) return default; Vector2 s = Vector2.zero; for (int i = 0; i < Points.Count; i++) s += Points[i]; return s / Points.Count; }
        }

        public float Radius { get { Bounds(out var mn, out var mx); return (mx - mn).magnitude * 0.5f; } }
    }
}
