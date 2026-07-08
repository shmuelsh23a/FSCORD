using UnityEngine;
using FSCORD.Data;

namespace FSCORD.Gameplay
{
    /// <summary>
    /// Movement seam for a unit. SimpleMover (transform-based) works immediately;
    /// NavMeshMover (built-in UnityEngine.AI) is the production mover once a NavMesh
    /// is baked. The AI drives units through this interface, so swapping movers is a
    /// per-prefab choice with no code changes (backlog A3-4).
    /// </summary>
    public interface IUnitMover
    {
        void Begin(UnitDefinition definition, Vector3 destination);
        void SetDestination(Vector3 destination);
        void Stop();
    }
}
