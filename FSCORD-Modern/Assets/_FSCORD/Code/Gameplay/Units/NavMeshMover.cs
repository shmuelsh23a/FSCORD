using UnityEngine;
using UnityEngine.AI;
using FSCORD.Data;

namespace FSCORD.Gameplay
{
    /// <summary>
    /// Production mover using the built-in NavMeshAgent. The agent ships in Unity's
    /// core AIModule; baking the surface it walks on uses the AI Navigation package
    /// (NavMeshSurface). Add this component (instead of SimpleMover) to unit prefabs
    /// once a NavMesh is baked - backlog A3-4.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class NavMeshMover : MonoBehaviour, IUnitMover
    {
        NavMeshAgent _agent;

        void Awake() => _agent = GetComponent<NavMeshAgent>();

        public void Begin(UnitDefinition definition, Vector3 destination)
        {
            if (_agent == null) _agent = GetComponent<NavMeshAgent>();
            _agent.speed = definition.moveSpeed;
            _agent.angularSpeed = definition.turretTurnSpeed;
            SetDestination(destination);
        }

        public void SetDestination(Vector3 destination)
        {
            if (_agent == null || !_agent.isOnNavMesh) return;
            _agent.isStopped = false;
            _agent.SetDestination(destination);
        }

        public void Stop()
        {
            if (_agent != null && _agent.isOnNavMesh) _agent.isStopped = true;
        }
    }
}
