using UnityEngine;

namespace FSCORD.Core
{
    /// <summary>
    /// Attached automatically by PoolService when an instance is spawned, so any
    /// pooled object (shell, VFX, tank, mine) can return itself to its pool
    /// without the caller tracking the source prefab.
    /// </summary>
    public sealed class PooledObject : MonoBehaviour
    {
        GameObject _prefab;
        PoolService _service;
        bool _released;

        public void Bind(GameObject prefab, PoolService service)
        {
            _prefab = prefab;
            _service = service;
            _released = false;
        }

        /// <summary>Return this instance to its pool. Safe to call more than once.</summary>
        public void Release()
        {
            if (_released || _service == null) return;
            _released = true;
            _service.Despawn(_prefab, gameObject);
        }
    }
}
