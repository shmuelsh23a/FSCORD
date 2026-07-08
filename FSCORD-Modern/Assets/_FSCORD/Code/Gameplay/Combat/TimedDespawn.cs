using UnityEngine;
using FSCORD.Core;

namespace FSCORD.Gameplay
{
    /// <summary>Returns a pooled object (e.g. impact VFX) to its pool after a fixed lifetime.</summary>
    public sealed class TimedDespawn : MonoBehaviour, IPoolable
    {
        public float lifetime = 2f;
        float _elapsed;

        public void OnSpawned() => _elapsed = 0f;
        public void OnDespawned() { }

        void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= lifetime) GetComponent<PooledObject>()?.Release();
        }
    }
}
