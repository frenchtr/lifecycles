// =========================
// File: LifeCycleDriver.cs
// =========================

using UnityEngine;

namespace TravisRFrench.Lifecycles.Runtime
{
    /// <summary>
    /// Drives the lifecycle runtime Tick() in LateUpdate.
    /// Auto-instantiated as a DontDestroyOnLoad singleton if none exists.
    /// </summary>
    public sealed class LifeCycleDriver : MonoBehaviour
    {
        private const string DriverObjectName = "[LifeCycleDriver]";
        private static LifeCycleDriver instance;

        private ILifeCycleService svc;

        public static LifeCycleDriver Instance
        {
            get
            {
                if (instance != null) return instance;

                // Try to locate an existing instance first (scene or DDOL).
#if UNITY_2023_1_OR_NEWER
                instance = FindFirstObjectByType<LifeCycleDriver>();
#else
				instance = FindObjectOfType<LifeCycleDriver>();
#endif
                if (instance != null) return instance;

                // Create one if none exists.
                var go = new GameObject(DriverObjectName);
                DontDestroyOnLoad(go);
                instance = go.AddComponent<LifeCycleDriver>();
                return instance;
            }
        }

        // Ensures the driver exists before any scene Awake runs.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureExists()
        {
            _ = Instance;
        }

        private void Awake()
        {
            // Singleton enforcement.
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);

            // Name it consistently (nice for hierarchy/debug).
            if (this.gameObject.name != DriverObjectName)
            {
                this.gameObject.name = DriverObjectName;
            }

            this.svc = LifeCycleRuntime.Service;
        }

        private void LateUpdate()
        {
            this.svc.Tick();
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
