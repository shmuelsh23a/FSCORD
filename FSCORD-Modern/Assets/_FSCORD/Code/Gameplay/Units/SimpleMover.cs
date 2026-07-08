using UnityEngine;
using FSCORD.Data;

namespace FSCORD.Gameplay
{
    /// <summary>
    /// Placeholder mover: drives straight toward the destination at the unit's move
    /// speed and faces travel direction. Fine before a NavMesh is baked; swap for
    /// NavMeshMover in production. Kept behind IUnitMover so nothing else changes.
    /// </summary>
    public sealed class SimpleMover : MonoBehaviour, IUnitMover
    {
        Vector3 _destination;
        float _speed;
        bool _moving;

        public void Begin(UnitDefinition definition, Vector3 destination)
        {
            _speed = definition.moveSpeed;
            SetDestination(destination);
        }

        public void SetDestination(Vector3 destination)
        {
            _destination = destination;
            _moving = true;
        }

        public void Stop() => _moving = false;

        void Update()
        {
            if (!_moving) return;

            Vector3 pos = transform.position;
            Vector3 to = _destination - pos;
            to.y = 0f;
            float dist = to.magnitude;
            if (dist < 0.1f) { _moving = false; return; }

            Vector3 dir = to / dist;
            transform.position = pos + dir * (_speed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 8f * Time.deltaTime);
        }
    }
}
