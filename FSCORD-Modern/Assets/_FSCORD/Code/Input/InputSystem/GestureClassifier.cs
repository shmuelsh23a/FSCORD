using UnityEngine;
using FSCORD.Input;

namespace FSCORD.Input.InputSystem
{
    public enum GestureKind { None, Tap, Circle, Swipe, Drag }

    public struct GestureResult
    {
        public GestureKind Kind;
        public Vector2 PointA; // tap point / circle centre / line start
        public Vector2 PointB; // line end (swipe/drag)
    }

    /// <summary>
    /// Classifies one completed single-finger stroke. X (daisy cutter) is not a
    /// single stroke - it is two crossing swipes, arbitrated by the input service.
    /// </summary>
    public sealed class GestureClassifier
    {
        readonly GestureSettings _s;
        public GestureClassifier(GestureSettings settings) { _s = settings; }

        public GestureResult Classify(GesturePath path)
        {
            var r = new GestureResult { Kind = GestureKind.None };
            if (path.Points.Count == 0) return r;

            // Tap: short and nearly stationary.
            if (path.Duration <= _s.tapMaxDuration && path.TotalLength <= _s.tapMaxMove)
            {
                r.Kind = GestureKind.Tap; r.PointA = path.Start; return r;
            }

            // Circle: returns near its start, low straightness, meaningful radius.
            bool closed = path.StraightDistance <= _s.circleClosureDistance;
            if (closed && path.Straightness < _s.circleMaxStraightness && path.Radius >= _s.minCircleRadius)
            {
                r.Kind = GestureKind.Circle; r.PointA = path.Centroid; return r;
            }

            // Straight stroke: fast = swipe (napalm), slow = drag (mines).
            if (path.Straightness >= _s.swipeMinStraightness)
            {
                float speed = path.Duration > 0.0001f ? path.TotalLength / path.Duration : 0f;
                r.PointA = path.Start; r.PointB = path.End;
                r.Kind = speed >= _s.swipeMinSpeed ? GestureKind.Swipe : GestureKind.Drag;
                return r;
            }

            // Ambiguous (curved but open): ignore rather than misfire.
            r.PointA = path.Start; r.PointB = path.End;
            return r;
        }
    }
}
