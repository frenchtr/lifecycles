// =========================
// File: ILifecycleDebugStateSink.cs (internal)
// =========================
namespace TravisRFrench.Lifecycles.Runtime
{
	/// <summary>
	/// Internal hook the lifecycle runtime can use to mirror state onto a component for debugging.
	/// Not user-settable and not serialized.
	/// </summary>
	internal interface ILifecycleDebugStateSink
	{
		void __SetPhase(LifecyclePhase phase);
		void __SetIsActive(bool isActive);
		void __SetIsFaulted(bool isFaulted);
		void __SetFaultSummary(string summary);
	}
}
