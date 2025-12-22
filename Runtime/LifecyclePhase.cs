namespace TravisRFrench.Lifecycles.Runtime
{
	public enum LifecyclePhase
	{
		None = 0,

		Composed = 10,
		Registered = 20,
		Initialized = 30,
		Bound = 40,
		Activated = 50,

		Deactivated = 60,
		Unbound = 70,
		Unregistered = 80,
		Disposed = 90
	}
}
