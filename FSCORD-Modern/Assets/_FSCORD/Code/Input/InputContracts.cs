using UnityEngine;
using FSCORD.Core;

namespace FSCORD.Input
{
    /// <summary>
    /// Abstraction over raw touch input. A concrete implementation (Input System
    /// + Enhanced Touch - backlog A2-7) recognizes the six weapon gestures and
    /// camera controls, then publishes FireMissionRequested and the camera
    /// intents below on the EventBus. Gameplay depends on this interface, never
    /// on the input backend, so the recognizer can change without touching
    /// combat code.
    /// </summary>
    public interface IInputService : IService
    {
        void Tick(float deltaTime);
    }

    public readonly struct CameraPanIntent
    {
        public readonly Vector2 Delta;
        public CameraPanIntent(Vector2 delta) { Delta = delta; }
    }

    public readonly struct CameraZoomIntent
    {
        public readonly float Delta;
        public CameraZoomIntent(float delta) { Delta = delta; }
    }

    public readonly struct CameraRotateIntent
    {
        public readonly float DegreesDelta;
        public CameraRotateIntent(float degreesDelta) { DegreesDelta = degreesDelta; }
    }
}
