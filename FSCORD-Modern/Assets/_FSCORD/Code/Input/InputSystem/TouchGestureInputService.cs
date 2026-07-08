using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Utilities;
using FSCORD.Core;
using FSCORD.Input;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

namespace FSCORD.Input.InputSystem
{
    /// <summary>
    /// Concrete input service (backlog A2-7). Recognizes the six weapon gestures
    /// and multi-touch camera controls via the Input System's Enhanced Touch, and
    /// publishes intent events on the EventBus. Gameplay subscribes to those events
    /// and never sees raw touch. Thresholds live in a GestureSettings asset.
    ///
    /// Gesture map: tap = HE, circle = Concentrated, swipe = Napalm,
    /// X (two crossing swipes) = Daisy Cutter, drag = Mines, shake = Nuke.
    /// </summary>
    [DefaultExecutionOrder(-500)]
    public sealed class TouchGestureInputService : MonoBehaviour, IInputService
    {
        [SerializeField] GestureSettings settings;
        [SerializeField] Camera worldCamera;
        [SerializeField] float groundY = 0f;

        EventBus _bus;
        GestureClassifier _classifier;
        readonly Dictionary<int, GesturePath> _paths = new();

        // X (cross) arbitration
        GesturePath _pendingSwipe;
        float _pendingSwipeTime;

        // two-finger camera state
        bool _hadTwo;
        Vector2 _prevMid;
        float _prevDist;
        float _prevAngle;

        // shake
        Vector3 _accelLowPass;
        float _lastShakeTime;

        /// <summary>Inject the shared bus + camera (e.g. from a level loader).</summary>
        public void Configure(EventBus bus, Camera cam)
        {
            _bus = bus;
            if (cam != null) worldCamera = cam;
        }

        void Awake()
        {
            if (_bus == null && GameBootstrap.Instance != null) _bus = GameBootstrap.Instance.Events;
            if (worldCamera == null) worldCamera = Camera.main;
            if (settings == null) settings = ScriptableObject.CreateInstance<GestureSettings>();
            _classifier = new GestureClassifier(settings);
        }

        public void Initialize()
        {
            EnhancedTouchSupport.Enable();
            if (Accelerometer.current != null) UnityEngine.InputSystem.InputSystem.EnableDevice(Accelerometer.current);
        }

        public void Shutdown() => EnhancedTouchSupport.Disable();

        void OnEnable() { if (!EnhancedTouchSupport.enabled) EnhancedTouchSupport.Enable(); }

        void Update() => Tick(Time.deltaTime);

        public void Tick(float deltaTime)
        {
            if (_bus == null) return;

            var active = ETouch.Touch.activeTouches;
            int n = active.Count;

            if (n >= 2)
            {
                HandleCamera(active);
                _paths.Clear();          // cancel single-finger strokes while panning
            }
            else
            {
                _hadTwo = false;
                if (n == 1) HandleSingleTouch(active[0]);
            }

            HandleShake();
            CommitAgedSwipe(Time.time);
        }

        // ---- single-finger gestures ----
        void HandleSingleTouch(ETouch.Touch touch)
        {
            int id = touch.touchId;
            switch (touch.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    var np = new GesturePath();
                    np.Begin(touch.screenPosition, Time.time);
                    _paths[id] = np;
                    break;

                case UnityEngine.InputSystem.TouchPhase.Moved:
                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    if (_paths.TryGetValue(id, out var mp)) mp.Append(touch.screenPosition, Time.time);
                    break;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                    if (_paths.TryGetValue(id, out var ep)) { _paths.Remove(id); CompleteStroke(ep); }
                    break;

                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    _paths.Remove(id);
                    break;
            }
        }

        void CompleteStroke(GesturePath path)
        {
            var g = _classifier.Classify(path);
            switch (g.Kind)
            {
                case GestureKind.Tap:    Fire(WeaponKind.HighExplosive, g.PointA, g.PointA); break;
                case GestureKind.Circle: Fire(WeaponKind.ConcentratedStrike, g.PointA, g.PointA); break;
                case GestureKind.Drag:   Fire(WeaponKind.Mines, g.PointA, g.PointB); break;
                case GestureKind.Swipe:  HandleSwipe(path); break;
                default: break; // ignore ambiguous strokes
            }
        }

        // Two crossing swipes within the time gap = X = Daisy Cutter; otherwise Napalm.
        void HandleSwipe(GesturePath path)
        {
            if (_pendingSwipe != null &&
                Time.time - _pendingSwipeTime <= settings.xMaxStrokeGap &&
                SegmentsCross(_pendingSwipe.Start, _pendingSwipe.End, path.Start, path.End, out var crossPx))
            {
                Fire(WeaponKind.DaisyCutter, crossPx, crossPx);
                _pendingSwipe = null;
                return;
            }

            CommitPendingSwipeNow();
            _pendingSwipe = path;
            _pendingSwipeTime = Time.time;
        }

        void CommitAgedSwipe(float now)
        {
            if (_pendingSwipe != null && now - _pendingSwipeTime > settings.xMaxStrokeGap)
                CommitPendingSwipeNow();
        }

        void CommitPendingSwipeNow()
        {
            if (_pendingSwipe == null) return;
            Fire(WeaponKind.Napalm, _pendingSwipe.Start, _pendingSwipe.End);
            _pendingSwipe = null;
        }

        // ---- camera (two fingers) ----
        void HandleCamera(ReadOnlyArray<ETouch.Touch> touches)
        {
            Vector2 a = touches[0].screenPosition;
            Vector2 b = touches[1].screenPosition;
            Vector2 mid = (a + b) * 0.5f;
            float dist = Vector2.Distance(a, b);
            float ang = Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;

            if (_hadTwo)
            {
                Vector2 pan = mid - _prevMid;
                float zoom = dist - _prevDist;
                float rot = Mathf.DeltaAngle(_prevAngle, ang);
                if (pan.sqrMagnitude > 0.01f) _bus.Publish(new CameraPanIntent(pan));
                if (Mathf.Abs(zoom) > 0.01f) _bus.Publish(new CameraZoomIntent(zoom));
                if (Mathf.Abs(rot) > 0.01f) _bus.Publish(new CameraRotateIntent(rot));
            }

            _prevMid = mid; _prevDist = dist; _prevAngle = ang; _hadTwo = true;
        }

        // ---- shake (accelerometer) ----
        void HandleShake()
        {
            var accel = Accelerometer.current;
            if (accel == null) return;

            Vector3 a = accel.acceleration.ReadValue();
            _accelLowPass = Vector3.Lerp(_accelLowPass, a, 0.1f);
            Vector3 delta = a - _accelLowPass;

            if (delta.magnitude >= settings.shakeThreshold && Time.time - _lastShakeTime >= settings.shakeCooldown)
            {
                _lastShakeTime = Time.time;
                var centre = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                Fire(WeaponKind.TacticalNuke, centre, centre);
            }
        }

        // ---- emit ----
        void Fire(WeaponKind weapon, Vector2 aScreen, Vector2 bScreen)
        {
            if (!ScreenToWorld.ToGround(worldCamera, aScreen, groundY, out var a)) return;
            Vector3 b = a;
            if (bScreen != aScreen) ScreenToWorld.ToGround(worldCamera, bScreen, groundY, out b);
            _bus.Publish(new FireMissionRequested(weapon, a, b));
        }

        // 2D segment intersection; returns the crossing point if the segments cross.
        static bool SegmentsCross(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 hit)
        {
            hit = default;
            float d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);
            if (Mathf.Abs(d) < 1e-5f) return false;
            float u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
            float v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;
            if (u < 0f || u > 1f || v < 0f || v > 1f) return false;
            hit = p1 + u * (p2 - p1);
            return true;
        }
    }
}
