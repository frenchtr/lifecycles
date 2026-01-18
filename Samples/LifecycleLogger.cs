// =========================
// File: LifecycleLogger.cs
// =========================

using UnityEngine;

namespace TravisRFrench.Lifecycles.Runtime
{
	/// <summary>
	/// Logs lifecycle callbacks as they are executed by the lifecycle runtime (Tick()).
	/// Useful for validating ordering and phase barriers.
	/// </summary>
	public sealed class LifecycleLogger : ManagedMonoBehaviour
	{
		[SerializeField] private bool includeFrameCount = true;
		[SerializeField] private bool includeHierarchyPath = false;
		[SerializeField] private bool includeInstanceId = false;
		[SerializeField] private bool includePhaseAfterCallback = true;

		private string Prefix
		{
			get
			{
				var frame = this.includeFrameCount ? $"[frame:{Time.frameCount}] " : string.Empty;
				var id = this.includeInstanceId ? $"(id:{this.GetInstanceID()}) " : string.Empty;
				var who = this.includeHierarchyPath ? GetHierarchyPath(this.transform) : this.gameObject.name;
				return $"{frame}{id}[{who}] ";
			}
		}

		private void Log(string callbackName)
		{
			if (this.includePhaseAfterCallback)
			{
				Debug.Log($"{this.Prefix}{callbackName} -> Phase={this.Phase} Active={this.IsActive} Faulted={this.IsFaulted}");
			}
			else
			{
				Debug.Log($"{this.Prefix}{callbackName}");
			}
		}

		// Awake band
		protected override void OnLifecycleCompose() => Log(nameof(ILifeCycleManaged.Compose));
		protected override void OnLifecycleVerifyComposition() => Log(nameof(ILifeCycleManaged.VerifyComposition));
		protected override void OnLifecycleRegister() => Log(nameof(ILifeCycleManaged.Register));
		protected override void OnLifecycleSetup() => Log(nameof(ILifeCycleManaged.Setup));

		// Enable band
		protected override void OnLifecycleInitializeEnable() => Log(nameof(ILifeCycleManaged.InitializeEnable));
		protected override void OnLifecycleSubscribe() => Log(nameof(ILifeCycleManaged.Subscribe));
		protected override void OnLifecycleActivate() => Log(nameof(ILifeCycleManaged.Activate));
		protected override void OnLifecycleFirstActivation() => Log(nameof(ILifeCycleManaged.FirstActivation));

		// Disable band
		protected override void OnLifecycleDeactivate() => Log(nameof(ILifeCycleManaged.Deactivate));
		protected override void OnLifecycleUnsubscribe() => Log(nameof(ILifeCycleManaged.Unsubscribe));
		protected override void OnLifecycleFinalizeDisable() => Log(nameof(ILifeCycleManaged.FinalizeDisable));

		// Destroy band
		protected override void OnLifecycleTeardown() => Log(nameof(ILifeCycleManaged.Teardown));
		protected override void OnLifecycleUnregister() => Log(nameof(ILifeCycleManaged.Unregister));
		protected override void OnLifecycleDispose() => Log(nameof(ILifeCycleManaged.Dispose));

		private static string GetHierarchyPath(Transform t)
		{
			if (t == null) return "<null>";

			// Build "Root/Child/Grandchild"
			var path = t.name;
			while (t.parent != null)
			{
				t = t.parent;
				path = $"{t.name}/{path}";
			}

			return path;
		}
	}
}
