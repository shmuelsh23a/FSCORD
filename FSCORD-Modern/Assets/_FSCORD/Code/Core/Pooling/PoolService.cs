using System.Collections.Generic;
using UnityEngine;

namespace FSCORD.Core
{
    /// <summary>
    /// Pools GameObject instances keyed by prefab. This is the single biggest
    /// runtime win over the 2015 code, which Instantiate/Destroy-ed shells,
    /// explosions, tanks and mines every frame of combat (the main GC-stutter
    /// source on mobile). Spawn/Despawn here reuse instances instead. Each
    /// spawned instance carries a PooledObject so it can return itself.
    /// </summary>
    public sealed class PoolService : MonoBehaviour, IService
    {
        readonly Dictionary<GameObject, Queue<GameObject>> _pools = new();
        readonly List<IPoolable> _buffer = new();
        Transform _root;

        public void Initialize()
        {
            _root = new GameObject("~PooledObjects").transform;
            _root.SetParent(transform, false);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var queue = GetQueue(prefab);
            GameObject go = queue.Count > 0 ? queue.Dequeue() : Instantiate(prefab);

            go.transform.SetParent(null, false);
            go.transform.SetPositionAndRotation(position, rotation);

            var po = go.GetComponent<PooledObject>();
            if (po == null) po = go.AddComponent<PooledObject>();
            po.Bind(prefab, this);

            go.SetActive(true);
            Notify(go, spawned: true);
            return go;
        }

        /// <summary>Return an instance created by Spawn() to its pool.</summary>
        public void Despawn(GameObject prefab, GameObject instance)
        {
            Notify(instance, spawned: false);
            instance.SetActive(false);
            instance.transform.SetParent(_root, false);
            GetQueue(prefab).Enqueue(instance);
        }

        public void Prewarm(GameObject prefab, int count)
        {
            var queue = GetQueue(prefab);
            for (int i = 0; i < count; i++)
            {
                var go = Instantiate(prefab);
                go.SetActive(false);
                go.transform.SetParent(_root, false);
                queue.Enqueue(go);
            }
        }

        public void Shutdown() => _pools.Clear();

        Queue<GameObject> GetQueue(GameObject prefab)
        {
            if (!_pools.TryGetValue(prefab, out var q)) { q = new Queue<GameObject>(); _pools[prefab] = q; }
            return q;
        }

        void Notify(GameObject go, bool spawned)
        {
            go.GetComponentsInChildren(_buffer);
            for (int i = 0; i < _buffer.Count; i++)
            {
                if (spawned) _buffer[i].OnSpawned();
                else _buffer[i].OnDespawned();
            }
        }
    }
}
