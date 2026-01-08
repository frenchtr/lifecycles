// =========================
// File: LifecyclePhase.cs
// =========================
namespace TravisRFrench.Lifecycles.Runtime
{
	/// <summary>
	/// Framework-owned lifecycle phases (contract boundaries).
	/// </summary>
	public enum LifecyclePhase
	{
		None = 0,

		// Awake band (once per lifetime)
		Assembled = 10,
		VerifiedStructure = 20,
		Registered = 30,
		Setup = 40,

		// Enable band (each enable)
		InitializedEnable = 50,
		Subscribed = 60,
		Activated = 70,
		FirstActivation = 80,

		// Disable band (each disable)
		Deactivated = 90,
		Unsubscribed = 100,
		FinalizedDisable = 110,

		// Destroy band (on destroy)
		Teardown = 120,
		Unregistered = 130,
		Disposed = 140,

		Faulted = 1000
	}
}
