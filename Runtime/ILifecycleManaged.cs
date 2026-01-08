// =========================
// File: ILifeCycleManaged.cs
// =========================
namespace TravisRFrench.Lifecycles.Runtime
{
	/// <summary>
	/// Implemented by objects participating in the managed lifecycle.
	/// </summary>
	public interface ILifeCycleManaged
	{
		/// <summary>True if the instance is still alive and may process lifecycle callbacks.</summary>
		bool IsAlive { get; }

		/// <summary>
		/// Framework-owned "active" signal: true after OnLifeCycleActivate completes,
		/// false after OnLifeCycleDeactivate completes.
		/// </summary>
		bool IsActive { get; }

		/// <summary>Current phase (mirrored for debugging).</summary>
		LifecyclePhase Phase { get; }

		/// <summary>True if the instance faulted (mirrored for debugging).</summary>
		bool IsFaulted { get; }

		/// <summary>Optional fault summary (mirrored for debugging).</summary>
		string FaultSummary { get; }

		// Awake band (once)
		void Compose();
		void VerifyComposition();
		void Register();
		void Setup();

		// Enable band (per enable)
		void InitializeEnable();
		void Subscribe();
		void Activate();
		void FirstActivation();

		// Disable band (per disable)
		void Deactivate();
		void Unsubscribe();
		void FinalizeDisable();

		// Destroy band (terminal)
		void Teardown();
		void Unregister();
		void Dispose();
	}
}
