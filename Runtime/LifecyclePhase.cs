namespace TravisRFrench.Lifecycles.Runtime
{
    public enum LifecyclePhase
    {
        // Initial state
        None = 0,
        
        // Awake()
        Composed = 10,
        Verified = 20,
        Registered = 30,
        Wired = 40,
        Initialized = 50,
        
        // OnEnable()
        Subscribed = 60,
        Activated = 70,
        
        // OnDisable()
        Deactivated = 80,
        Unsubscribed = 90,
        Unwired = 100,
        Unregistered = 110,
        Disposed = 120,
    }
}
