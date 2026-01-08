using UnityEngine;

namespace TravisRFrench.Lifecycles.Runtime
{
	/// <summary>
	/// Drives the lifecycle runtime Tick() in LateUpdate.
	/// Place once in your scene (or spawn via bootstrap).
	/// </summary>
	public sealed class LifeCycleDriver : MonoBehaviour
	{
		private ILifeCycleService svc;

		private void Awake()
		{
			this.svc = LifeCycleRuntime.Service;
		}

		private void LateUpdate()
		{
			this.svc.Tick();
		}
	}
}