using UnityEngine;

namespace FSCORD.Core
{
    /// <summary>
    /// Composition root. Place on a single GameObject in the Boot scene. Creates
    /// the EventBus and the core services once, survives scene loads, and exposes
    /// them to the rest of the game. Additional services (Save, Analytics, Economy,
    /// Input, Ads) register here as they are implemented (backlog A4/A5).
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        public static GameBootstrap Instance { get; private set; }

        public EventBus Events { get; private set; }
        public ServiceLocator Services { get; private set; }

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Events = new EventBus();
            Services = new ServiceLocator();

            var pools = gameObject.AddComponent<PoolService>();
            pools.Initialize();
            Services.Register(pools);

            var targets = new TargetRegistry();
            targets.Initialize();
            Services.Register(targets);
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
