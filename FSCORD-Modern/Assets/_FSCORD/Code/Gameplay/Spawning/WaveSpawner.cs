using System.Collections;
using UnityEngine;
using FSCORD.Core;
using FSCORD.Data;

namespace FSCORD.Gameplay
{
    /// <summary>
    /// Instantiates each wave's units over time from the object pool (backlog
    /// A3-7). Listens for SpawnWaveRequested from the MatchEngine. Place in the
    /// Game scene and assign spawn points + the objective the enemy advances on.
    /// </summary>
    public sealed class WaveSpawner : MonoBehaviour
    {
        [SerializeField] Transform[] spawnPoints;
        [SerializeField] Transform objective;

        EventBus _bus;
        PoolService _pool;

        public void Configure(EventBus bus, PoolService pool)
        {
            _bus = bus;
            _pool = pool;
        }

        void Start()
        {
            if (_bus == null && GameBootstrap.Instance != null) _bus = GameBootstrap.Instance.Events;
            if (_pool == null && GameBootstrap.Instance != null) GameBootstrap.Instance.Services.TryGet(out _pool);
            _bus?.Subscribe<SpawnWaveRequested>(OnSpawnWave);
        }

        void OnDestroy() => _bus?.Unsubscribe<SpawnWaveRequested>(OnSpawnWave);

        void OnSpawnWave(SpawnWaveRequested cmd)
        {
            if (cmd.Wave != null) StartCoroutine(SpawnRoutine(cmd.Wave));
        }

        IEnumerator SpawnRoutine(WaveDefinition wave)
        {
            var wait = new WaitForSeconds(Mathf.Max(0f, wave.spawnInterval));
            foreach (var entry in wave.spawns)
            {
                if (entry == null || entry.unit == null || entry.unit.modelPrefab == null) continue;
                for (int i = 0; i < entry.count; i++)
                {
                    SpawnOne(entry.unit);
                    yield return wait;
                }
            }
        }

        void SpawnOne(UnitDefinition unit)
        {
            if (_pool == null) return;

            Transform sp = PickSpawnPoint();
            Vector3 pos = sp != null ? sp.position : transform.position;
            Quaternion rot = sp != null ? sp.rotation : Quaternion.identity;

            GameObject go = _pool.Spawn(unit.modelPrefab, pos, rot);
            var tank = go.GetComponent<TankUnit>();
            if (tank == null) tank = go.AddComponent<TankUnit>();

            Vector3 dest = objective != null ? objective.position : transform.position;
            tank.Init(unit, _bus, dest);
        }

        Transform PickSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0) return null;
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
    }
}
