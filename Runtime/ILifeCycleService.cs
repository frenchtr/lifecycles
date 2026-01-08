// =========================
// File: ILifeCycleService.cs
// =========================
namespace TravisRFrench.Lifecycles.Runtime
{
	public interface ILifeCycleService
	{
		void Manage(ILifeCycleManaged instance);
		void Unmanage(ILifeCycleManaged instance);

		void RequestAwake(ILifeCycleManaged instance);
		void RequestEnable(ILifeCycleManaged instance);
		void RequestDisable(ILifeCycleManaged instance);
		void RequestDestroy(ILifeCycleManaged instance);

		/// <summary>
		/// Executes queued lifecycle work in deterministic phase barriers.
		/// Call once per frame (LateUpdate recommended).
		/// </summary>
		void Tick();
	}
}
